namespace Poliedro.Billing.Domain.CreditNote.Entity;

public class CreditNoteEntity
{
    public Discrepancy Discrepancy { get; set; }
    public string Resolution { get; set; }
    public string Prefix { get; set; }
    public int Number { get; set; }
    public InvoiceReference InvoiceReference { get; set; }
    public List<ItemElectronicEntity> Items { get; set; }
    public List<TaxTotal> AllTaxTotals { get; set; }
    public Seller Seller { get; set; }
    public string HeadNote { get; set; }
    public string FootNote { get; set; }
    public int TotalToPay { get; set; }
}

public class Discrepancy
{
    public int Code { get; set; }
    public string Description { get; set; }
}

public class InvoiceReference
{
    public string IssueDate { get; set; }
    public string Uuid { get; set; }
    public string Number { get; set; }
}

public class ItemElectronicEntity
{
    public int UnitMeasureId { get; set; }
    public decimal LineExtensionAmount { get; set; }
    public bool FreeOfChargeIndicator { get; set; }
    public List<AllowanceCharge> AllowanceCharges { get; set; }
    public List<TaxTotal> TaxTotals { get; set; }
    public string Description { get; set; }
    public string Notes { get; set; }
    public string Code { get; set; }
    public int TypeItemIdentificationId { get; set; }
    public decimal PriceAmount { get; set; }
    public decimal BaseQuantity { get; set; }
    public decimal InvoicedQuantity { get; set; }
}

public class AllowanceCharge
{
    public bool ChargeIndicator { get; set; }
    public string AllowanceChargeReason { get; set; }
    public decimal MultiplierFactorNumeric { get; set; }
    public decimal Amount { get; set; }
    public decimal BaseAmount { get; set; }
}

public class TaxTotal
{
    public int TaxId { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Percent { get; set; }
    public decimal TaxableAmount { get; set; }
}

public class Seller
{
    public string IdentificationNumber { get; set; }
    public string Dv { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string PostalZoneCode { get; set; }
    public string Email { get; set; }
    public string MerchantRegistration { get; set; }
    public int TypeDocumentIdentificationId { get; set; }
    public int TypeOrganizationId { get; set; }
    public int TypeLiabilityId { get; set; }
    public int MunicipalityId { get; set; }
    public int TypeRegimeId { get; set; }
}









