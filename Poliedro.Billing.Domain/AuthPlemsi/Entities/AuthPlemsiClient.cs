using System;
using System.Collections.Generic;
using System.Text;

namespace Poliedro.Billing.Domain.AuthPlemsi.Entities
{
    public class AuthPlemsiClient
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? UserAccess { get; set; }
        public string? KeyAccess { get; set; }
        public int? ResolutionId { get; set; }
        public int? CreditNoteId { get; set; }
        public int? MultipleResolution { get; set; }
        public string? HeadNote { get; set; }
        public string? FoodNote { get; set; }
        public double? Active { get; set; }
    }
}
