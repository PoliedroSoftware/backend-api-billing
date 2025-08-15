
namespace Poliedro.Billing.Domain.Billing.Pos.Entity;
public class InvoicePosEntity
{
    public int number { get; set; }
    public required string date { get; set; }
    public required string time { get; set; }
    public SoftwareManufacturerPosEntity softwareManufacturer { get; set; }
    public required string sendToEmail { get; set; }
    public required string resolution { get; set; }
    public required string prefix { get; set; }
    public string head_note { get; set; }
    public string foot_note { get; set; }
    public  PayPointInfoPosEntity payPointInfo { get; set; }
    public PaymentPosEntity payment { get; set; }
    public required string invoiceBaseTotal { get; set; }
    public required string invoiceTaxExclusiveTotal { get; set; }
    public string invoiceTaxInclusiveTotal { get; set; }
    public string totalToPay { get; set; }
    public List<TaxTotalPosEntity> allTaxTotals { get; set; }
    public List<ItemFERetailEntity> items { get; set; }
    public List<TaxItemPosEntity> tax_totals { get; set; }




}
