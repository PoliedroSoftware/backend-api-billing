using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
namespace Poliedro.Billing.Application.Billing.Commands.CreateBilling;
public record CreateBillingCommand(
IEnumerable<Domain.Billing.CreateBilling> Invoices,
string ApiKey
) : IRequest<IEnumerable<CreateBillingDTO>>;




