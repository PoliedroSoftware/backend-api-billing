namespace Poliedro.Billing.Application.Siigo.Contracts.Responses
{
    public class ErrorResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public List<string> Params { get; set; }
        public string Detail { get; set; }
    }
}
