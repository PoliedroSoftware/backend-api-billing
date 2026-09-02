using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
namespace Poliedro.Billing.Application.Billing.Commands.CreateBilling;
public record CreateBillingCommand(int Id, IEnumerable<CreateBillingDTO> Invoices) : IRequest<CreateBillingCommandResult>;