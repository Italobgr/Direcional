namespace Direcional.Api.Dtos
{
    public record VendaCreateDto(int IdCliente, int IdApartamento, DateTime DataVenda, decimal ValorFinal);
    public record VendaUpdateDto(int IdCliente, int IdApartamento, DateTime DataVenda, decimal ValorFinal);
    public record VendaReadDto(int Id, int IdCliente, int IdApartamento, DateTime DataVenda, decimal ValorFinal);
}
