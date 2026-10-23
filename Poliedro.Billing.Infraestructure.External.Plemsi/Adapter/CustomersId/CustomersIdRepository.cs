using Poliedro.Billing.Domain.CustomersId.Ports;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Net.Http.Headers;
using Poliedro.Billing.Domain.CustomersId.Entities;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.CustomersId;

public class CustomersIdRepository(IConfiguration configuration, HttpClient? httpClient = null) : ICustomersIdRepository
{
    private readonly HttpClient _httpClient = httpClient ?? new();
    private static readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<Customers> GetByIdAsync(string id, string token)
    {

        string baseUrl = configuration["ApiPlemsi:BaseUrl"];
        string url = $"{baseUrl}/customer/{id}";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        using HttpResponseMessage response = await _httpClient.SendAsync(request);

        string responseData = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var apiResponse = JsonSerializer.Deserialize<ApiResponseWrapper<Customers>>(responseData, jsonOptions);
            return apiResponse?.Data!;

        }

        return null!;
    }
}
