using FluentValidation;

namespace SaasDental.Application.Features.Tenants.Commands.CreateTenant;

public class CreateTenantValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la clínica es requerido.")
            .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("La dirección es requerida.")
            .MaximumLength(300).WithMessage("La dirección no puede exceder los 300 caracteres.");
    }
}
