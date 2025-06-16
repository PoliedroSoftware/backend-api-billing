using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Application.NotifyResolution.Services;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.InvoiceDetailElectronic.Ports;
using Poliedro.Billing.Domain.InvoicePendingWithDetails.Ports;
using Poliedro.Billing.Domain.InvoicePos.DomainService;
using Poliedro.Billing.Domain.InvoicePos.DomainService.Impl;
using Poliedro.Billing.Domain.InvoicePos.Ports;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Ports;
using Poliedro.Billing.Domain.NotifyResolution.Ports;
using Poliedro.Billing.Domain.NotifyResolution.Services;
using Poliedro.Billing.Domain.PdfInvoice.Service;
using Poliedro.Billing.Domain.PedingInvoice.DomainPedingInvoice;
using Poliedro.Billing.Domain.Ports;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Server.DomainService;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Adapter;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Client.DomainService.Impl;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.DianResolution.DomainService.Impl;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.InvoiceDetailElectronic.DomainService.Impl;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.InvoicePos;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.InvoicesPendingWithDetails.DomainService.Impl;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.PdfInvoice.DomainPdfInvoice;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.PedingInvoice.DomainPedingInvoice.Impl;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Server.DomainService.Impl;




namespace Poliedro.Billing.Infraestructure.Persistence.Mysql;

public static class DependencyInjectionService
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION") ?? configuration.GetConnectionString("MysqlConnection");
        services.AddDbContext<DataBaseContext>(
            options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)
        ));
        services.AddTransient<IClientDomainService, ClientBillingDomainService>();
        services.AddScoped<IServerDomainService, ServerDomainService>();

        services.AddScoped<IDianResolutionDomainService, DianResolutionDomainService>();
        services.AddTransient<IMessageProvider, MessageProvider>();
       
        services.AddTransient<IInvoicePosDomainService, InvoicePosDomainService>();
        services.AddTransient<IInvoicePosRepository, InvoicePosRepository>();

        services.AddTransient<IPedingInvoiceDomainPedingInvoice, PedingInvoiceDomainPedingInvoice>();

        services.AddScoped<IPdfInvoiceService, PdfInvoiceService>();

 
        services.AddScoped<INotifyResolutionAlertService, NotifyResolutionAlertService>();

        services.AddScoped<IInvoiceElectronicRepository, InvoiceDetailElectronicService>();

        //services.AddTransient<IInvoicesPendingWithDetailsRepository,InvoicesPendingWithDetailsFERepository>();

        services.AddTransient<InvoicesPendingWithDetailsFERepository>();
        services.AddTransient<InvoicesPendingWithDetailsPOSRepository>();
        services.AddTransient<IInvoicesPendingWithDetailsStrategyFactory, InvoicesPendingWithDetailsStrategyFactory>();



        return services;
    }
}
