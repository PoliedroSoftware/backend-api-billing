using System.Net;

namespace Poliedro.Billing.Domain.Common.Results.Errors;

/// <summary>
/// Represents an error with a code, description, and associated HTTP status code.
/// </summary>
public record Error
{
    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the description of the error.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the HTTP status code associated with the error.
    /// </summary>
    public HttpStatusCode HttpStatusCode { get; set; }

    // Private constructor prevents instantiation from outside
    private Error(string code, string description, HttpStatusCode httpStatusCode)
    {
        Code = code;
        Description = description;
        HttpStatusCode = httpStatusCode;
    }

    /// <summary>
    /// Creates an instance of the Error record.
    /// </summary>
    /// <remarks>
    /// This method should be used exclusively for initializing static readonly properties of specific error.
    /// </remarks>
    /// <param name="code">The error code.</param>
    /// <param name="description">The description of the error.</param>
    /// <param name="httpStatusCode">The HTTP status code associated with the error.</param>
    /// <returns>An instance of the Error record.</returns>
    public static Error CreateInstance(string code, string description, HttpStatusCode httpStatusCode)
    {
        return new Error(code, description, httpStatusCode);
    }
}
