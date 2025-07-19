namespace Poliedro.Billing.Domain.Billing.Ports;

public interface IGetSenderBillingResolver
{
   Task<object> ResolveSenderAsync(string Provider, string TypeResolution);
}
