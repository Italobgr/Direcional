namespace Direcional.Api.Domain
{
    public class Apartamento
    {
        public int Id { get; set; }
        public string Endereco { get; set; } = string.Empty;
        public int NumeroQuartos { get; set; }
        public decimal Valor { get; set; }
        public bool Disponivel { get; set; } = true;

        public ICollection<Venda> Vendas { get; set; } = new List<Venda>();
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
