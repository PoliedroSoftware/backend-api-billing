namespace Poliedro.Billing.Domain.FERetail.Entity;

public class InvoiceEntity
 {

    public int Id { get; set; }
    public string identication { get; set; }
    public string contact_name { get; set; }
    public string name { get; set; }
    public string email { get; set; }
    public string mobile { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string country { get; set; }
    public string invoice { get; set; }
    public string payment_status { get; set; }
    public string transaction_date { get; set; }
    public double allowanceTotal { get; set; }
    public double invoiceBaseTotal { get; set; }
    public double invoiceTaxExclusiveTotal { get; set; }
    public double invoiceTaxInclusiveTotal { get; set; }
    public double totalToPay { get; set; }
    public string created_by_name { get; set; }
    public double percent { get; set; }
    public double invoiceTax { get; set; }
    public int sendDian { get; set; }

}

