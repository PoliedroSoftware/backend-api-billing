using AutoMapper;
using Moq;
using Poliedro.Billing.Application.DianResolution.Commands.CreateDianResolution;
using Poliedro.Billing.Application.DianResolution.Commands.DeleteDianResolution;
using Poliedro.Billing.Application.DianResolution.Commands.UpdateDianResolution;
using Poliedro.Billing.Application.DianResolution.Dtos;
using Poliedro.Billing.Application.DianResolution.Queries.GetAllDianResolution;
using Poliedro.Billing.Application.DianResolution.Queries.GetDianResolutionById;
using Poliedro.Billing.Application.Server.Commands.CreateServer;
using Poliedro.Billing.Application.Server.Commands.UpdateServer;
using Poliedro.Billing.Application.Server.Dtos;
using Poliedro.Billing.Application.Server.Queries.GellAllServer;
using Poliedro.Billing.Application.Server.Queries.GetServerById;
using Poliedro.Billing.Domain.Common.Pagination;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Server.DomainService;
using Poliedro.Billing.Domain.Server.Entities;
using System.Net;

namespace Poliedro.Billing.Api.Tests;

public sealed class AdministrationHandlerUnitTests
{
    private static readonly Error Missing = Error.CreateInstance("not_found", "Not found", HttpStatusCode.NotFound);

    [Fact]
    public async Task Dian_resolution_commands_map_create_and_update_and_delete_by_id()
    {
        var service = new Mock<IDianResolutionDomainService>();
        var mapper = new Mock<IMapper>();
        var entity = new DianResolutionEntity { ResolutionId = 6 };
        var create = new CreateDianResolutionCommand(6, 2, 0, "R-6", "A", 0, 6, 0, 1, 99, 1, DateTime.Today, DateTime.Today.AddMonths(6), 0, 0, true);
        var update = new UpdateDianResolutionCommand(6, 2);
        mapper.Setup(x => x.Map<DianResolutionEntity>(create)).Returns(entity);
        mapper.Setup(x => x.Map<DianResolutionEntity>(update)).Returns(entity);
        service.Setup(x => x.CreateAsync(entity, It.IsAny<CancellationToken>())).ReturnsAsync(Result<VoidResult, Error>.Success(VoidResult.Instance));
        service.Setup(x => x.UpdateAsync(entity, It.IsAny<CancellationToken>())).ReturnsAsync(Result<VoidResult, Error>.Success(VoidResult.Instance));
        service.Setup(x => x.DeleteAsync(It.Is<DianResolutionEntity>(r => r.ResolutionId == 6), It.IsAny<CancellationToken>())).ReturnsAsync(Result<VoidResult, Error>.Failure(Missing));

        Assert.True((await new CreateDianResolutionCommandHandler(service.Object, mapper.Object).Handle(create, default)).IsSuccess);
        Assert.True((await new UpdateDianResolutionCommandHandler(service.Object, mapper.Object).Handle(update, default)).IsSuccess);
        var deleted = await new DeleteDianResolutionCommandHandler(service.Object, mapper.Object).Handle(new DeleteDianResolutionCommand { Id = 6 }, default);

        Assert.False(deleted.IsSuccess);
        Assert.Equal(Missing, deleted.Error);
        service.Verify(x => x.DeleteAsync(It.Is<DianResolutionEntity>(r => r.ResolutionId == 6), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dian_resolution_queries_map_lists_and_propagate_lookup_error()
    {
        var service = new Mock<IDianResolutionDomainService>();
        var mapper = new Mock<IMapper>();
        var entities = new[] { new DianResolutionEntity { ResolutionId = 7 } };
        var dtos = new List<DianResolutionDto>();
        var one = new DianResolutionDto(1, 1);
        service.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Result<IEnumerable<DianResolutionEntity>, Error>.Success(entities));
        service.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync(Result<DianResolutionEntity, Error>.Failure(Missing));
        mapper.Setup(x => x.Map<List<DianResolutionDto>>(entities)).Returns(dtos);
        var all = await new GetAllDianResolutionQueryHandler(service.Object, mapper.Object).Handle(new GetAllDianResolutionQuery(), default);
        var missing = await new GetDianResolutionByIdQueryHandler(service.Object, mapper.Object).Handle(new GetDianResolutionByIdQuery { Id = 99 }, default);

        Assert.Same(dtos, all.Value);
        Assert.False(missing.IsSuccess);
        Assert.Equal(Missing, missing.Error);
    }

    [Fact]
    public async Task Server_handlers_create_update_and_map_reads()
    {
        var service = new Mock<IServerDomainService>();
        var mapper = new Mock<IMapper>();
        var entity = new ServerEntity { ServerId = 2, Ip = "localhost" };
        var create = new CreateServerCommand { Ip = "localhost", DatabaseName = "test", DbUsername = "test", DbPassword = "test" };
        var update = new UpdateServerCommand { ServerId = 2, Ip = "localhost", DatabaseName = "test", DbUsername = "test", DbPassword = "test" };
        mapper.Setup(x => x.Map<ServerEntity>(create)).Returns(entity);
        mapper.Setup(x => x.Map<ServerEntity>(update)).Returns(entity);
        service.Setup(x => x.CreateAsync(entity)).ReturnsAsync(Result<VoidResult, Error>.Success(VoidResult.Instance));
        service.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(Result<VoidResult, Error>.Failure(Missing));
        service.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(Result<ServerEntity, Error>.Success(entity));
        service.Setup(x => x.GetAllAsync(It.IsAny<PaginationParams>())).ReturnsAsync([entity]);
        mapper.Setup(x => x.Map<ServerDto>(entity)).Returns(new ServerDto());
        mapper.Setup(x => x.Map<List<ServerDto>>(It.IsAny<IEnumerable<ServerEntity>>())).Returns([new ServerDto()]);

        Assert.True((await new CreateServerCommandHandler(service.Object, mapper.Object).Handle(create, default)).IsSuccess);
        var updateResult = await new UpdateServerCommandHandler(service.Object, mapper.Object).Handle(update, default);
        Assert.Equal(Missing, updateResult.Error);
        Assert.NotNull(await new GetServerByIdQueryHandler(service.Object, mapper.Object).Handle(new GetServerByIdQuery { Id = 2 }, default));
        Assert.Single(await new GetAllServerQueryHandler(service.Object, mapper.Object).Handle(new GellAllServerQuery(new PaginationParams()), default));
    }
}
