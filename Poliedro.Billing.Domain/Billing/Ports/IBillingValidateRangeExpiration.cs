namespace Poliedro.Billing.Domain.Billing.Ports;
public interface IBillingValidateRangeExpiration
{
    Task ValidateNumerationDateAsync(int Invoice ,DateTime ExpirationDate, int FinalRange);
}
