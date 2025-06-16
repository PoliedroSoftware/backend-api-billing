namespace Poliedro.Billing.Domain.TNS.Models
{
    public class DetailDiscount
    {
        public string CodigoConcepto { get; set; }
        public string Valor { get; set; }
        public string BaseRetencion { get; set; }
        public string PorcentajeRetencion { get; set; }
        public string PorcentajeIva { get; set; }
        public string CodigoCentroCostos { get; set; }
        public string CodigoArea { get; set; }
    }
}
