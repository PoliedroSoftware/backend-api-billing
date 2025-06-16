namespace Poliedro.Billing.Domain.InvoiceDetailElectronic.Entities;

public class InvoiceWithDetailElectronic
{
    public int Id { get; set; }
    public int Transaccion { get; set; }
    public int Code { get; set; }
    public int TypeItemIdentification { get; set; }
    public string? Description { get; set; }
    public int UnitMeasureId { get; set; }
    public decimal BaseQuantity { get; set; }
    public decimal InvoicedQuantity { get; set; }
    public decimal? PriceAmount { get; set; }
    public decimal? LineExtensionAmount { get; set; }
    public double? Percent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? Invoice { get; set; }
    public DateTime? DateRegister { get; set; }
    public string? Verify { get; set; }


}
