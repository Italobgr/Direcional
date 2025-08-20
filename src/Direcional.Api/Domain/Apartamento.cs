namespace Direcional.Api.Domain;

public class Apartamento
{
    public int Id { get; set; }
    public string Bloco { get; set; } = null!;
    public string Numero { get; set; } = null!;
    public decimal Preco { get; set; }
    public StatusApartamento Status { get; set; } = StatusApartamento.Disponivel;
}
