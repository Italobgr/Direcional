namespace Direcional.Api.Dtos;

public record ClienteCreateDto(string Nome, string Cpf, string Email);
public record ClienteUpdateDto(string Nome, string Email);