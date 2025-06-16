using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Application.Client.AutoMappers;
using Poliedro.Billing.Application.Common.Behaviors;
using Poliedro.Billing.Application.CustomersId.AutoMappers;
using Poliedro.Billing.Application.DianResolution.AutoMappers;
using Poliedro.Billing.Application.GetInvoice.AutoMappers;
using Poliedro.Billing.Application.InvoiceDetailElectronic.AutoMappers;
using Poliedro.Billing.Application.InvoicePendingWithDetails.AutoMappers;
using Poliedro.Billing.Application.NotifyResolution.AutoMappers;
using Poliedro.Billing.Application.PendingInvoice.AutoMappers;
using Poliedro.Billing.Application.Server.AutoMappers;
using Poliedro.Billing.Application.SuccessInvoice.AutoMappers;
using System.Reflection;

namespace Poliedro.Billing.Application;

public static class DependencyInjectionService
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        #region Mappers
        var mapper = new MapperConfiguration(config =>
        {
            
            config.AddProfile(new ClientMapper());
            config.AddProfile(new ServerMapper());
            config.AddProfile(new DianResolutionMapper());
            config.AddProfile(new PaginationMapper());
            config.AddProfile(new PendingInvoiceMapper());
            config.AddProfile(new SuccessInvoiceAutoMapper());
            config.AddProfile(new NotifyResolutionAutoMapper());
            config.AddProfile(new CustumersIdAutoMapper());
            config.AddProfile(new GetInvoiceMapper());
            config.AddProfile(new InvoiceElectronicProfile());
            config.AddProfile(new InvoiceFEPendingWithDetailsProfile());

        });
        services.AddSingleton(mapper.CreateMapper());
        #endregion

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehaviour<,>)
        );

        return services;
    }
}