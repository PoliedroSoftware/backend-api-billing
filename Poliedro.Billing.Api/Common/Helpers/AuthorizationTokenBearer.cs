namespace Poliedro.Billing.Api.Common.Helpers
{
    public static class TokenHelper
    {
        public static string? ExtractBearerToken(HttpRequest request)
        {
            if (!request.Headers.TryGetValue("Authorization", out var authHeader))
                return null;

            var header = authHeader.ToString();
            if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return null;

            return header["Bearer ".Length..].Trim();
        }
    }
}
