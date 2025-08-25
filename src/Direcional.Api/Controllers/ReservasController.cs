using Direcional.Api.Domain;
using Direcional.Api.Dtos;
using Direcional.Api.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Direcional.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ReservasController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ReservasController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservaReadDto>>> GetAll()
            => Ok(await _db.Reservas.AsNoTracking()
                .Select(r => new ReservaReadDto(r.Id, r.IdCliente, r.IdApartamento, r.DataReserva, r.Validade, r.Status))
                .ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ReservaReadDto>> GetById(int id)
        {
            var r = await _db.Reservas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (r == null) return NotFound();
            return Ok(new ReservaReadDto(r.Id, r.IdCliente, r.IdApartamento, r.DataReserva, r.Validade, r.Status));
        }



        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ReservaReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ReservaReadDto>> Create([FromBody] ReservaCreateDto dto)
        {
            if (!await _db.Clientes.AnyAsync(c => c.Id == dto.IdCliente))
                return BadRequest("Cliente nao encontrado.");

            var apto = await _db.Apartamentos.FirstOrDefaultAsync(a => a.Id == dto.IdApartamento);
            if (apto == null) return BadRequest("Apartamento nao encontrado.");
            if (!apto.Disponivel) return Conflict("Apartamento indisponivel.");

            if (await _db.Reservas.AnyAsync(r => r.IdApartamento == apto.Id && r.Status == ReservaStatus.Ativa))
                return Conflict("Apartamento ja possui reserva ativa.");

            var r = new Reserva
            {
                IdCliente = dto.IdCliente,
                IdApartamento = dto.IdApartamento,
                DataReserva = dto.DataReserva,
                Validade   = dto.Validade,
                Status     = ReservaStatus.Ativa
            };

            try
            {
                _db.Reservas.Add(r);
                apto.Disponivel = false;

                // EF Core já envolve este SaveChanges em UMA transação
                await _db.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = r.Id },
                    new ReservaReadDto(r.Id, r.IdCliente, r.IdApartamento, r.DataReserva, r.Validade, r.Status));
            }
            catch (DbUpdateException ex) when (ex.GetBaseException() is SqlException sql && (sql.Number == 2601 || sql.Number == 2627))
            {
                return Conflict("Apartamento ja possui reserva ativa.");
            }
            catch (DbUpdateException ex) when (ex.GetBaseException() is SqlException sql && sql.Number == 547)
            {
                return BadRequest("Dados inconsistentes (FK). Verifique idCliente e idApartamento.");
            }
        }



        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, ReservaUpdateDto dto)
        {
            var r = await _db.Reservas.FirstOrDefaultAsync(x => x.Id == id);
            if (r == null) return NotFound();

            if (!await _db.Clientes.AnyAsync(c => c.Id == dto.IdCliente))
                return BadRequest("Cliente nao encontrado.");

            // troca de apartamento: validar disponibilidade
            if (r.IdApartamento != dto.IdApartamento)
            {
                var novo = await _db.Apartamentos.FirstOrDefaultAsync(a => a.Id == dto.IdApartamento);
                if (novo == null) return BadRequest("Apartamento nao encontrado.");
                if (!novo.Disponivel) return Conflict("Novo apartamento indisponivel.");
                bool reservado = await _db.Reservas.AnyAsync(x => x.IdApartamento == novo.Id && x.Status == ReservaStatus.Ativa);
                if (reservado) return Conflict("Novo apartamento ja reservado.");

                var antigo = await _db.Apartamentos.FirstAsync(a => a.Id == r.IdApartamento);
                antigo.Disponivel = true; // libera antigo
                novo.Disponivel = false;  // bloqueia novo
                r.IdApartamento = dto.IdApartamento;
            }

            // se cancelar, libera o apto
            if (r.Status != ReservaStatus.Cancelada && dto.Status == ReservaStatus.Cancelada)
            {
                var apto = await _db.Apartamentos.FirstOrDefaultAsync(a => a.Id == r.IdApartamento);
                if (apto != null) apto.Disponivel = true;
            }

            r.IdCliente = dto.IdCliente;
            r.DataReserva = dto.DataReserva;
            r.Validade = dto.Validade;
            r.Status = dto.Status;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var r = await _db.Reservas.FirstOrDefaultAsync(x => x.Id == id);
            if (r == null) return NotFound();

            var apto = await _db.Apartamentos.FirstOrDefaultAsync(a => a.Id == r.IdApartamento);
            if (apto != null && r.Status == ReservaStatus.Ativa) apto.Disponivel = true;

            _db.Reservas.Remove(r);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // POST api/reservas/{id}/confirmar -> cria venda e converte reserva
        [HttpPost("{id:int}/confirmar")]
        public async Task<ActionResult<VendaReadDto>> ConfirmarParaVenda(int id, [FromBody] decimal valorFinal)
        {
            var r = await _db.Reservas.FirstOrDefaultAsync(x => x.Id == id);
            if (r == null) return NotFound();
            if (r.Status != ReservaStatus.Ativa) return Conflict("Reserva nao esta ativa.");

            // apto ja esta indisponivel por causa da reserva
            if (!await _db.Clientes.AnyAsync(c => c.Id == r.IdCliente))
                return BadRequest("Cliente nao encontrado.");

            var venda = new Venda
            {
                IdCliente = r.IdCliente,
                IdApartamento = r.IdApartamento,
                DataVenda = DateTime.UtcNow,
                ValorFinal = valorFinal
            };

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                _db.Vendas.Add(venda);
                r.Status = ReservaStatus.Convertida;
                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch { await tx.RollbackAsync(); throw; }

            return CreatedAtAction("GetById", "Vendas",
                new { id = venda.Id }, new VendaReadDto(venda.Id, venda.IdCliente, venda.IdApartamento, venda.DataVenda, venda.ValorFinal));
        }
    }
}
