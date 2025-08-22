using Direcional.Api.Dtos;
using Direcional.Api.Dtos.Validators;
using FluentAssertions;
using Xunit;

namespace Direcional.Tests.Unit;

public class ClienteValidatorsTests
{




    [Fact]
    public void ClienteCreate_Deve_Falhar_Quando_Dados_Invalidos()
    {

        // Testa se o validador falha com dados inválidos
        var validator = new ClienteCreateValidator();
        var dto = new ClienteCreateDto("", "123", "email_xpto");

        var result = validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }


    [Fact]
    public void ClienteCreate_Deve_Passar_Quando_Dados_Válidos()
    {
        //  dados válidos
        var validator = new ClienteCreateValidator();
        var dto = new ClienteCreateDto("Maria", "12345678901", "maria@ex.com"); //pegar do -> set

        var result = validator.Validate(dto);
        result.IsValid.Should().BeTrue();
    }
}
