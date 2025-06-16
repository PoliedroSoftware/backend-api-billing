using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Domain.BillingPos.Ports;
using Poliedro.Billing.Domain.CreditNote.Ports;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.Ports;
using Poliedro.Billing.Domain.SendEmail.Ports;
using Poliedro.Billing.Domain.SuccessInvoice.Ports;
using Poliedro.Billing.Domain.CustomersId.Ports;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.CreditNote;
using Poliedro.Billing.Domain.UpdateCurrentlyNumber.Port;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.POS.EDS;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.SendMessage;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.SuccessInvoice;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.CustomersId;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.UpdateCurrentlyNumber;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Adapter;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.SendEmail;
using Poliedro.Billing.Application.SendEmail.Ports;
using Poliedro.Billing.Domain.GetInvoice.DomainGetInvoice;
using Poliedro.Billing.Infrastructure.GetInvoice.Adapters;


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
            services.AddTransient<IFERetailService, FERetailService>();
            services.AddTransient<Domain.FERetail.Ports.IInvoiceFE, Adapter.FE.Retail.InvoiceFEService>();
            services.AddTransient<Domain.FERetail.Ports.IInvoiceLastFE, Adapter.FE.Retail.InvoiceLastFERepository>();
            services.AddTransient<Domain.FERetail.Ports.IDatabaseUtils, Adapter.FE.Retail.DatabaseUtils>();
            services.AddTransient<Domain.FERetail.Ports.IGetItemFE, Adapter.FE.Retail.GetItem>();
            services.AddTransient<IGetItemsInvoiceFERetail, GetItemsInvoiceFERetail>();
            services.AddTransient<Domain.FERetail.Ports.IInsertInvoiceFE, Adapter.FE.Retail.InsertInvoice>();
            services.AddTransient<IBillingService, BillingPosService>();
            services.AddTransient<IInvoicePos, Adapter.POS.EDS.InvoicePosService>();
            services.AddTransient<IInvoiceLastPos, Adapter.POS.EDS.InvoiceLastPosRepository>();
            services.AddTransient<IGetItemPos, Adapter.POS.EDS.GetItem>();
            services.AddTransient<IGetItemsInvoicePos, GetItemsInvoicePos>();
            services.AddTransient<IInsertInvoicePos, Adapter.POS.EDS.InsertInvoice>();
            services.AddTransient<IDatabaseUtilsPos, Adapter.POS.EDS.DatabaseUtils>();
            services.AddTransient<ISendMessage, SendMessageService>();
            services.AddTransient<ISuccessInvoiceRepository, SuccessInvoiceRepository>();
            services.AddTransient<ICreditNoteDomainService, CreditNoteDomainService>();
            services.AddTransient<IUpdateCurrentlyNumber, UpdateCurrentlyNumberService>();
            services.AddTransient<IEmailSender, SmtpEmailSender>();
            services.AddTransient<ICustomersIdRepository, CustomersIdRepository>();
            services.AddTransient<IEmailBodyRenderer, HtmlEmailBodyRenderer>();
            services.AddTransient<IGetInvoiceDomainGetInvoice, GetInvoiceDomainGetInvoice>();
            services.AddTransient<EmailErrorHandler>();
            return services;
        }
    }
}
