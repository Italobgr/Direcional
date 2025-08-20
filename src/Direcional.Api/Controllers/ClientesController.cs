using Direcional.Api.Domain;
using Direcional.Api.Dtos;
using Direcional.Api.Infra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Direcional.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> Get() =>
        await db.Clientes.AsNoTracking().ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Cliente>> GetById(int id)
    {
        var c = await db.Clientes.FindAsync(id);
        return c is null ? NotFound() : Ok(c);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ClienteCreateDto dto)
    {
        var c = new Cliente { Nome = dto.Nome, Cpf = dto.Cpf, Email = dto.Email };
        db.Clientes.Add(c);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = c.Id }, c);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ClienteUpdateDto dto)
    {
        var c = await db.Clientes.FindAsync(id);
        if (c is null) return NotFound();
        c.Nome = dto.Nome;
        c.Email = dto.Email;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await db.Clientes.FindAsync(id);
        if (c is null) return NotFound();
        db.Clientes.Remove(c);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
