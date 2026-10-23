using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.Resolution.Enums;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.InvoicesPendingWithDetails.DomainService.Impl;

namespace Poliedro.Billing.Api.Tests;

public sealed class PendingInvoiceStrategyFactoryTests
{
    [Theory]
    [InlineData(ResolutionType.FE)]
    [InlineData(ResolutionType.POS)]
    public void Returns_strategy_for_supported_resolution_type(ResolutionType type)
    {
        var fe = new InvoicesPendingWithDetailsFERepository(null!);
        var pos = new InvoicesPendingWithDetailsPOSRepository(null!);
        var factory = new InvoicesPendingWithDetailsStrategyFactory(fe, pos);

        var strategy = factory.GetStrategy(type);

        Assert.Same(type == ResolutionType.FE ? fe : pos, strategy);
    }

    [Fact]
    public void Rejects_unsupported_resolution_type()
    {
        var factory = new InvoicesPendingWithDetailsStrategyFactory(
            new InvoicesPendingWithDetailsFERepository(null!),
            new InvoicesPendingWithDetailsPOSRepository(null!));

        var error = Assert.Throws<NotSupportedException>(() => factory.GetStrategy((ResolutionType)999));

        Assert.Equal("Tipo de resolución no soportado", error.Message);
    }
}
