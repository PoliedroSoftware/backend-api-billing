using Poliedro.Billing.Domain.Siigo.Models;

namespace Poliedro.Billing.Domain.Siigo.ResponseModels
{
    public class SiigoItemResponse
    {
        public Guid? Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public List<Tax> Taxes { get; set; }
        public decimal Price { get; set; }
        public decimal? Total { get; set; }
    }
}
