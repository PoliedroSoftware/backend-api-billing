
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);






builder.Services
    .AddWebApi()
    .AddApplication()
    .AddExternalPlemsi(builder.Configuration)
    .AddExternalTns(builder.Configuration)
    .AddExternalSiigo(builder.Configuration)
    .AddPersistence(builder.Configuration);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionConfiguration>();
});

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

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
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
    .AddMySql(builder.Configuration.GetConnectionString("MysqlConnection"), name: "sql", tags: ["ready"]);

var app = builder.Build();

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

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("PoliedroBilling");

// Map Minimal API Endpoints BEFORE OpenAPI/Scalar
var apiV1 = app.MapGroup("api/v1");

apiV1.MapGroup("/billing").MapBillingEndpoints();
apiV1.MapGroup("/client").MapClientEndpoints();
apiV1.MapGroup("/dianresolution").MapDianResolutionEndpoints();
apiV1.MapGroup("/Controllers/v1/FERetail").MapFERetailEndpoints();
apiV1.MapGroup("/getinvoice").MapGetInvoiceEndpoints();
apiV1.MapGroup("/invoicespendingwithdetails").MapInvoicesPendingWithDetailsEndpoints();
app.MapGroup("api/billing").MapPdfInvoiceEndpoints();
apiV1.MapGroup("/server").MapServerEndpoints();
app.MapGroup("api/v1/billing/invoices").MapSiigoEndpoints();
apiV1.MapGroup("/invoice").MapSuccessInvoiceEndpoints();
app.MapGroup("api/v1/billing/sales/create").MapTnsEndpoints();
apiV1.MapGroup("/customers").MapCustomersIdEndpoints();
apiV1.MapGroup("/location").MapLocationEndpoints();
apiV1.MapGroup("/companyProvider").MapCompanyProviderEndpoints();

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
