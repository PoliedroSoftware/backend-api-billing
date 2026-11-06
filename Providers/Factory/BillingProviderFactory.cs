using Poliedro.Billing.Application.Billing.Ports.Providers;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poliedro.Billing.Infraestructure.External.Provider.Factory
{
    public class BillingProviderFactory : IBillingProviderFactory
    {
        public IBillingProvider Resolve(ProviderType providerType, ResolutionType resolutionType)
        {
            throw new NotImplementedException();
        }
    }
}
