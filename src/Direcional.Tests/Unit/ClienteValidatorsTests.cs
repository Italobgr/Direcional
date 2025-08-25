using Direcional.Api.Dtos;
//using Direcional.Api.Dtos; // onde estão os validators
using FluentValidation.TestHelper;
using Xunit;

namespace Direcional.Tests.Unit;

public class ClienteValidatorsTests
{
    [Fact]
    public void Nome_Obrigatorio()
    {
        var v = new ClienteCreateDtoValidator();
        var dto = new ClienteCreateDto("", "a@a.com", "123");
        var res = v.TestValidate(dto);
        res.ShouldHaveValidationErrorFor(x => x.Nome);
    }
}
