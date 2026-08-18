using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
namespace Poliedro.Billing.Application.Billing.Commands.CreateBilling;
public record CreateBillingCommand(int Id, IEnumerable<CreateBillingInputDTO> Invoices) : IRequest<CreateBillingCommandResult>;




