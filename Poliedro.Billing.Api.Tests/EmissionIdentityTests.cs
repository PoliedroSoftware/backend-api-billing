using System.Reflection;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Poliedro.Billing.Application.Billing.Commands.CreateBilling;
using Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Api.Tests;

public class EmissionIdentityTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task AcceptedInvoiceReturnsAssignedNumberEvenIfPersistenceFails(bool persistenceFails)
    {
        var invoice = new Domain.Billing.CreateBilling { Number = "odoo-source-42", Numeration = "101" };
        IEnumerable<(Domain.Billing.CreateBilling Billing, object Output)> prepared = [(invoice, new object())];
        var processor = Stub<ICreateBilling>(() => Task.FromResult(prepared));
        var sender = Stub<IBillingSender>(() => Task.FromResult(new List<ApiResponseFERetailPos>
        {
            new() { Success = true, Data = new() { Cude = "synthetic-cufe-101" } }
        }));
        var handler = new CreateBillingHandler(
            Stub<IDianResolutionGetByIdService>(() => Task.FromResult<Result<DianResolutionEntity, Error>>(
                new DianResolutionEntity { Active = true, ResolutionId = 20, CompanyProviderId = 28, Prefix = "QA" })),
            Stub<ICompanyProviderGetByIdService>(() => Task.FromResult(new CompanyProviderEntity
                { Active = 1, CompanyId = 28, CompanyProviderId = 28, ApiKey = "synthetic" })),
            Stub<IGetProcessorBilling>(() => Task.FromResult(processor)),
            Stub<IBillingSenderFactory>(() => sender),
            Stub<IBillingResponseApi>(() => persistenceFails ? Task.FromException(new Exception("synthetic failure")) : Task.CompletedTask),
            Stub<IMapper>(() => new[] { invoice }), NullLogger<CreateBillingHandler>.Instance,
            Stub<IClientGetByIdService>(() => Task.FromResult<Result<ClientEntity, Error>>(new ClientEntity { Active = true })),
            Stub<IBillingLicenseAuthorizer>(() => Task.FromResult(new BillingLicenseDecision(true, "active"))), null!);
        var result = await handler.Handle(new CreateBillingCommand(20, []), default);
        var emitted = Assert.Single(result.Results!);
        Assert.True(emitted.Status);
        var data = JsonSerializer.SerializeToElement(emitted.Data);
        Assert.Equal("101", data.GetProperty("Number").GetString());
        Assert.Equal("synthetic-cufe-101", data.GetProperty("Cufe").GetString());
        Assert.Equal(!persistenceFails, data.GetProperty("Persisted").GetBoolean());
    }

    private static T Stub<T>(Func<object> call) where T : class
    {
        var proxy = DispatchProxy.Create<T, CallbackProxy>();
        ((CallbackProxy)(object)proxy).Call = call;
        return proxy;
    }
    public class CallbackProxy : DispatchProxy
    {
        public Func<object> Call = null!;
        protected override object Invoke(MethodInfo? method, object?[]? args) => Call();
    }
}
