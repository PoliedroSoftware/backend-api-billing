using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Enum;
namespace Poliedro.Billing.Domain.Common.Methods.Billing.Prepare.Plemsi.ElectronicBilling;
public class ValidateScriptBilling : IBillingValidateScript
{
    public async Task<DocumentType> ValidateScriptAsync(string script, CancellationToken cancellationToken)
    {
        return await Task.FromResult(script.Contains('-') ? DocumentType.NIT : DocumentType.CC);

    }
}
