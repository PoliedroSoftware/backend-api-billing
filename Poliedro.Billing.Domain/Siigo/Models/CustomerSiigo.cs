namespace Poliedro.Billing.Domain.Siigo.Models
{
    public class CustomerSiigo
    {
        public Guid? Id { get; set; }
        public string Identification { get; set; }
        public string? BranchOffice { get; set; }
    }
}
