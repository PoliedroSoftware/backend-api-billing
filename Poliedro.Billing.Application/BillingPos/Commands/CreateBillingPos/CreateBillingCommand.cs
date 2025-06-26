using MediatR;
using Poliedro.Billing.Domain.BillingPos;
namespace Poliedro.Billing.Application.BillingPos.Commands.CreateBillingPos;
public record CreateBillingCommand(
List<CreateBilling> invoices,
string token
) : IRequest<List<CreateBilling>>;




