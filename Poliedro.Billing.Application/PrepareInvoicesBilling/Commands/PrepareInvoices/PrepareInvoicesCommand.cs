using MediatR;
namespace Poliedro.Billing.Application.PrepareInvoicesBilling.Commands.PrepareInvoices;
public record PrepareInvoicesCommand(

 List<object> invoices,
 string token
    
): IRequest<List<object>>;