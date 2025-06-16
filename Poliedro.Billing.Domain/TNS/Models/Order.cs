namespace Poliedro.Billing.Domain.TNS.Models
{
    public class Order
    {
        public string CodigoPrefijo { get; set; }
        public string Numero { get; set; }
        public string NumeroFactura { get; set; }
        public string Fecha { get; set; }
        public int KardexId { get; set; }
        public string CodigoPedido { get; set; }
        public string NombreCliente { get; set; }
        public string CodTercero { get; set; }
        public string CodVendedor { get; set; }
        public string CodDespachar { get; set; }
        public string CodFormaPago { get; set; }
        public string CodBanco { get; set; }
        public string FechaVence { get; set; }
        public int PlazoDias { get; set; }
        public string Observacion { get; set; }
        public List<OrderDetail> DetallePedido { get; set; }
        public List<PaymentMethodDetail> DetalleFormaPago { get; set; }
        public List<DetailDiscount> DetalleDescuentos { get; set; }
    }
}
