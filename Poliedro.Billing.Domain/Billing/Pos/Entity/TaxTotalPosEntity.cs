
namespace Poliedro.Billing.Domain.Billing.Pos.Entity;
public class TaxTotalPosEntity
{
    public int tax_id { get; set; }
    public int tax_amount { get; set; }
    public int percent { get; set; }
    public decimal taxable_amount { get; set; }
}
