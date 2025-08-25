using FluentValidation;

namespace Direcional.Api.Dtos
{
    public class ClienteCreateDtoValidator : AbstractValidator<ClienteCreateDto>
    {
        public ClienteCreateDtoValidator()
        {
            RuleFor(x => x.Nome).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        }
    }

    public class ClienteUpdateDtoValidator : AbstractValidator<ClienteUpdateDto>
    {
        public ClienteUpdateDtoValidator()
        {
            RuleFor(x => x.Nome).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        }
    }
}
