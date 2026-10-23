using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Contracts;
using Poliedro.Billing.Infraestructure.External.Plemsi.Contracts.Responses;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Contracts;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.Resolution.Enums;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Impl.Plemsi;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Selectors.Plemsi;

namespace Poliedro.Billing.Api.Tests;

public sealed class BillingSenderFactoryTests
{
    [Fact]
    public void Resolves_fe_and_pos_senders_when_wrapped_by_decorators()
    {
        IBillingSender fe = new SenderDecorator(new BillingSenderFE(null!, null!, null!));
        IBillingSender pos = new SenderDecorator(new BillingSenderPOS(null!, null!, null!));
        var factory = new BillingSenderRegistry([fe, pos]);

        Assert.Same(fe, factory.Resolve(1, ResolutionType.FE));
        Assert.Same(pos, factory.Resolve(1, ResolutionType.POS));
    }

    [Fact]
    public void Reports_missing_or_unknown_senders_explicitly()
    {
        var factory = new BillingSenderRegistry([]);
        Assert.Throws<InvalidOperationException>(() => factory.Resolve(1, ResolutionType.FE));
        Assert.Throws<InvalidOperationException>(() => factory.Resolve(2, ResolutionType.FE));
        Assert.Throws<Exception>(() => new BillingSenderRegistry([new UnknownSender()]));
    }

    private sealed class SenderDecorator : IBillingSender
    {
        private readonly IBillingSender _innerSender;
        public SenderDecorator(IBillingSender innerSender) => _innerSender = innerSender;
        public Task<List<ApiResponseFERetailPos>> SendAsync(PlemsiInvoiceRequest request, CancellationToken cancellationToken)
            => _innerSender.SendAsync(request, cancellationToken);
    }

    private sealed class UnknownSender : IBillingSender
    {
        public Task<List<ApiResponseFERetailPos>> SendAsync(PlemsiInvoiceRequest request, CancellationToken cancellationToken)
            => Task.FromResult(new List<ApiResponseFERetailPos>());
    }
}
