namespace Poliedro.Billing.Domain.FERetail.Entity
{
    public class ApiResponseFERetailPos
    {
        public int Code { get; init; }
        public bool Success { get; init; }
        public string Info { get; init; }
        public apiDataPos Data { get; init; }
    }
}
