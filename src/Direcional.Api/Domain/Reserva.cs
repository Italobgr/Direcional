namespace Direcional.Api.Domain
{
    public class Reserva
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public int IdApartamento { get; set; }
        public DateTime DataReserva { get; set; }
        public DateTime? Validade { get; set; }     
        public ReservaStatus Status { get; set; } = ReservaStatus.Ativa;

        public Cliente? Cliente { get; set; }
        public Apartamento? Apartamento { get; set; }
    }
}
