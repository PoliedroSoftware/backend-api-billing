using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Impl.Plemsi.TypeTax;

namespace Poliedro.Billing.Api.Tests;

public sealed class PlemsiTaxMappingTests
{
    public static IEnumerable<object[]> KnownMappings => PlemsiTaxMapping.Ids
        .Select(mapping => new object[] { (int)mapping.Key, mapping.Value });

    [Theory]
    [MemberData(nameof(KnownMappings))]
    public void Every_supported_internal_tax_has_the_expected_plemsi_identifier(int internalId, int plemsiId)
        => Assert.Equal(plemsiId, PlemsiTaxMapping.ToProviderId(internalId));

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(24)]
    [InlineData(999)]
    public void Rejects_internal_tax_ids_without_provider_mapping(int internalId)
    {
        var error = Assert.Throws<ArgumentException>(() => PlemsiTaxMapping.ToProviderId(internalId));
        Assert.Contains(internalId.ToString(), error.Message);
    }
}
