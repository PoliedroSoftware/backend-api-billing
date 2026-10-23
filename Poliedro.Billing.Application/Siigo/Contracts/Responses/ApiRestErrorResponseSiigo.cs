namespace Poliedro.Billing.Application.Siigo.Contracts.Responses
{
    public class ApiRestErrorResponseSiigo: ApiRestResponseSiigo
    {
        public int Status {  get; set; }
        public List<ErrorResponse>? Errors { get; set; }
    }
}
