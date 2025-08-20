namespace Direcional.Api.Domain;

public class Reserva
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int ApartamentoId { get; set; }
    public DateTime DataReserva { get; set; } = DateTime.UtcNow;
    public StatusReserva Status { get; set; } = StatusReserva.Ativa;

    public Cliente? Cliente { get; set; }
    public Apartamento? Apartamento { get; set; }
}