namespace Direcional.Api.Domain
{
    public class Venda
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public int IdApartamento { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal ValorFinal { get; set; }

        public Cliente? Cliente { get; set; }
        public Apartamento? Apartamento { get; set; }
    }
}
