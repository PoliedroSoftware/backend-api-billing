using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Domain.SuccessInvoice.Entities;
using Poliedro.Billing.Domain.SuccessInvoice.Ports;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.SuccessInvoice
{
    public class SuccessInvoiceRepository : ISuccessInvoiceRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _FEUrl;
        private static readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public SuccessInvoiceRepository(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _FEUrl = configuration["ApiPlemsi:FEUrl"] ?? throw new ArgumentNullException("FEUrl not found in configuration");
        }
        public async Task<IEnumerable<Poliedro.Billing.Domain.SuccessInvoice.Entities.SuccessInvoice>> GetAllInvoice(SuccessInvoiceQueryParameters parameters)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", parameters.Token);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string url = $"{_FEUrl}?page={parameters.Page}&perPage={parameters.PerPage}&state=Emitted&order={parameters.Order}";
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            string responseData = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponseWrapper<SuccessInvoiceApiResponse>>(responseData, jsonOptions);
                return apiResponse?.Data?.Docs ?? [];
            }

            return [];
        }
    }
}
