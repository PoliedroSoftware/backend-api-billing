using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Poliedro.Billing.Application.Billing.Ports;
using Poliedro.Billing.Application.Billing.Ports.Providers.Plemsi;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;
using System.Net.Http.Headers;
using System.Text;

namespace Poliedro.Billing.Infraestructure.External.Provider.Provider.Plemsi.Senders;

/// <summary>
/// Implementación de ISenderPlemsi para facturas FE (Factura Electrónica) en Plemsi.
/// Maneja el envío de una factura a Plemsi y la normalización de la respuesta.
/// </summary>
public class PlemsiFESender : ISenderPlemsi
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<PlemsiFESender> _logger;

    public PlemsiFESender(
        IHttpClientFactory httpClientFactory,
        IConfiguration config,
        ILogger<PlemsiFESender> logger)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
        _logger = logger;
    }

    public async Task<BillingResultItem> SendAsync(
        object invoice,
        DianResolutionEntity resolution,
        CompanyProviderEntity company,
        CancellationToken cancellationToken)
    {
        using var client = _httpClientFactory.CreateClient();

        try
        {
            _logger.LogInformation("Enviando factura FE a Plemsi");

            // Obtener la URL según ambiente
            var isProd = bool.Parse(_config["Enviroment:Production"] ?? "false");
            var url = isProd
                ? _config["ApiPlemsi:FEUrl"]
                : _config["ApiPlemsiQa:FEUrl"];

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new InvalidOperationException("URL de Plemsi FE no configurada");
            }

            // Serializar factura
            var jsonContent = JsonConvert.SerializeObject(invoice);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Crear request con autenticación
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = stringContent
            };
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", company.ApiKey);

            // Enviar a Plemsi
            using var response = await client.SendAsync(httpRequest, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Respuesta fallida de Plemsi FE. Status: {Status}. Contenido: {Content}",
                    response.StatusCode, responseContent);

                return new BillingResultItem
                {
                    InvoiceNumber = ExtractInvoiceNumber(invoice),
                    Success = false,
                    Message = $"Error HTTP {response.StatusCode}: {responseContent}"
                };
            }

            return NormalizePlemsiResponse(responseContent, invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando factura FE a Plemsi");

            return new BillingResultItem
            {
                InvoiceNumber = ExtractInvoiceNumber(invoice),
                Success = false,
                Message = $"Error de conexión: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Normaliza la respuesta de Plemsi a un único BillingResultItem.
    /// </summary>
    private BillingResultItem NormalizePlemsiResponse(string responseContent, object invoice)
    {
        try
        {
            var responses = JsonConvert.DeserializeObject<List<dynamic>>(responseContent);
            var response = responses?.FirstOrDefault();

            if (response is null)
            {
                _logger.LogWarning("Respuesta de Plemsi vacía o inválida");
                return new BillingResultItem
                {
                    InvoiceNumber = ExtractInvoiceNumber(invoice),
                    Success = false,
                    Message = "Respuesta desconocida de Plemsi"
                };
            }

            bool success = response["success"] ?? false;
            string? cufe = response["cufe"]?.ToString();
            string? uuid = response["uuid"]?.ToString();
            string? errorMsg = response["message_error"]?.ToString();

            return new BillingResultItem
            {
                InvoiceNumber = ExtractInvoiceNumber(invoice),
                Success = success,
                Message = success
                    ? $"CUFE: {cufe}, UUID: {uuid}"
                    : errorMsg ?? "Error desconocido de Plemsi"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error normalizando respuesta de Plemsi FE");

            return new BillingResultItem
            {
                InvoiceNumber = ExtractInvoiceNumber(invoice),
                Success = false,
                Message = $"Error procesando respuesta: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Extrae el número de factura del objeto.
    /// </summary>
    private static string? ExtractInvoiceNumber(object invoice)
    {
        try
        {
            var prop = invoice.GetType().GetProperty("number");
            return prop?.GetValue(invoice)?.ToString();
        }
        catch
        {
            return "Desconocido";
        }
    }
}
