using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;
using Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;
using Poliedro.Billing.Application.SendEmail;
using Poliedro.Billing.Application.SendEmail.Ports;
using Poliedro.Billing.Domain.Billing.Ports;

using Poliedro.Billing.Domain.Common.Methods.Billing.Prepare.Plemsi.ElectronicBilling;
using Poliedro.Billing.Domain.Common.Methods.Billing.Validate;
using Poliedro.Billing.Domain.Common.Methods.Billing.Validate.Plemsi;
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
            services.AddDbContext<DataBaseContext>(
                options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)
            ));

            services.AddTransient<IMessageProvider, MessageProvider>();

            // FE (Factura Electrónica) implementations
            services.AddTransient<IGetItemsInvoiceFERetail, GetItemsInvoiceFERetail>();
            services.AddTransient<IDatabaseUtils, Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail.DatabaseUtils>();
            services.AddTransient<IGetItemFE, Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail.GetItem>();
            services.AddTransient<IInsertInvoiceFE, Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail.InsertInvoice>();

            services.AddTransient<IBillingService, BillingPosService>();

            services.AddTransient<IGetItemPos, Adapter.POS.EDS.GetItem>();
            services.AddTransient<IGetItemsInvoicePos, GetItemsInvoicePos>();
            services.AddTransient<IInsertInvoicePos, Adapter.POS.EDS.InsertInvoice>();
            services.AddTransient<IDatabaseUtilsPos, Adapter.POS.EDS.DatabaseUtils>();
            services.AddTransient<IInvoiceLastPos, Adapter.POS.EDS.InvoiceLastPosRepository>();
            services.AddTransient<ISendMessage, SendMessageService>();
            services.AddTransient<ISuccessInvoiceRepository, SuccessInvoiceRepository>();
            
            services.AddTransient<IUpdateCurrentlyNumber, UpdateCurrentlyNumberService>();
            services.AddTransient<IEmailSender, SmtpEmailSender>();
            services.AddTransient<ICustomersIdRepository, CustomersIdRepository>();
            services.AddTransient<IEmailBodyRenderer, HtmlEmailBodyRenderer>();
            services.AddTransient<IGetInvoiceDomainGetInvoice, GetInvoiceDomainGetInvoice>();

            // Factoría y strategies
            services.AddScoped<IGetProcessorBilling, BillingPrepareFactory>();
            services.AddTransient<PrepareBillingFE>();
            services.AddTransient<PrepareBillingPOS>();

            services.AddTransient<IBillingSender, BillingSenderFE>();
            services.AddTransient<IBillingSender, BillingSenderPOS>();
            services.AddScoped<IBillingSenderFactory, BillingSenderFactory>();
            services.AddTransient<IBillingResponseApi, BillingResponseApi>();

            // Domain adapters / stubs
            services.AddTransient<IFERetailService, Adapter.FE.Retail.FERetailService>();
            services.AddTransient<IGetLastInvoiceBilling, Adapter.Billing.Impl.Plemsi.GetLastInvoiceBillingPlemsi>();
            services.AddTransient<ICreditNoteDomainService, Adapter.CreditNote.CreditNoteDomainService>();

            // Dependencias internas de PrepareBillingFE/POS
            services.AddScoped<IPrepareItemBilling, PrepareItemElectronic>();
            services.AddScoped<IGetAllTaxTotalsBilling, GetAllTaxTotalsBilling>();
            services.AddScoped<IBillingValidateScript, ValidateScriptBilling>();
            services.AddScoped<ICalculateCheckDigits, CalculateCheckDigitsBilling>();
            services.AddScoped<IAllowanceChargesBilling, GetAllowanceChargesBilling>();

           

            

            // Location
            services.AddHttpClient<IMunicipalityService, PlemsiLocationService>();

            return services;
        }
    }
}