
namespace Poliedro.Billing.Application.NotifyResolution.Exceptions
{
    public class NotifyResolutionNotFoundException : Exception
    {
        public NotifyResolutionNotFoundException(string apiKey)
            : base($"No notify resolution found for API key: {apiKey}") { }
    }
}