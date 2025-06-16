namespace Poliedro.Billing.Domain.TNS.ResponseModels
{
    public class ApiResponseTNS
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public List<ApiDataResponseTNS> Data { get; set; }
    }
}
