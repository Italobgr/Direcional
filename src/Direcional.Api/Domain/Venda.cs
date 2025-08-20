namespace Direcional.Api.Domain;

public class Venda
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int ApartamentoId { get; set; }
    public DateTime DataVenda { get; set; } = DateTime.UtcNow;
    public decimal ValorEntrada { get; set; }

    public Cliente? Cliente { get; set; }
    public Apartamento? Apartamento { get; set; }
}
