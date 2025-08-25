using FluentValidation;

namespace Direcional.Api.Dtos
{
    public class VendaCreateDtoValidator : AbstractValidator<VendaCreateDto>
    {
        public VendaCreateDtoValidator()
        {
            RuleFor(x => x.IdCliente).GreaterThan(0);
            RuleFor(x => x.IdApartamento).GreaterThan(0);
            RuleFor(x => x.DataVenda).LessThanOrEqualTo(DateTime.UtcNow.AddDays(1));
            RuleFor(x => x.ValorFinal).GreaterThanOrEqualTo(0);
        }
    }

    public class VendaUpdateDtoValidator : AbstractValidator<VendaUpdateDto>
    {
        public VendaUpdateDtoValidator()
        {
            RuleFor(x => x.IdCliente).GreaterThan(0);
            RuleFor(x => x.IdApartamento).GreaterThan(0);
            RuleFor(x => x.DataVenda).LessThanOrEqualTo(DateTime.UtcNow.AddDays(1));
            RuleFor(x => x.ValorFinal).GreaterThanOrEqualTo(0);
        }
    }
}
