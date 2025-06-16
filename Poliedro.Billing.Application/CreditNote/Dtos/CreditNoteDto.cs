using Poliedro.Billing.Application.DianResolution.Dtos;
using Poliedro.Billing.Application.Server.Dtos;

namespace Poliedro.Billing.Application.CreditNote.Dtos
{
    public class CreditNoteDto
    {
        public int CreditNoteId { get; set; } = default!;
        public string Resolution { get; set; } = string.Empty;
        public string Prefix { get; set; } = string.Empty;
        public int Number { get; set; } = default!;
        public decimal TotalToPay { get; set; } = default!;
        public decimal InvoiceBaseTotal { get; set; } = default!;
        public decimal InvoiceTaxExclusiveTotal { get; set; } = default!;
        public decimal InvoiceTaxInclusiveTotal { get; set; } = default!;
        public decimal AllowanceTotal { get; set; } = default!;
        public string HeadNote { get; set; } = string.Empty;
        public string FootNote { get; set; } = string.Empty;


    }
}
