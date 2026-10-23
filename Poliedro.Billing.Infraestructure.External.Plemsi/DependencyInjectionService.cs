using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Contracts;
using Poliedro.Billing.Application.Billing.Services.Factories;
using Poliedro.Billing.Application.Billing.Services.Factories;
using Poliedro.Billing.Application.Billing.Services.Preparation;
using Poliedro.Billing.Application.SendEmail;
using Poliedro.Billing.Application.SendEmail.Ports;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Contracts;
using Poliedro.Billing.Application.Billing.Services.Factories;

using Poliedro.Billing.Domain.Common.Methods.Billing.Preparation.ElectronicBilling;
using Poliedro.Billing.Domain.CreditNote.Ports;
using Poliedro.Billing.Domain.CustomersId.Ports;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.GetInvoice.DomainGetInvoice;
using Poliedro.Billing.Domain.Location.Ports;
using Poliedro.Billing.Domain.Ports;
using Poliedro.Billing.Domain.SuccessInvoice.Ports;
using Poliedro.Billing.Domain.UpdateCurrentlyNumber.Port;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Impl;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Impl.Plemsi;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Selectors.Plemsi;

using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.CustomersId;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.GetInvoice;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Location;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.POS.EDS;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.SendEmail;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.SendMessage;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.SuccessInvoice;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.UpdateCurrentlyNumber;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Adapter;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Infraestructure.External.Plemsi
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddExternalPlemsi(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION") ?? configuration.GetConnectionString("MysqlConnection");
            var configuredVersion = configuration["Database:ServerVersion"];
            var serverVersion = ResolveServerVersion(configuredVersion, connectionString);
            services.AddDbContext<DataBaseContext>(
                options => options.UseMySql(connectionString, serverVersion
            ));

            services.AddTransient<IMessageProvider, MessageProvider>();

            services.AddHttpClient();

            // FE (Factura Electr�nica) implementations
            services.AddTransient<IGetItemsInvoiceFERetail, GetItemsInvoiceFERetail>();
            services.AddTransient<IDatabaseUtils, Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail.DatabaseUtils>();
            services.AddTransient<IRelationalConnectionFactory, MySqlServerDatabaseConnectionFactory>();
            services.AddTransient<IGetItemFE, Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail.GetItem>();

            services.AddTransient<IBillingService, BillingPosService>();

            services.AddTransient<IGetItemPos, Adapter.POS.EDS.GetItem>();
            services.AddTransient<IGetItemsInvoicePos, GetItemsInvoicePos>();
            services.AddTransient<IInsertInvoicePos, Adapter.POS.EDS.InsertInvoice>();
            services.AddTransient<IDatabaseUtilsPos, Adapter.POS.EDS.DatabaseUtils>();
            services.AddTransient<IInvoiceLastPos, Adapter.POS.EDS.InvoiceLastPosRepository>();
            services.AddHttpClient<ISendMessage, SendMessageService>();
            services.AddTransient<ISuccessInvoiceRepository, SuccessInvoiceRepository>();
            
            services.AddTransient<IUpdateCurrentlyNumber, UpdateCurrentlyNumberService>();
            services.AddTransient<IEmailSender, SmtpEmailSender>();
            services.AddTransient<ICustomersIdRepository, CustomersIdRepository>();
            services.AddTransient<IEmailBodyRenderer, HtmlEmailBodyRenderer>();
            services.AddHttpClient<IGetInvoiceDomainGetInvoice, GetInvoiceDomainGetInvoice>();

            // Factor�a y strategies

            // Registrar concrete implementations para que el decorador pueda resolverlas
            services.AddTransient<BillingSenderFE>();
            services.AddTransient<BillingSenderPOS>();
            services.AddTransient<IBillingSender>(provider => provider.GetRequiredService<BillingSenderFE>());
            services.AddTransient<IBillingSender>(provider => provider.GetRequiredService<BillingSenderPOS>());

            services.AddScoped<IBillingSenderFactory, BillingSenderRegistry>();
            services.AddScoped<IBillingProviderAdapter, PlemsiBillingProviderAdapter>();
            // Domain adapters / stubs
            services.AddTransient<IFERetailService, Adapter.FE.Retail.FERetailService>();
            services.AddTransient<IGetLastInvoiceBilling, Adapter.Billing.Impl.Plemsi.GetLastInvoiceBillingPlemsi>();
            services.AddTransient<ICreditNoteDomainService, Adapter.CreditNote.CreditNoteDomainService>();

            // Dependencias internas de PrepareBillingFE/POS

          

            services.AddHttpClient<IMunicipalityService, PlemsiLocationService>();

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
}
