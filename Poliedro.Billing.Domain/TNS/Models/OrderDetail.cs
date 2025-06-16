namespace Poliedro.Billing.Domain.TNS.Models
{
    public class OrderDetail
    {
        public string CodMat { get; set; }
        public string CodBodega { get; set; }
        public string CodTalla { get; set; }
        public string CodColor { get; set; }
        public int Cantidad { get; set; }
        public string TipoUnidad { get; set; }
        public decimal Descuento { get; set; }
        public string CentrosCostos { get; set; }
        public decimal PorcIva { get; set; }
        public decimal Valor { get; set; }
        public decimal ImpConsumo { get; set; }
        public string Observacion { get; set; }
        public List<ItemSerial> ItemsSerial { get; set; }
    }
}
