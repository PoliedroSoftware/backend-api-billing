using System.Net;
using System.Reflection;
using System.Text;
using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Api.Endpoints.v1.Client;
using Poliedro.Billing.Api.Endpoints.v1.CompanyProvider;
using Poliedro.Billing.Api.Endpoints.v1.DianResolution;
using Poliedro.Billing.Api.Endpoints.v1.CustomersId;
using Poliedro.Billing.Api.Endpoints.v1.GetInvoice;
using Poliedro.Billing.Api.Endpoints.v1.PdfInvoice;
using Poliedro.Billing.Api.Endpoints.v1.Location;
using Poliedro.Billing.Api.Endpoints.v1.SuccessInvoice;
using Poliedro.Billing.Api.Endpoints.v1.Tns;
using Poliedro.Billing.Api.Endpoints.v1.Siigo;
using Poliedro.Billing.Api.Endpoints.v1.InvoicesPendingWithDetails;
using Poliedro.Billing.Api.Endpoints.v1.Server;
using Poliedro.Billing.Application.Server.Commands.CreateServer;
using Poliedro.Billing.Application.Server.Commands.UpdateServer;
using Poliedro.Billing.Application.Server.Queries.GetServerById;
using Poliedro.Billing.Application.Server.Queries.GellAllServer;
using Poliedro.Billing.Application.InvoicesPendingWithDetails.Queries.GetAllInvoicesPendingWithDetails;
using Poliedro.Billing.Application.Client.Commands.CreateClient;
using Poliedro.Billing.Application.Client.Commands.DeleteClient;
using Poliedro.Billing.Application.Client.Commands.UpdateClient;
using Poliedro.Billing.Application.Client.Queries.GetAllClient;
using Poliedro.Billing.Application.Client.Queries.GetClientBillingElectronicById;
using Poliedro.Billing.Application.CompanyProvider.Commands.CreateCompanyProvider;
using Poliedro.Billing.Application.CompanyProvider.Commands.DeleteCompanyProvider;
using Poliedro.Billing.Application.CompanyProvider.Commands.UpdateCompanyProvider;
using Poliedro.Billing.Application.CompanyProvider.Dtos;
using Poliedro.Billing.Application.CompanyProvider.Queries.GetCompanyProviderById;
using Poliedro.Billing.Application.DianResolution.Commands.CreateDianResolution;
using Poliedro.Billing.Application.DianResolution.Commands.DeleteDianResolution;
using Poliedro.Billing.Application.DianResolution.Commands.UpdateDianResolution;
using Poliedro.Billing.Application.DianResolution.Queries.GetAllDianResolution;
using Poliedro.Billing.Application.DianResolution.Queries.GetDianResolutionById;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Api.Tests;

public sealed class AdministrationEndpointsIntegrationTests
{
    [Fact]
    public async Task Client_and_resolution_read_endpoints_dispatch_queries_and_return_success()
    {
        var mediator = CreateMediator();
        await using var app = await CreateApp(mediator);
        using var client = app.GetTestClient();

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/clients/")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/clients/12")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/resolutions/")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/resolutions/34")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/providers/56")).StatusCode);

        var requests = ((EndpointMediatorProxy)(object)mediator).Requests;
        Assert.Collection(requests,
            request => Assert.IsType<GetAllClientQuery>(request),
            request => Assert.Equal(12, Assert.IsType<GetClientByIdQuery>(request).Id),
            request => Assert.IsType<GetAllDianResolutionQuery>(request),
            request => Assert.Equal(34, Assert.IsType<GetDianResolutionByIdQuery>(request).Id),
            request => Assert.IsType<GetCompanyProviderByIdQuery>(request));
    }

    [Fact]
    public async Task Client_resolution_and_provider_delete_routes_dispatch_commands()
    {
        var mediator = CreateMediator();
        await using var app = await CreateApp(mediator);
        using var client = app.GetTestClient();

        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync("/clients/12")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync("/resolutions/34")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync("/providers/56")).StatusCode);

        var requests = ((EndpointMediatorProxy)(object)mediator).Requests;
        Assert.Collection(requests,
            request => Assert.Equal(12, Assert.IsType<DeleteClientCommand>(request).Id),
            request => Assert.Equal(34, Assert.IsType<DeleteDianResolutionCommand>(request).Id),
            request => Assert.Equal(56, Assert.IsType<DeleteCompanyProviderCommand>(request).Id));
    }

    [Fact]
    public async Task Create_routes_bind_payloads_and_dispatch_commands()
    {
        var mediator = CreateMediator();
        await using var app = await CreateApp(mediator);
        using var client = app.GetTestClient();

        Assert.Equal(HttpStatusCode.NoContent, (await client.PostAsync("/clients/", Json("{}"))).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.PostAsync("/resolutions/", Json("{}"))).StatusCode);
        Assert.Equal(HttpStatusCode.Created, (await client.PostAsync("/providers/", Json("{}"))).StatusCode);

        var requests = ((EndpointMediatorProxy)(object)mediator).Requests;
        Assert.Collection(requests,
            request => Assert.IsType<CreateClientCommand>(request),
            request => Assert.IsType<CreateDianResolutionCommand>(request),
            request => Assert.IsType<CreateCompanyProviderCommand>(request));
    }

    [Fact]
    public async Task Update_routes_bind_payloads_and_dispatch_commands()
    {
        var mediator = CreateMediator();
        await using var app = await CreateApp(mediator);
        using var client = app.GetTestClient();

        Assert.Equal(HttpStatusCode.NoContent, (await client.PutAsync("/clients/", Json("{}"))).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.PatchAsync("/resolutions/", Json("{}"))).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PutAsync("/providers/56", Json("{}"))).StatusCode);

        var requests = ((EndpointMediatorProxy)(object)mediator).Requests;
        Assert.Collection(requests,
            request => Assert.IsType<UpdateClientCommand>(request),
            request => Assert.IsType<UpdateDianResolutionCommand>(request),
            request => Assert.IsType<UpdateCompanyProviderCommand>(request));
    }

    [Fact]
    public async Task Customer_and_location_endpoints_require_bearer_authorization_before_dispatch()
    {
        var mediator = CreateMediator();
        await using var app = await CreateApp(mediator);
        using var client = app.GetTestClient();

        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/customers/123")).StatusCode);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "test");
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/customers/123")).StatusCode);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "synthetic-token");
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/customers/123")).StatusCode);

        client.DefaultRequestHeaders.Authorization = null;
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/locations/municipalities")).StatusCode);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "synthetic-token");
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/locations/municipalities")).StatusCode);
        Assert.IsType<Poliedro.Billing.Application.Location.Queries.GetAllMunicipalities.GetAllMunicipalitiesQuery>(
            ((EndpointMediatorProxy)(object)mediator).Requests[1]);
    }

    [Fact]
    public async Task Invoice_endpoint_validates_cufe_and_authorization_then_dispatches_query()
    {
        var mediator = CreateMediator();
        await using var app = await CreateApp(mediator);
        using var client = app.GetTestClient();

        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/invoices/?cufe=%20")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/invoices/?cufe=CUFE-1")).StatusCode);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "synthetic-token");
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/invoices/?cufe=CUFE-1")).StatusCode);

        var proxy = (EndpointMediatorProxy)(object)mediator;
        proxy.NullResponseTypes.Add(typeof(Poliedro.Billing.Application.GetInvoice.Queries.GetByIdGetInvoiceQuery));
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/invoices/?cufe=CUFE-MISSING")).StatusCode);
        proxy.ExceptionToThrow = new InvalidOperationException("simulated handler failure");
        Assert.Equal(HttpStatusCode.InternalServerError, (await client.GetAsync("/invoices/?cufe=CUFE-ERROR")).StatusCode);
    }

    [Fact]
    public async Task Pdf_and_success_invoice_endpoints_enforce_auth_and_preserve_not_found_behavior()
    {
        var mediator = CreateMediator();
        await using var app = await CreateApp(mediator);
        using var client = app.GetTestClient();

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "test");
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/files/pdfinvoice/pdf/123")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/success/")).StatusCode);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "synthetic-token");
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/files/pdfinvoice/pdf/123")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/success/?page=2&perPage=5&order=asc")).StatusCode);
    }

    [Fact]
    public async Task Tns_and_siigo_endpoints_require_authorization_and_reject_empty_batches()
    {
        var mediator = CreateMediator();
        await using var app = await CreateApp(mediator);
        using var client = app.GetTestClient();

        Assert.Equal(HttpStatusCode.Unauthorized, (await client.PostAsync("/tns/", Json("{\"orders\":[]}"))).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.PostAsync("/siigo/", Json("{\"invoices\":[]}"))).StatusCode);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "synthetic-token");
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/tns/", Json("{\"orders\":[]}"))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/siigo/", Json("{\"invoices\":[]}"))).StatusCode);
        Assert.Empty(((EndpointMediatorProxy)(object)mediator).Requests);
    }

    [Fact]
    public async Task Pending_invoice_endpoint_returns_not_found_for_missing_data_and_500_for_provider_exception()
    {
        var mediator = CreateMediator();
        var proxy = (EndpointMediatorProxy)(object)mediator;
        await using var app = await CreateApp(mediator);
        using var client = app.GetTestClient();

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/pending/7")).StatusCode);
        proxy.NullResponseTypes.Add(typeof(InvoicesPendingWithDetailsQuery));
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/pending/8")).StatusCode);
        proxy.NullResponseTypes.Clear();
        proxy.ExceptionToThrow = new InvalidOperationException("synthetic provider error");
        Assert.Equal(HttpStatusCode.InternalServerError, (await client.GetAsync("/pending/9")).StatusCode);
    }

    [Fact]
    public async Task Server_endpoints_validate_identifiers_and_dispatch_valid_crud_requests()
    {
        var mediator = CreateMediator();
        var proxy = (EndpointMediatorProxy)(object)mediator;
        await using var app = await CreateApp(mediator);
        using var client = app.GetTestClient();

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/servers/?pageNumber=1&pageSize=5")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/servers/0")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/servers/5")).StatusCode);
        var payload = Json("{\"ip\":\"127.0.0.1\",\"databaseName\":\"test\",\"dbUsername\":\"user\",\"dbPassword\":\"synthetic\"}");
        Assert.Equal(HttpStatusCode.NoContent, (await client.PostAsync("/servers/", payload)).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PutAsync("/servers/", Json("{}"))).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PutAsync("/servers/", Json("{\"serverId\":5,\"ip\":\"127.0.0.1\",\"databaseName\":\"test\",\"dbUsername\":\"user\",\"dbPassword\":\"synthetic\"}"))).StatusCode);

        proxy.NullResponseTypes.Add(typeof(GellAllServerQuery));
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/servers/")).StatusCode);
        proxy.NullResponseTypes.Clear();
        proxy.ErrorResponseTypes.Add(typeof(UpdateServerCommand));
        Assert.Equal(HttpStatusCode.InternalServerError, (await client.PutAsync("/servers/", Json("{\"serverId\":5,\"ip\":\"127.0.0.1\",\"databaseName\":\"test\",\"dbUsername\":\"user\",\"dbPassword\":\"synthetic\"}"))).StatusCode);
    }

    private static StringContent Json(string value) => new(value, Encoding.UTF8, "application/json");

    private static IMediator CreateMediator() => DispatchProxy.Create<IMediator, EndpointMediatorProxy>();

    private static async Task<WebApplication> CreateApp(IMediator mediator)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton(mediator);
        builder.Services.AddSingleton<IValidator<GetServerByIdQuery>, GetServerByIdQueryValidator>();
        builder.Services.AddSingleton<IValidator<UpdateServerCommand>, UpdateServerCommandValidator>();
        var app = builder.Build();
        app.MapGroup("/clients").MapClientEndpoints();
        app.MapGroup("/resolutions").MapDianResolutionEndpoints();
        app.MapGroup("/providers").MapCompanyProviderEndpoints();
        app.MapGroup("/customers").MapCustomersIdEndpoints();
        app.MapGroup("/invoices").MapGetInvoiceEndpoints();
        app.MapGroup("/files").MapPdfInvoiceEndpoints();
        app.MapGroup("/locations").MapLocationEndpoints();
        app.MapGroup("/success").MapSuccessInvoiceEndpoints();
        app.MapGroup("/tns").MapTnsEndpoints();
        app.MapGroup("/siigo").MapSiigoEndpoints();
        app.MapGroup("/pending").MapInvoicesPendingWithDetailsEndpoints();
        app.MapGroup("/servers").MapServerEndpoints();
        await app.StartAsync();
        return app;
    }
}

public class EndpointMediatorProxy : DispatchProxy
{
    public List<object> Requests { get; } = [];
    public HashSet<Type> NullResponseTypes { get; } = new();
    public HashSet<Type> ErrorResponseTypes { get; } = new();
    public Exception? ExceptionToThrow { get; set; }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod?.Name != "Send" || args?.FirstOrDefault() is not object request)
            throw new NotSupportedException($"Unexpected mediator call: {targetMethod?.Name}");

        Requests.Add(request);
        var responseType = targetMethod.GetGenericArguments()[0];
        if (ExceptionToThrow is Exception exception)
        {
            ExceptionToThrow = null;
            var fromException = typeof(Task).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(method => method.Name == nameof(Task.FromException) && method.IsGenericMethodDefinition);
            return fromException.MakeGenericMethod(responseType).Invoke(null, [exception])!;
        }
        if (NullResponseTypes.Contains(request.GetType()))
        {
            var fromResultNull = typeof(Task).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(method => method.Name == nameof(Task.FromResult) && method.IsGenericMethodDefinition)
                .MakeGenericMethod(responseType);
            return fromResultNull.Invoke(null, [null])!;
        }
        if (ErrorResponseTypes.Contains(request.GetType()))
        {
            var providerError = Poliedro.Billing.Domain.Common.Results.Errors.Error.CreateInstance(
                "synthetic", "synthetic failure", System.Net.HttpStatusCode.InternalServerError);
            var failure = responseType.GetMethod(nameof(Result<object, Poliedro.Billing.Domain.Common.Results.Errors.Error>.Failure))!
                .Invoke(null, [providerError]);
            var fromResultFailure = typeof(Task).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(method => method.Name == nameof(Task.FromResult) && method.IsGenericMethodDefinition)
                .MakeGenericMethod(responseType);
            return fromResultFailure.Invoke(null, [failure])!;
        }
        var response = CreateSuccessfulResponse(responseType);
        var fromResult = typeof(Task).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method => method.Name == nameof(Task.FromResult) && method.IsGenericMethodDefinition)
            .MakeGenericMethod(responseType);
        return fromResult.Invoke(null, [response])!;
    }

    private static object? CreateSuccessfulResponse(Type responseType)
    {
        if (!responseType.IsGenericType || responseType.GetGenericTypeDefinition() != typeof(Result<,>))
            return responseType == typeof(bool) ? true : CreateValue(responseType);

        var valueType = responseType.GetGenericArguments()[0];
        var value = CreateValue(valueType);
        return responseType.GetMethod(nameof(Result<object, Error>.Success))!.Invoke(null, [value]);
    }

    private static object? CreateValue(Type valueType)
    {
        if (valueType == typeof(VoidResult))
            return VoidResult.Instance;
        if (valueType.IsArray)
            return Array.CreateInstance(valueType.GetElementType()!, 0);
        if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            return Array.CreateInstance(valueType.GetGenericArguments()[0], 0);
        if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(List<>))
            return Activator.CreateInstance(valueType);
        if (valueType.IsValueType)
            return Activator.CreateInstance(valueType);
        var constructor = valueType.GetConstructors()
            .OrderByDescending(item => item.GetParameters().Length)
            .FirstOrDefault();
        if (constructor is null)
            return null;
        var arguments = constructor.GetParameters()
            .Select(parameter => parameter.ParameterType.IsValueType ? Activator.CreateInstance(parameter.ParameterType) : null)
            .ToArray();
        return constructor.Invoke(arguments);
    }
}
