using FluentValidation;

namespace Direcional.Api.Dtos
{
    public class ApartamentoCreateDtoValidator : AbstractValidator<ApartamentoCreateDto>
    {
        public ApartamentoCreateDtoValidator()
        {
            RuleFor(x => x.Endereco).NotEmpty().MaximumLength(250);
            RuleFor(x => x.NumeroQuartos).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Valor).GreaterThanOrEqualTo(0);
        }
    }

    public class ApartamentoUpdateDtoValidator : AbstractValidator<ApartamentoUpdateDto>
    {
        public ApartamentoUpdateDtoValidator()
        {
            RuleFor(x => x.Endereco).NotEmpty().MaximumLength(250);
            RuleFor(x => x.NumeroQuartos).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Valor).GreaterThanOrEqualTo(0);
        }
    }
}
