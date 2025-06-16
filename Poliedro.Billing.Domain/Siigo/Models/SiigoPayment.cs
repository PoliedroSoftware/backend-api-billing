namespace Poliedro.Billing.Domain.Siigo.Models
{
    public class SiigoPayment
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Name { get; set; }
    }
}
