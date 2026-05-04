using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Poliedro.Billing.Domain.Location.Entities;
using Poliedro.Billing.Domain.Location.Ports;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Location;

public class PlemsiLocationService(HttpClient httpClient) : IMunicipalityService
{
    private const string Endpoint =
        "https://api.plemsi.com/api/common/list/municipality/complete";

    public async Task<IEnumerable<MunicipalityEntity>> GetAllAsync(
        string apiKey,
        CancellationToken cancellationToken)
    {
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        var response = await httpClient.GetAsync(Endpoint, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var wrapper = JsonSerializer.Deserialize<PlemsiResponse>(json, options);

        if (wrapper?.Data == null)
            return [];

        return wrapper.Data.Select(m => new MunicipalityEntity
        {
            Id = m.Id,
            Name = m.Name,
            DepartmentName = m.Department?.Name ?? string.Empty,
            Divipola = m.Code
        });
    }

    private class PlemsiResponse
    {
        public bool Success { get; set; }
        public List<PlemsiMunicipalityResponse> Data { get; set; } = [];
    }

    private class PlemsiMunicipalityResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("department_id")]
        public int DepartmentId { get; set; }
        public PlemsiDepartmentResponse? Department { get; set; }
    }

    private class PlemsiDepartmentResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}