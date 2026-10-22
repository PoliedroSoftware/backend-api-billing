using Microsoft.Extensions.Logging.Abstractions;
using Poliedro.Billing.Application.Billing.Commands.CreateBilling;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Api.Tests;

public class BillingGuardTests
{
    [Theory]
    [InlineData(false, true, true, true)]
    [InlineData(true, false, true, true)]
    [InlineData(true, true, false, true)]
    [InlineData(true, true, true, false)]
    public async Task DeniedBillingStopsBeforePreparingOrSending(bool resolutionActive, bool providerActive, bool clientActive, bool licenseActive)
    {
        var authorizer = new Authority(licenseActive);
        // Null factories/senders intentionally fail if the rejection ever reaches preparation or emission.
        var handler = new CreateBillingHandler(new Resolution(resolutionActive), new Provider(providerActive),
            null!, null!, null!, null!, NullLogger<CreateBillingHandler>.Instance,
            new Client(clientActive), authorizer, null!);
        var result = await handler.Handle(new CreateBillingCommand(1, []), default);
        Assert.False(result.Success);
        Assert.Equal(403, result.StatusCode);
        Assert.Equal(resolutionActive && providerActive && clientActive ? 1 : 0, authorizer.Calls);
    }

    private class Resolution(bool active) : IDianResolutionGetByIdService
    {
        public Task<Result<DianResolutionEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult<Result<DianResolutionEntity, Error>>(new DianResolutionEntity { ResolutionId = id, CompanyProviderId = 2, Active = active });
    }
    private class Provider(bool active) : ICompanyProviderGetByIdService
    {
        public Task<CompanyProviderEntity> GetCompanyProviderByIdAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult(new CompanyProviderEntity { CompanyProviderId = id, CompanyId = 3, Active = active ? 1 : 0, ApiKey = "unused" });
    }
    private class Client(bool active) : IClientGetByIdService
    {
        public Task<Result<ClientEntity, Error>> GetByIdAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult<Result<ClientEntity, Error>>(new ClientEntity { CompanyId = id, Active = active });
    }
    private class Authority(bool allowed) : IBillingLicenseAuthorizer
    {
        public int Calls { get; private set; }
        public Task<BillingLicenseDecision> AuthorizeAsync(int companyId, int companyProviderId, CancellationToken cancellationToken)
        {
            Calls++;
            return Task.FromResult(new BillingLicenseDecision(allowed, allowed ? "active" : "inactive"));
        }
    }
}
