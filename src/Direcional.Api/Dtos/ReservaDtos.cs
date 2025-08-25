using Direcional.Api.Domain;

namespace Direcional.Api.Dtos
{
    public record ReservaCreateDto(int IdCliente, int IdApartamento, DateTime DataReserva, DateTime? Validade);
    public record ReservaUpdateDto(int IdCliente, int IdApartamento, DateTime DataReserva, DateTime? Validade, ReservaStatus Status);
    public record ReservaReadDto(int Id, int IdCliente, int IdApartamento, DateTime DataReserva, DateTime? Validade, ReservaStatus Status);
}
