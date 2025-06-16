namespace Poliedro.Billing.Domain.TNS.Models
{
    public class PaymentMethodDetail
    {
        public string CodigoFormaPago { get; set; }
        public string PlazoDias { get; set; }
        public string FechaVencimiento { get; set; }
        public string Valor{ get; set;}
    }
}
