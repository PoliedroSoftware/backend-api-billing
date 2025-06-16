namespace Poliedro.Billing.Domain.Siigo.Models
{
    public class Invoice
    {
        public Document Document { get; set; }
        public string Date { get; set; }
        public CustomerSiigo Customer { get; set; }
        public int Seller { get; set; }
        public List<Item> Items { get; set; }
        public Stamp Stamp { get; set; }
        public Mail Mail { get; set; }
        public string Observations { get; set; }
        public List<SiigoPayment> Payments { get; set; }
    }
}
