using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Client.Enums;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Impl;

public class BillingGetInfoClient : IBillingGetInfoClient
{
    public Task<BillingInfoClient> BillingInfoClient(ClientEntity clientEntity, CancellationToken cancellationToken)
    {
        var providerTypeEnum = (ProviderType)clientEntity.ProviderId;

        var billingInfoClient = new BillingInfoClient
        {
            ApiKey = clientEntity.ApiKey,
            TypeResolution = clientEntity.DianResolution.ResolutionType,
            ProviderType = providerTypeEnum,
            Provider = providerTypeEnum.ToString(),
            Prefix = clientEntity.DianResolution.Prefix,
            ExpirationDate = clientEntity.DianResolution.ExpirationDate,
            FinalRange = clientEntity.DianResolution.FinalRange,
            ResolucionNumber = clientEntity.DianResolution.ResolutionNumber,
            Descripcion = clientEntity.DianResolution.Description,
            CurrentlyNumber = clientEntity.DianResolution.CurrentlyNumber,
            MultipleResolution = clientEntity.MultipleResolution,
            ResolutionId = clientEntity.ResolutionId,
            HeadNote = clientEntity.HeadNote,
            FootNote = clientEntity.FootNote

        };

        return Task.FromResult(billingInfoClient);
    }
}
