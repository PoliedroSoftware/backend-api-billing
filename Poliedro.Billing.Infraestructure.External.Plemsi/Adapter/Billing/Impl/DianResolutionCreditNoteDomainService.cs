using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Client.Enums;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Impl;

public class DianResolutionCreditNoteDomainService : IBillingGetInfgoClientCreditNote
{
    public Task<BillingInfoClient> BillingInfoClientCreditNote(ClientEntity clientEntity, CancellationToken cancellationToken)
    {
        var providerTypeEnum = (ProviderType)clientEntity.ProviderId;

        var billingInfoClient = new BillingInfoClient
        {
            ApiKey = clientEntity.ApiKey,
            ServerRepository = clientEntity.Server,
            TypeResolution = clientEntity.DianResolutionCreditNote.ResolutionType,
            ProviderType = providerTypeEnum,
            Provider = providerTypeEnum.ToString(),
            Prefix = clientEntity.DianResolutionCreditNote.Prefix,
            ExpirationDate = clientEntity.DianResolutionCreditNote.ExpirationDate,
            FinalRange = clientEntity.DianResolutionCreditNote.FinalRange,
            ResolucionNumber = clientEntity.DianResolutionCreditNote.ResolutionNumber,
            Descripcion = clientEntity.DianResolutionCreditNote.Description,
            CurrentlyNumber = clientEntity.DianResolutionCreditNote.CurrentlyNumber,
            MultipleResolution = clientEntity.MultipleResolution,
            ResolutionId = clientEntity.ResolutionId,
            HeadNote = clientEntity.HeadNote,
            FootNote = clientEntity.FootNote

        };

        return Task.FromResult(billingInfoClient);
    }
}
