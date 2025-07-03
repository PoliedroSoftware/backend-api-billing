namespace Poliedro.Billing.Domain.Billing.Ports;

public interface IBillingSenderFactory
{
    IBillingSender GetSender(string typeResolution, string providerType);
}