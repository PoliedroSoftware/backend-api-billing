using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.OpenApi.Models;
using Poliedro.Billing.Api;
using Poliedro.Billing.Api.Common.Configurations;
using Poliedro.Billing.Api.Controllers.v1.Billing;
using Poliedro.Billing.Application;
using Poliedro.Billing.Application.Common.Behaviors;
using Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote;
using Poliedro.Billing.Domain.CreditNote.Ports;
using Poliedro.Billing.Domain.Ports;
using Poliedro.Billing.Infraestructure.External.Plemsi;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.CreditNote;
using Poliedro.Billing.Infraestructure.External.Siigo;
using Poliedro.Billing.Infraestructure.External.TNS;
using Poliedro.Billing.Infraestructure.Persistence.Mysql;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Adapter;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;
using WorkerServiceBilling;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);


builder.Services.AddScoped<IMockPendingInvoiceService, MockBillingService>();
builder.Services.AddHttpClient<ICreditNoteDomainService, CreditNoteDomainService>(client =>
{
    client.BaseAddress = new Uri("http://159.89.239.32:5009"); 
});




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

builder.Services.AddHostedService<Worker>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateCreditNoteCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
builder.Services.AddSingleton<IMessageProvider, MessageProvider>();
var app = builder.Build();

app.UseCors("PoliedroBilling");

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapControllers();
app.Run();
