# Billing provider boundary

## Request and provider selection

`POST /api/v1/billing/{resolutionId}` remains provider-neutral. The caller sends the invoice data and resolution identifier; it does not send a provider name or provider id. The application loads the resolution, follows `company_provider_id`, and selects the configured `provider_id` from that company-provider record. A client therefore cannot route an invoice to a different provider by changing request JSON.

Provider selection happens after the client, resolution, and license checks. The selected provider is resolved through `IBillingProviderAdapterFactory`. The provider adapter owns both provider-specific numbering retrieval and transmission, while Application prepares the internal FE/POS document from the Billing request.

## Adapter responsibilities

The Application contract is `IBillingProviderAdapter`. Provider wire DTOs, transport requests, response mapping, configuration validation, and last-number retrieval belong to the corresponding Infrastructure project. The Plemsi JSON DTOs and AutoMapper profile live in `Poliedro.Billing.Infraestructure.External.Plemsi`; Application preparation returns its internal typed FE/POS document instead. The handler receives a normalized `BillingProviderResponse` and does not consume Plemsi response DTOs or `object` payloads.

Recording a confirmed emission is also expressed as Application ports (`IBillingEmissionResultRecorder` and `IInvoiceEmissionStore`). Their MySQL implementations live in `Poliedro.Billing.Infraestructure.Persistence.Mysql`, so the billing use case no longer invokes a provider-named response persistence service. Plemsi's tax identifiers are translated in its adapter mapping profile; the prepared Domain item retains the domain `TaxType` identifier.

A new provider implements `IBillingProviderAdapter`: it declares its configured provider id/key, validates its credentials, supplies its numbering strategy for supported document types, translates the internal prepared model to its wire payload, and maps its result to `BillingProviderResponse`. The provider module must register that adapter and the API composition root must include the module. No provider selector belongs in the public request. A provider change is made in server-side company-provider/resolution configuration, then the same endpoint flow resolves that adapter.

## Current scope and remaining modularization

Only Plemsi currently implements the new adapter contract for FE/POS. Siigo and TNS still have separate provider-specific Application use cases and endpoints; their contracts and gateway ports now live in Application, while their Domain request/response models and service interfaces have been removed. Provider wire responses and Plemsi credit-note payloads live in the Plemsi integration project. Domain uses only generic provider IDs/configuration and has no vendor names, vendor contracts, or project reference to Infrastructure. The API host still references and registers the integration projects in its composition root; this is ordinary startup wiring, but means adding/removing a provider requires updating the host registration and project references.

The database already stores the provider relationship in `dian_resolution_billing.company_provider_id` and `company_provider.provider_id`; this change does not require a new table. Before switching a live client, the target company-provider configuration, credentials, license, resolution sequence, and provider-specific support for the document type must be present. This implementation establishes the extension point; it does not add Carvajal or make an unregistered provider operational.
