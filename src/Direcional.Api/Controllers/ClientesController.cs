using Direcional.Api.Domain;
using Direcional.Api.Dtos;
using Direcional.Api.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Direcional.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ClientesController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteReadDto>>> GetAll()
            => Ok(await _db.Clientes.AsNoTracking()
                .Select(c => new ClienteReadDto(c.Id, c.Nome, c.Email, c.Telefone))
                .ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClienteReadDto>> Get(int id)
        {
            var c = await _db.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return NotFound();
            return Ok(new ClienteReadDto(c.Id, c.Nome, c.Email, c.Telefone));
        }

        [HttpPost]
        public async Task<ActionResult<ClienteReadDto>> Create(ClienteCreateDto dto)
        {
            var c = new Cliente { Nome = dto.Nome, Email = dto.Email, Telefone = dto.Telefone };
            _db.Clientes.Add(c);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = c.Id }, new ClienteReadDto(c.Id, c.Nome, c.Email, c.Telefone));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, ClienteUpdateDto dto)
        {
            var c = await _db.Clientes.FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return NotFound();

            c.Nome = dto.Nome; c.Email = dto.Email; c.Telefone = dto.Telefone;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _db.Clientes.FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return NotFound();

            bool temVendas = await _db.Vendas.AnyAsync(v => v.IdCliente == id);
            bool temReservasAtivas = await _db.Reservas.AnyAsync(r => r.IdCliente == id && r.Status == Domain.ReservaStatus.Ativa);

            if (temVendas || temReservasAtivas)
                return Conflict("Cliente possui vendas ou reservas ativas.");

            _db.Clientes.Remove(c);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
