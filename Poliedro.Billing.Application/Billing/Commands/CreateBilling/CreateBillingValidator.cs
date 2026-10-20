using FluentValidation;
using Poliedro.Billing.Application.Billing.Dtos;

namespace Poliedro.Billing.Application.Billing.Commands.CreateBilling;

public class CreateBillingValidator : AbstractValidator<CreateBillingCommand>
{
    public CreateBillingValidator()
    {
        RuleFor(command => command.Invoices)
            .NotNull().WithMessage("El campo 'data' del cuerpo de la petición es obligatorio.")
            .NotEmpty().WithMessage("Debe incluir al menos una factura en 'data'.");

        RuleForEach(command => command.Invoices)
            .SetValidator(new CreateBillingInvoiceValidator());
    }
}

public class CreateBillingInvoiceValidator : AbstractValidator<CreateBillingDTO>
{
    public CreateBillingInvoiceValidator()
    {
        RuleFor(invoice => invoice.Number)
            .NotEmpty().WithMessage("La factura debe tener un número (number).");
    }
}