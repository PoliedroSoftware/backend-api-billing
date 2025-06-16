using Poliedro.Billing.Domain.Siigo.Models;

namespace Poliedro.Billing.Domain.Siigo.ResponseModels
{
    public class ApiRestSuccessResponseSiigo: ApiRestResponseSiigo
    {
        public Guid Id { get; set; }
        public Document Document { get; set; }
        public string Prefix { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public CustomerSiigo Customer { get; set; }
        public int Seller { get; set; }
        public decimal Total { get; set; }
        public decimal Balance { get; set; }
        public string Observations { get; set; }
        public List<SiigoItemResponse> Items { get; set; }
        public List<SiigoPayment> Payments { get; set; }
        public StampResponse Stamp { get; set; }
        public MailResponse Mail { get; set; }
        public Metadata Metadata { get; set; }
        public string PublicUrl { get; set; }
    }
}
