using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Domain.PrepareInvoicesBilling.Ports;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.PrepareInvoicesBilling.DomainService.Impl;

public class PrepareInvoicesBillingFactory : IPrepareInvoicesBillingFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PrepareInvoicesBillingFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPrepareInvoicesBillingStrategy GetProcessor(string clientType)
    {
        return clientType switch
        {
            "FE" => _serviceProvider.GetRequiredService<PrepareInvoicesBillingFERepository>(),
            "POS" => _serviceProvider.GetRequiredService<PrepareInvoicesBillingPOSRepository>(),
            _ => throw new ArgumentException($"Unknown client type: {clientType}")
        };
    }
}
