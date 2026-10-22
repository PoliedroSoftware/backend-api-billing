namespace Poliedro.Billing.Domain.Billing;

/// <summary>
/// Se lanza cuando una factura tiene ítems sin precio unitario ni precio de referencia.
/// La DIAN rechaza esas líneas (FAW03: precio referencial no informado), por lo que la
/// factura no debe enviarse al proveedor hasta corregir el precio en el origen.
/// </summary>
public sealed class ItemPriceNotInformedException : Exception
{
    public ItemPriceNotInformedException(IReadOnlyList<string> items)
        : base(
            $"Ítems sin precio informado: {string.Join("; ", items)}. " +
            "Corrija el precio en el origen y reintente.")
    {
        Items = items;
    }

    public IReadOnlyList<string> Items { get; }
}
