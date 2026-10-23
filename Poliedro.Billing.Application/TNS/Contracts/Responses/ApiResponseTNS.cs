namespace Poliedro.Billing.Application.TNS.Contracts.Responses
{
    public class ApiResponseTNS
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public List<ApiDataResponseTNS> Data { get; set; }
    }
}
