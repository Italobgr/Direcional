namespace Direcional.Api.Domain;

public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Cpf { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
