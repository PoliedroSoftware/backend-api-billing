
namespace Poliedro.Billing.Domain.InvoicesPendingWithDetails.Entities;
public class DetailsInvoicePOSPendingEntity
{
    public int Id { get; set; }
    public string Description { get; set; }
    public string Code { get; set; }
    public decimal BaseQuantity { get; set; }
    public decimal InvoicedQuantity { get; set; }
    public decimal PriceAmount { get; set; }
    public decimal Subtotal { get; set; }
}
