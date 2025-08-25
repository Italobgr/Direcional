namespace Direcional.Api.Dtos
{
    public record ApartamentoCreateDto(string Endereco, int NumeroQuartos, decimal Valor, bool Disponivel);
    public record ApartamentoUpdateDto(string Endereco, int NumeroQuartos, decimal Valor, bool Disponivel);
    public record ApartamentoReadDto(int Id, string Endereco, int NumeroQuartos, decimal Valor, bool Disponivel);
}
