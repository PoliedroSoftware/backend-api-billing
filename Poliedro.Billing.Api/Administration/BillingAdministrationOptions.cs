namespace Poliedro.Billing.Api.Administration;

public sealed class BillingAdministrationOptions
{
    public bool Enabled { get; set; }
    public string ApiKey { get; set; } = "";
    public string LicenseUrl { get; set; } = "";
    public Dictionary<int, string> CompanyLicenseUrls { get; set; } = new();

    public bool IsConfigured => Enabled && ApiKey.Length >= 32
        && IsValidLicenseUrl(LicenseUrl);

    public static bool IsValidLicenseUrl(string url) => Uri.TryCreate(url, UriKind.Absolute, out var uri)
        && (uri.Scheme == "https" || (uri.Scheme == "http" && uri.IsLoopback))
        && string.IsNullOrEmpty(uri.UserInfo);
}
