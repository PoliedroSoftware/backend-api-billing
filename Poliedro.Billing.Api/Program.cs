
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Poliedro.Billing.Api;
using Poliedro.Billing.Api.Common.Configurations;
using Poliedro.Billing.Api.Endpoints.v1.Billing;
using Poliedro.Billing.Api.Endpoints.v1.Client;
using Poliedro.Billing.Api.Endpoints.v1.CompanyProvider;
using Poliedro.Billing.Api.Endpoints.v1.CustomersId;
using Poliedro.Billing.Api.Endpoints.v1.DianResolution;
using Poliedro.Billing.Api.Endpoints.v1.FERetail;
using Poliedro.Billing.Api.Endpoints.v1.GetInvoice;
using Poliedro.Billing.Api.Endpoints.v1.InvoicesPendingWithDetails;
using Poliedro.Billing.Api.Endpoints.v1.Location;
using Poliedro.Billing.Api.Endpoints.v1.PdfInvoice;
using Poliedro.Billing.Api.Endpoints.v1.Server;
using Poliedro.Billing.Api.Endpoints.v1.Siigo;
using Poliedro.Billing.Api.Endpoints.v1.SuccessInvoice;
using Poliedro.Billing.Api.Endpoints.v1.Tns;
using Poliedro.Billing.Application;
using Poliedro.Billing.Application.Common.Behaviors;
using Poliedro.Billing.Domain.Ports;
using Poliedro.Billing.Infraestructure.External.Plemsi;
using Poliedro.Billing.Infraestructure.External.Siigo;
using Poliedro.Billing.Infraestructure.External.TNS;
using Poliedro.Billing.Infraestructure.Persistence.Mysql;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Adapter;
using Poliedro.Billing.Api.Observability;
using Scalar.AspNetCore;
using Poliedro.Billing.Api.Administration;
using Poliedro.Billing.Domain.Billing.Ports;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
builder.Services.Configure<BillingAdministrationOptions>(config.GetSection("BillingAdministration"));
builder.Services.AddHttpClient<IBillingLicenseAuthorizer, OdooBillingLicenseAuthorizer>(client =>
    client.Timeout = TimeSpan.FromSeconds(5))
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);






builder.Services
    .AddWebApi()
    .AddApplication()
    .AddObservability()
    .AddExternalPlemsi(builder.Configuration)
    .AddExternalTns(builder.Configuration)
    .AddExternalSiigo(builder.Configuration)
    .AddPersistence(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddRouting(routing => routing.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Poliedro Billing API",
        Version = "v1",
        Description = "API de facturaci�n de Poliedro"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT en el siguiente formato: Bearer {token}"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });

    options.CustomSchemaIds(type => type.FullName);
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("PoliedroBilling", policy =>
    {
        var allowedOrigins = config.GetSection("AllowedOrigins").Get<List<string>>() ?? new List<string>();
        policy.WithOrigins(allowedOrigins.ToArray())
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});


builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
builder.Services.AddSingleton<IMessageProvider, MessageProvider>();
builder.Services.AddHealthChecks()
    .AddMySql(Environment.GetEnvironmentVariable("MYSQL_CONNECTION")
        ?? builder.Configuration.GetConnectionString("MysqlConnection"), name: "sql", tags: ["ready"]);

var app = builder.Build();

// Additive, idempotent schema setup for the shared administrative database.
if (config.GetValue("Database:InitializeEmissionFailureSchema", true))
{
    await using var schemaScope = app.Services.CreateAsyncScope();
    var database = schemaScope.ServiceProvider.GetRequiredService<Poliedro.Billing.Infraestructure.Persistence.Mysql.Context.DataBaseContext>();
    await database.Database.ExecuteSqlRawAsync("""
        CREATE TABLE IF NOT EXISTS billing_emission_failure (
            id BIGINT NOT NULL AUTO_INCREMENT,
            company_id INT NOT NULL,
            resolution_id INT NOT NULL,
            client_name VARCHAR(200) NOT NULL,
            invoice_number VARCHAR(100) NOT NULL,
            prefix VARCHAR(30) NOT NULL,
            document_type VARCHAR(20) NOT NULL,
            reason VARCHAR(2000) NOT NULL,
            failed_at DATETIME(6) NOT NULL,
            PRIMARY KEY (id),
            INDEX ix_billing_emission_failure_company_date (company_id, failed_at)
        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
        """);
}

// Observability: Prometheus HTTP metrics middleware
app.UsePrometheusMetrics()
   .MapPrometheusMetrics();

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.ToString(),
            entries = report.Entries.ToDictionary(
                e => e.Key,
                e => new
                {
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    duration = e.Value.Duration.ToString(),
                    data = e.Value.Data,
                    tags = e.Value.Tags,
                    exception = e.Value.Exception?.Message
                }
            )
        }, new System.Text.Json.JsonSerializerOptions { WriteIndented = false });
        await context.Response.WriteAsync(result);
    }
});
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
});

app.UseExceptionHandler();

app.UseCors("PoliedroBilling");

// Map Minimal API Endpoints BEFORE OpenAPI/Scalar
var apiV1 = app.MapGroup("api/v1");
app.MapBillingAdministration();

apiV1.MapGroup("/billing").MapBillingEndpoints();
var clientEndpoints = apiV1.MapGroup("/client").MapClientEndpoints();
var resolutionEndpoints = apiV1.MapGroup("/dianresolution").MapDianResolutionEndpoints();
apiV1.MapGroup("/Controllers/v1/FERetail").MapFERetailEndpoints();
apiV1.MapGroup("/getinvoice").MapGetInvoiceEndpoints();
apiV1.MapGroup("/invoicespendingwithdetails").MapInvoicesPendingWithDetailsEndpoints();
app.MapGroup("api/billing").MapPdfInvoiceEndpoints();
var serverEndpoints = apiV1.MapGroup("/server").MapServerEndpoints();
app.MapGroup("api/v1/billing/invoices").MapSiigoEndpoints();
apiV1.MapGroup("/invoice").MapSuccessInvoiceEndpoints();
app.MapGroup("api/v1/billing/sales/create").MapTnsEndpoints();
apiV1.MapGroup("/customers").MapCustomersIdEndpoints();
apiV1.MapGroup("/location").MapLocationEndpoints();
var providerEndpoints = apiV1.MapGroup("/companyProvider").MapCompanyProviderEndpoints();
if (config.GetValue<bool>("BillingAdministration:Enabled"))
{
    clientEndpoints.AddEndpointFilter<BillingAdminFilter>();
    resolutionEndpoints.AddEndpointFilter<BillingAdminFilter>();
    serverEndpoints.AddEndpointFilter<BillingAdminFilter>();
    providerEndpoints.AddEndpointFilter<BillingAdminFilter>();
}

// Configure Swagger and Scalar
app.UseSwagger();
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("Poliedro Billing API")
        .WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl)
        .WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
});

app.Run();

public partial class Program { }
