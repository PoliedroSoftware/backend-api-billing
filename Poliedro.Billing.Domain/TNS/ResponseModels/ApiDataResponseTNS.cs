namespace Poliedro.Billing.Domain.TNS.ResponseModels
{
    public class ApiDataResponseTNS
    {
        public bool Success { get; set; }
        public string Response { get; set; }
        public string PedidoID { get; set; }
        public string Consecutivo { get; set; }
        public bool Asentado { get; set; }
        public string MensajeAsentado { get; set; }
        public string EnlacePago { get; set; }
    }

}
