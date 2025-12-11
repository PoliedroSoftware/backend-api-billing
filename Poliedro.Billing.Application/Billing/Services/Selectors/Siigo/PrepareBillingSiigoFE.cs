
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Siigo.Ports;

namespace Poliedro.Billing.Application.Billing.Services.Selectors.Siigo;

public class PrepareBillingSiigoFE(
    IAuthSiigo _authSiigo
    ) : ICreateBilling
{
    public Task<IEnumerable<(CreateBilling Billing, object Output)>> CreateInvoicesAsync(IEnumerable<CreateBilling> invoices, BillingInfoClient clientInfo, CancellationToken cancellationToken)
    {
        //Auth Siigo Token Bearer
        //_authSiigo.GetAuthSiigoAsync(clientInfo.Email!, clientInfo.Password!, cancellationToken);


        throw new NotImplementedException();
    }
}
