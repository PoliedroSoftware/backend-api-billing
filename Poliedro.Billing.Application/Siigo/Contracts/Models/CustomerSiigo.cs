namespace Poliedro.Billing.Application.Siigo.Contracts.Models
{
    public class CustomerSiigo
    {
        public Guid? Id { get; set; }
        public string Identification { get; set; }
        public string? BranchOffice { get; set; }
    }
}
