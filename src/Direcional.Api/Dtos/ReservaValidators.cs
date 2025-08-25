using FluentValidation;

namespace Direcional.Api.Dtos
{
    public class ReservaCreateDtoValidator : AbstractValidator<ReservaCreateDto>
    {
        public ReservaCreateDtoValidator()
        {
            RuleFor(x => x.IdCliente).GreaterThan(0);
            RuleFor(x => x.IdApartamento).GreaterThan(0);
            RuleFor(x => x.DataReserva).LessThanOrEqualTo(DateTime.UtcNow.AddDays(1));
        }
    }

    public class ReservaUpdateDtoValidator : AbstractValidator<ReservaUpdateDto>
    {
        public ReservaUpdateDtoValidator()
        {
            RuleFor(x => x.IdCliente).GreaterThan(0);
            RuleFor(x => x.IdApartamento).GreaterThan(0);
            RuleFor(x => x.DataReserva).LessThanOrEqualTo(DateTime.UtcNow.AddDays(1));
        }
    }
}
