namespace Direcional.Api.Domain
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefone { get; set; }

        public ICollection<Venda> Vendas { get; set; } = new List<Venda>();
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
