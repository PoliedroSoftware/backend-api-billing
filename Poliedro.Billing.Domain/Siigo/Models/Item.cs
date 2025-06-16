namespace Poliedro.Billing.Domain.Siigo.Models
{
    public class Item
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public List<Tax> Taxes { get; set; }
        public decimal Price { get; set; }
        public decimal? Total { get; set; }
    }
}
