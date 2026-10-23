using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Poliedro.Billing.Domain.Billing.Ports;

using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Common.Methods.Billing.Preparation.ElectronicBilling;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;

using Poliedro.Billing.Domain.InvoicePos.DomainService;
using Poliedro.Billing.Domain.InvoicePos.DomainService.Impl;
using Poliedro.Billing.Domain.InvoicePos.Ports;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Ports;

using Poliedro.Billing.Domain.PdfInvoice.Service;

using Poliedro.Billing.Domain.Ports;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Server.DomainService;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Adapter;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Client.DomainService.Impl;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.CompanyProvider.DomainService.Impl;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.DianResolution.DomainService.Impl;


using Poliedro.Billing.Infraestructure.Persistence.Mysql.InvoicePos;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.InvoicesPendingWithDetails.DomainService.Impl;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.PdfInvoice.DomainPdfInvoice;

using Poliedro.Billing.Infraestructure.Persistence.Mysql.Server.DomainService.Impl;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.EmissionFailures;
using Poliedro.Billing.Application.Billing.Services.Ports;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Billing;



namespace Poliedro.Billing.Infraestructure.Persistence.Mysql;

public static class DependencyInjectionService
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION") ?? configuration.GetConnectionString("MysqlConnection");
        var configuredVersion = configuration["Database:ServerVersion"];
        var serverVersion = ResolveServerVersion(configuredVersion, connectionString);
        services.AddDbContext<DataBaseContext>(
            options => options.UseMySql(connectionString, serverVersion
        ));
        services.AddScoped<IEmissionFailureRepository, EmissionFailureRepository>();
        services.AddTransient<IBillingEmissionResultRecorder, BillingEmissionResultRecorder>();
        services.AddTransient<IInvoiceEmissionStore, MySqlInvoiceEmissionStore>();
        services.AddScoped<IServerDatabaseConnectionFactory, MySqlServerDatabaseConnectionFactory>();
        services.AddScoped<IRelationalConnectionFactory, MySqlServerDatabaseConnectionFactory>();
        
        // Client services - specialized implementations
        services.AddScoped<IClientExistsService, ClientExistsService>();
        services.AddScoped<IClientCreateService, ClientCreateService>();
        services.AddScoped<IClientUpdateService, ClientUpdateService>();
        services.AddScoped<IClientGetAllService, ClientGetAllService>();
        services.AddScoped<IClientGetByIdService, ClientGetByIdService>();
        services.AddScoped<IClientDeleteService, ClientDeleteService>();
        
        // Composite service that implements IClientDomainService
        services.AddScoped<IClientDomainService, ClientBillingDomainService>(); // de Transient a Scoped

        // Company Provider services - specialized implementations
        services.AddScoped<ICompanyProviderGetByIdService, CompanyProviderGetByIdSevice>(); // existente viejo 
        services.AddScoped<ICompanyProviderCreateService, CompanyProviderCreateService>();   //  agregado
        services.AddScoped<ICompanyProviderUpdateService, CompanyProviderUpdateService>();   //  agregado
        services.AddScoped<ICompanyProviderDeleteService, CompanyProviderDeleteService>();   //  agregado

        // DianResolution services - specialized implementations
        services.AddScoped<IDianResolutionExistsService, DianResolutionExistsService>();
        services.AddScoped<IDianResolutionCreateService, DianResolutionCreateService>();
        services.AddScoped<IDianResolutionUpdateService, DianResolutionUpdateService>();
        services.AddScoped<IDianResolutionDeleteService, DianResolutionDeleteService>();
        services.AddScoped<IDianResolutionGetAllService, DianResolutionGetAllService>();
        services.AddScoped<IDianResolutionGetByIdService, DianResolutionGetByIdService>();
        
        // Composite service that implements IDianResolutionDomainService
        services.AddScoped<IDianResolutionDomainService, DianResolutionDomainService>();

        // Server services - specialized implementations
        services.AddScoped<IServerExistsService, ServerExistsService>();
        services.AddScoped<IServerCreateService, ServerCreateService>();
        services.AddScoped<IServerUpdateService, ServerUpdateService>();
        services.AddScoped<IServerGetAllService, ServerGetAllService>();
        services.AddScoped<IServerGetByIdService, ServerGetByIdService>();
        
        // Composite service that implements IServerDomainService
        services.AddScoped<IServerDomainService, ServerDomainService>();
        
        services.AddTransient<IMessageProvider, MessageProvider>();
       
        services.AddTransient<IInvoicePosDomainService, InvoicePosDomainService>();
        services.AddTransient<IInvoicePosRepository, InvoicePosRepository>();


        services.AddScoped<IPdfInvoiceService, PdfInvoiceService>();

 


        //services.AddTransient<IInvoicesPendingWithDetailsRepository,InvoicesPendingWithDetailsFERepository>();

        services.AddTransient<InvoicesPendingWithDetailsFERepository>();
        services.AddTransient<InvoicesPendingWithDetailsPOSRepository>();
        services.AddTransient<IInvoicesPendingWithDetailsStrategyFactory, InvoicesPendingWithDetailsStrategyFactory>();

        




        return services;
    }

    private static ServerVersion ResolveServerVersion(string? configuredVersion, string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(configuredVersion))
            return ServerVersion.AutoDetect(connectionString);

        const string mariaDbSuffix = "-mariadb";
        var isMariaDb = configuredVersion.EndsWith(mariaDbSuffix, StringComparison.OrdinalIgnoreCase);
        var versionText = isMariaDb ? configuredVersion[..^mariaDbSuffix.Length] : configuredVersion;
        var version = Version.Parse(versionText);
        return isMariaDb ? new MariaDbServerVersion(version) : new MySqlServerVersion(version);
    }
}
