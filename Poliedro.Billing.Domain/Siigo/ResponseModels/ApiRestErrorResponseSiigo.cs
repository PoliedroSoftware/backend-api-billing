namespace Poliedro.Billing.Domain.Siigo.ResponseModels
{
    public class ApiRestErrorResponseSiigo: ApiRestResponseSiigo
    {
        public int Status {  get; set; }
        public List<ErrorResponse>? Errors { get; set; }
    }
}
