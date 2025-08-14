using Poliedro.Billing.Domain.Common.Enum;
namespace Poliedro.Billing.Domain.Billing.Ports;
public interface IBillingValidateScript
{
    Task<DocumentType> ValidateScriptAsync(string script, CancellationToken cancellationToken);
}
