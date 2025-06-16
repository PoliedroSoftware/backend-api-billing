namespace Poliedro.Billing.Domain.NotifyResolution.Entities
{
    public class ExpirationAlert
    {
        public bool IsExpired { get; set; }
        public bool IsCloseToExpire { get; set; }
        public string? Message { get; set; }
        public int? DaysToExpire { get; set; }
        public int? RemainingNumeration { get; set; }
        public bool StatusNumeration { get; set; }
    }
}