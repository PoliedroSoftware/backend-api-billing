namespace Poliedro.Billing.Domain.Siigo.ResponseModels
{
    public class ApiResponseSiigo
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public List<dynamic> Data { get; set; }
    }
}
