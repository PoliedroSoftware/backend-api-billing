using Poliedro.Billing.Application.DianResolution.Dtos;
using Poliedro.Billing.Application.Server.Dtos;
using Poliedro.Billing.Domain.Client.Enums;

namespace Poliedro.Billing.Application.Client.Dtos
{
    public class ClientDto
    {
        public int ClientBillingElectronicId { get; set; } = default!;
        public string Name { get; set; } = string.Empty;
        public ProviderType ProviderId { get; set; } = default!;
        public bool Active { get; set; } = default!;
        public DianResolutionDto DianResolution { get; set; } = default!;
        public ServerDto Server { get; set; } = default!;
        public int Iterations { get; set; } = default!;
        public DateTime Date { get; set; } = default!;
        public string ApiKey { get; set; } = string.Empty;
    }
}
