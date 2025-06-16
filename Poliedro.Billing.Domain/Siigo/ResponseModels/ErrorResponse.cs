namespace Poliedro.Billing.Domain.Siigo.ResponseModels
{
    public class ErrorResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public List<string> Params { get; set; }
        public string Detail { get; set; }
    }
}
