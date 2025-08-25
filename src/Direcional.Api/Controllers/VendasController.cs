using Direcional.Api.Domain;
using Direcional.Api.Dtos;
using Direcional.Api.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Direcional.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendasController : ControllerBase
    {
        private readonly AppDbContext _db;
        public VendasController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendaReadDto>>> GetAll()
            => Ok(await _db.Vendas.AsNoTracking()
                .Select(v => new VendaReadDto(v.Id, v.IdCliente, v.IdApartamento, v.DataVenda, v.ValorFinal))
                .ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<VendaReadDto>> GetById(int id)
        {
            var v = await _db.Vendas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (v == null) return NotFound();
            return Ok(new VendaReadDto(v.Id, v.IdCliente, v.IdApartamento, v.DataVenda, v.ValorFinal));
        }

        [HttpPost]
        public async Task<ActionResult<VendaReadDto>> Create(VendaCreateDto dto)
        {
            if (!await _db.Clientes.AnyAsync(c => c.Id == dto.IdCliente))
                return BadRequest("Cliente nao encontrado.");

            var apto = await _db.Apartamentos.FirstOrDefaultAsync(a => a.Id == dto.IdApartamento);
            if (apto == null) return BadRequest("Apartamento nao encontrado.");
            if (!apto.Disponivel) return Conflict("Apartamento indisponivel (vendido ou reservado).");

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var venda = new Venda
                {
                    IdCliente = dto.IdCliente,
                    IdApartamento = dto.IdApartamento,
                    DataVenda = dto.DataVenda,
                    ValorFinal = dto.ValorFinal
                };

                _db.Vendas.Add(venda);
                apto.Disponivel = false;

                // se existir reserva ativa para esse apto, converte/cancela
                var reservaAtiva = await _db.Reservas.FirstOrDefaultAsync(r => r.IdApartamento == apto.Id && r.Status == ReservaStatus.Ativa);
                if (reservaAtiva != null) reservaAtiva.Status = ReservaStatus.Convertida;

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = venda.Id },
                    new VendaReadDto(venda.Id, venda.IdCliente, venda.IdApartamento, venda.DataVenda, venda.ValorFinal));
            }
            catch { await tx.RollbackAsync(); throw; }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, VendaUpdateDto dto)
        {
            var v = await _db.Vendas.FirstOrDefaultAsync(x => x.Id == id);
            if (v == null) return NotFound();

            if (!await _db.Clientes.AnyAsync(c => c.Id == dto.IdCliente))
                return BadRequest("Cliente nao encontrado.");

            // troca de apto: validar disponibilidade
            if (v.IdApartamento != dto.IdApartamento)
            {
                var novo = await _db.Apartamentos.FirstOrDefaultAsync(a => a.Id == dto.IdApartamento);
                if (novo == null) return BadRequest("Apartamento nao encontrado.");
                if (!novo.Disponivel) return Conflict("Novo apartamento indisponivel.");

                var antigo = await _db.Apartamentos.FirstAsync(a => a.Id == v.IdApartamento);
                antigo.Disponivel = true; // libera antigo
                novo.Disponivel = false;  // bloqueia novo
                v.IdApartamento = dto.IdApartamento;
            }

            v.IdCliente = dto.IdCliente;
            v.DataVenda = dto.DataVenda;
            v.ValorFinal = dto.ValorFinal;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var v = await _db.Vendas.FirstOrDefaultAsync(x => x.Id == id);
            if (v == null) return NotFound();

            var apto = await _db.Apartamentos.FirstOrDefaultAsync(a => a.Id == v.IdApartamento);
            if (apto != null) apto.Disponivel = true;

            _db.Vendas.Remove(v);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
