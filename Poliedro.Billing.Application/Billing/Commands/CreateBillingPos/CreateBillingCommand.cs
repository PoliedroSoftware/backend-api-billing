using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Domain.BillingPos;
namespace Poliedro.Billing.Application.BillingPos.Commands.CreateBillingPos;
public record CreateBillingCommand(
IEnumerable<CreateBilling> Invoices,
string ApiKey
) : IRequest<IEnumerable<CreateBillingDto>>;




