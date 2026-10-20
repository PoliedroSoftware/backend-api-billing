using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace Poliedro.Billing.Api.Administration;

public sealed class BillingAdminFilter(IOptions<BillingAdministrationOptions> options) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var settings = options.Value;
        if (!settings.IsConfigured)
            return Results.Problem(statusCode: 503, title: "Administración no configurada.");
        var provided = context.HttpContext.Request.Headers["X-Billing-Admin-Key"].ToString();
        if (!Matches(provided, settings.ApiKey))
            return Results.Problem(statusCode: 401, title: "Credencial administrativa requerida.");
        return await next(context);
    }

    public static bool Matches(string provided, string expected) => expected.Length >= 32
        && CryptographicOperations.FixedTimeEquals(
            SHA256.HashData(Encoding.UTF8.GetBytes(provided)),
            SHA256.HashData(Encoding.UTF8.GetBytes(expected)));
}
