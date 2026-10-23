using AutoMapper;
using Moq;
using Poliedro.Billing.Application.Client.Commands.CreateClient;
using Poliedro.Billing.Application.Client.Commands.DeleteClient;
using Poliedro.Billing.Application.Client.Commands.UpdateClient;
using Poliedro.Billing.Application.Client.Dtos;
using Poliedro.Billing.Application.Client.Queries.GetAllClient;
using Poliedro.Billing.Application.Client.Queries.GetClientBillingElectronicById;
using Poliedro.Billing.Application.CompanyProvider.Commands.CreateCompanyProvider;
using Poliedro.Billing.Application.CompanyProvider.Commands.DeleteCompanyProvider;
using Poliedro.Billing.Application.CompanyProvider.Commands.UpdateCompanyProvider;
using Poliedro.Billing.Application.CompanyProvider.Dtos;
using Poliedro.Billing.Application.CompanyProvider.Queries.GetCompanyProviderById;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using System.Net;

namespace Poliedro.Billing.Api.Tests;

public sealed class ApplicationHandlerUnitTests
{
    private static readonly Error NotFound = Error.CreateInstance("not_found", "Record not found", HttpStatusCode.NotFound);

    [Fact]
    public async Task ClientCreateHandlerMapsAndDelegatesSuccessfulCommand()
    {
        var domain = new Mock<IClientDomainService>();
        var mapper = new Mock<IMapper>();
        var command = new CreateClientCommand { CompanyId = 42, Name = "Client", Nit = "TEST-42", Active = true };
        var entity = new ClientEntity { CompanyId = 42, Name = "Client", Nit = "TEST-42", Active = true };
        mapper.Setup(x => x.Map<ClientEntity>(command)).Returns(entity);
        domain.Setup(x => x.CreateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<VoidResult, Error>.Success(VoidResult.Instance));

        var result = await new CreateClientCommandHandler(domain.Object, mapper.Object).Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        domain.Verify(x => x.CreateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ClientUpdateHandlerPropagatesDomainError()
    {
        var domain = new Mock<IClientDomainService>();
        var mapper = new Mock<IMapper>();
        var command = new UpdateClientCommand { CompanyId = 404, Name = "Missing" };
        var entity = new ClientEntity { CompanyId = 404, Name = "Missing" };
        mapper.Setup(x => x.Map<ClientEntity>(command)).Returns(entity);
        domain.Setup(x => x.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<VoidResult, Error>.Failure(NotFound));

        var result = await new UpdateClientBillingElectronicCommandHandler(domain.Object, mapper.Object)
            .Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(NotFound, result.Error);
    }

    [Fact]
    public async Task ClientDeleteHandlerDelegatesIdentifierAndReturnsDomainResult()
    {
        var domain = new Mock<IClientDomainService>();
        domain.Setup(x => x.DeleteAsync(9, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<VoidResult, Error>.Failure(NotFound));

        var result = await new DeleteClientCommandHandler(domain.Object)
            .Handle(new DeleteClientCommand { Id = 9 }, CancellationToken.None);

        Assert.Equal(NotFound, result.Error);
        domain.Verify(x => x.DeleteAsync(9, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ClientQueriesMapResultsAndPropagateLookupErrors()
    {
        var domain = new Mock<IClientDomainService>();
        var mapper = new Mock<IMapper>();
        var clients = new[] { new ClientEntity { CompanyId = 1, Name = "Active" } };
        var dtos = new List<ClientDto> { new() };
        domain.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IEnumerable<ClientEntity>, Error>.Success(clients));
        mapper.Setup(x => x.Map<List<ClientDto>>(clients)).Returns(dtos);
        domain.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ClientEntity, Error>.Failure(NotFound));

        var all = await new GetAllClientQueryHandler(domain.Object, mapper.Object)
            .Handle(new GetAllClientQuery(), CancellationToken.None);
        var byId = await new GetClientBillingElectronicByIdQueryHandler(domain.Object, mapper.Object)
            .Handle(new GetClientByIdQuery { Id = 99 }, CancellationToken.None);

        Assert.Same(dtos, all.Value);
        Assert.False(byId.IsSuccess);
        Assert.Equal(NotFound, byId.Error);
    }

    [Fact]
    public async Task CompanyProviderCreateUpdateLookupAndDeleteHandlersDelegateAndMap()
    {
        var createService = new Mock<ICompanyProviderCreateService>();
        var updateService = new Mock<ICompanyProviderUpdateService>();
        var getService = new Mock<ICompanyProviderGetByIdService>();
        var deleteService = new Mock<ICompanyProviderDeleteService>();
        var mapper = new Mock<IMapper>();
        var input = new CreateCompanyProviderDto(5, 1, 2, null, null, "test-only", 0, null, null, 1);
        var updateInput = new UpdateCompanyProviderDto(5, 1, 2, null, null, null, 0, null, null, 0);
        var entity = new CompanyProviderEntity { CompanyProviderId = 11, CompanyId = 5, ProviderId = 1, ServiceId = 2, Active = 1 };
        var dto = new CompanyProviderDto(11, 5, 1, 2, null, null, null, 0, null, null, 1);
        mapper.Setup(x => x.Map<CompanyProviderEntity>(input)).Returns(entity);
        mapper.Setup(x => x.Map<CompanyProviderEntity>(updateInput)).Returns(new CompanyProviderEntity());
        mapper.Setup(x => x.Map<CompanyProviderDto>(entity)).Returns(dto);
        mapper.Setup(x => x.Map<CompanyProviderDto>(It.Is<CompanyProviderEntity>(p => p.CompanyProviderId == 11))).Returns(dto);
        createService.Setup(x => x.CreateCompanyProviderAsync(entity, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        updateService.Setup(x => x.UpdateCompanyProviderAsync(It.Is<CompanyProviderEntity>(p => p.CompanyProviderId == 11), It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        getService.Setup(x => x.GetCompanyProviderByIdAsync(11, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        deleteService.Setup(x => x.DeleteCompanyProviderAsync(11, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var created = await new CreateCompanyProviderCommandHandler(createService.Object, mapper.Object)
            .Handle(new CreateCompanyProviderCommand(input), CancellationToken.None);
        var updated = await new UpdateCompanyProviderCommandHandler(updateService.Object, mapper.Object)
            .Handle(new UpdateCompanyProviderCommand(11, updateInput), CancellationToken.None);
        var loaded = await new GetCompanyProviderByIdHandler(getService.Object, mapper.Object)
            .Handle(new GetCompanyProviderByIdQuery(11), CancellationToken.None);
        var deleted = await new DeleteCompanyProviderCommandHandler(deleteService.Object)
            .Handle(new DeleteCompanyProviderCommand(11), CancellationToken.None);

        Assert.Equal(dto, created);
        Assert.Equal(dto, updated);
        Assert.Equal(dto, loaded);
        Assert.True(deleted);
        updateService.Verify(x => x.UpdateCompanyProviderAsync(It.Is<CompanyProviderEntity>(p => p.CompanyProviderId == 11), It.IsAny<CancellationToken>()), Times.Once);
        deleteService.Verify(x => x.DeleteCompanyProviderAsync(11, It.IsAny<CancellationToken>()), Times.Once);
    }
}
