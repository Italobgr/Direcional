namespace Direcional.Api.Dtos
{
    public record ClienteCreateDto(string Nome, string? Email, string? Telefone);
    public record ClienteUpdateDto(string Nome, string? Email, string? Telefone);
    public record ClienteReadDto(int Id, string Nome, string? Email, string? Telefone);
}
