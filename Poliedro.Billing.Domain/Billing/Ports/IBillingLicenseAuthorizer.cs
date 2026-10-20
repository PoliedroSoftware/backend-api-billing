namespace Poliedro.Billing.Domain.Billing.Ports;

public interface IBillingLicenseAuthorizer
{
    Task<BillingLicenseDecision> AuthorizeAsync(int companyId, int companyProviderId, CancellationToken cancellationToken);
}

public record BillingLicenseDecision(bool Allowed, string Code);
