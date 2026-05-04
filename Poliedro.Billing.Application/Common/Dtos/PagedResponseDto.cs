namespace Poliedro.Billing.Application.Common.Dtos;

public class PagedResponseDto<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public IEnumerable<T> Data { get; set; } = [];
    public MetaDto Meta { get; set; } = new();
}

public class MetaDto
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}