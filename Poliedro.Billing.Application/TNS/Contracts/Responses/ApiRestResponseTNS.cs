namespace Poliedro.Billing.Application.TNS.Contracts.Responses
{
    public class ApiRestResponseTNS
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public ApiDataResponseTNS Data { get; set; }
    }
}
