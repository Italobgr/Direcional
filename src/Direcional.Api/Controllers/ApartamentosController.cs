using Direcional.Api.Domain;
using Direcional.Api.Dtos;
using Direcional.Api.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Direcional.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApartamentosController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ApartamentosController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApartamentoReadDto>>> GetAll()
            => Ok(await _db.Apartamentos.AsNoTracking()
                .Select(a => new ApartamentoReadDto(a.Id, a.Endereco, a.NumeroQuartos, a.Valor, a.Disponivel))
                .ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApartamentoReadDto>> GetById(int id)
        {
            var a = await _db.Apartamentos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (a == null) return NotFound();
            return Ok(new ApartamentoReadDto(a.Id, a.Endereco, a.NumeroQuartos, a.Valor, a.Disponivel));
        }

        [HttpPost]
        public async Task<ActionResult<ApartamentoReadDto>> Create(ApartamentoCreateDto dto)
        {
            var a = new Apartamento { Endereco = dto.Endereco, NumeroQuartos = dto.NumeroQuartos, Valor = dto.Valor, Disponivel = dto.Disponivel };
            _db.Apartamentos.Add(a);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = a.Id }, new ApartamentoReadDto(a.Id, a.Endereco, a.NumeroQuartos, a.Valor, a.Disponivel));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, ApartamentoUpdateDto dto)
        {
            var a = await _db.Apartamentos.FirstOrDefaultAsync(x => x.Id == id);
            if (a == null) return NotFound();

            a.Endereco = dto.Endereco; a.NumeroQuartos = dto.NumeroQuartos; a.Valor = dto.Valor; a.Disponivel = dto.Disponivel;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var a = await _db.Apartamentos.FirstOrDefaultAsync(x => x.Id == id);
            if (a == null) return NotFound();

            bool temVenda = await _db.Vendas.AnyAsync(v => v.IdApartamento == id);
            bool reservaAtiva = await _db.Reservas.AnyAsync(r => r.IdApartamento == id && r.Status == ReservaStatus.Ativa);

            if (temVenda || reservaAtiva)
                return Conflict("Apartamento possui venda ou reserva ativa e nao pode ser excluido.");

            _db.Apartamentos.Remove(a);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
