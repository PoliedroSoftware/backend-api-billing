using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Application.Billing.AutoMappers;
using Poliedro.Billing.Application.Client.AutoMappers;
using Poliedro.Billing.Application.Common.Behaviors;
using Poliedro.Billing.Application.CompanyProvider.AutoMappers;
using Poliedro.Billing.Application.CustomersId.AutoMappers;
using Poliedro.Billing.Application.DianResolution.AutoMappers;
using Poliedro.Billing.Application.GetInvoice.AutoMappers;
using Poliedro.Billing.Application.Location.AutoMappers;

using Poliedro.Billing.Application.Server.AutoMappers;
using Poliedro.Billing.Application.SuccessInvoice.AutoMappers;
using Poliedro.Billing.Application.Billing.Commands.CreateBilling;
using Poliedro.Billing.Application.Billing.Services.Factories;
using Poliedro.Billing.Application.Billing.Services.Ports;
using Poliedro.Billing.Application.Billing.Services.Preparation;
using Poliedro.Billing.Application.Billing.Services.Factories;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Methods.Billing.Preparation.ElectronicBilling;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.Billing;
using System.Reflection;

namespace Poliedro.Billing.Application;

public static class DependencyInjectionService
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IBillingProviderAdapterFactory, BillingProviderAdapterFactory>();
        services.AddScoped<IGetProcessorBilling, BillingPrepareFactory>();
        services.AddTransient<PrepareBillingFE>();
        services.AddTransient<PrepareBillingPOS>();
        services.AddScoped<IPrepareItemBilling, PrepareItemElectronic>();
        services.AddScoped<IGetAllTaxTotalsBilling, GetAllTaxTotalsBilling>();
        services.AddScoped<IBillingValidateScript, ValidateScriptBilling>();
        services.AddScoped<ICalculateCheckDigits, CalculateCheckDigitsBilling>();
        services.AddScoped<IAllowanceChargesBilling, GetAllowanceChargesBilling>();

        #region Mappers
        var mapper = new MapperConfiguration(config =>
        {
            
            config.AddProfile(new ClientMapper());
            config.AddProfile(new CompanyProviderMapper());
            config.AddProfile(new ServerMapper());
            config.AddProfile(new DianResolutionMapper());
            config.AddProfile(new PaginationMapper());
            config.AddProfile(new SuccessInvoiceAutoMapper());
            config.AddProfile(new CustumersIdAutoMapper());
            config.AddProfile(new GetInvoiceMapper());
            config.AddProfile(new BillingAutoMapper());
            config.AddProfile(new MunicipalityMapper());

        }, Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        services.AddSingleton(mapper.CreateMapper());
        #endregion

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        // Force production DI through the provider-neutral adapter factory. The compatibility
        // constructor on the handler exists only for existing direct test/caller construction.
        services.AddTransient<IRequestHandler<CreateBillingCommand, CreateBillingCommandResult>>(provider =>
            new CreateBillingHandler(
                provider.GetRequiredService<IDianResolutionGetByIdService>(),
                provider.GetRequiredService<ICompanyProviderGetByIdService>(),
                provider.GetRequiredService<IGetProcessorBilling>(),
                provider.GetRequiredService<IBillingProviderAdapterFactory>(),
                provider.GetRequiredService<IBillingEmissionResultRecorder>(),
                provider.GetRequiredService<IMapper>(),
                provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<CreateBillingHandler>>(),
                provider.GetRequiredService<IClientGetByIdService>(),
                provider.GetRequiredService<IBillingLicenseAuthorizer>(),
                provider.GetRequiredService<IEmissionFailureRepository>()));
        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehaviour<,>)
        );

        return services;
    }
}
