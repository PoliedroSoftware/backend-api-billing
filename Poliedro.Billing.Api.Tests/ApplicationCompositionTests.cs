using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Api;
using Poliedro.Billing.Application;
using Poliedro.Billing.Application.DianResolution.Commands.CreateDianResolution;
using Poliedro.Billing.Application.GetInvoice.Dtos;
using Poliedro.Billing.Domain.GetInvoice.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Resolution.Enums;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.Server.Entities;
using Poliedro.Billing.Application.CompanyProvider.Dtos;
using Poliedro.Billing.Application.Server.Commands.CreateServer;
using Poliedro.Billing.Application.Server.Commands.UpdateServer;
using Poliedro.Billing.Application.Server.Dtos;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Api.Tests;

public sealed class ApplicationCompositionTests
{
    [Fact]
    public void Application_registration_exposes_mediator_and_all_registered_mappings_are_valid()
    {
        var services = new ServiceCollection();
        services.AddApplication();
        services.AddWebApi();
        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IMediator>());
        var mapper = provider.GetRequiredService<IMapper>();
        mapper.ConfigurationProvider.AssertConfigurationIsValid();

        var resolution = mapper.Map<DianResolutionEntity>(new CreateDianResolutionCommand(
            8, 4, ResolutionType.FE, "R-8", "TST", 0, 6, 0, 1, 100, 1,
            DateTime.Today, DateTime.Today.AddMonths(6), 0, 0, true));
        Assert.Equal(0, resolution.ResolutionId);
        Assert.Equal("TST", resolution.Prefix);

        var invoiceLines = new List<InvoiceLineDto>();
        var attachments = new List<AttachmentDto>();
        var invoice = mapper.Map<GetInvoiceDto>(new GetInvoiceEntity { InvoiceLines = invoiceLines, Attachments = attachments });
        Assert.Empty(invoice.InvoiceLine);
        Assert.Empty(invoice.Attachment);
    }

    [Fact]
    public void Company_provider_mapping_preserves_configuration_and_credentials_in_both_directions()
    {
        var mapper = CreateMapper();
        var provider = new CompanyProviderEntity
        {
            CompanyProviderId = 17,
            CompanyId = 8,
            ProviderId = 2,
            ServiceId = 4,
            ApiUser = "tenant-user",
            ApiPassword = "configured-secret",
            ApiKey = "provider-key",
            EnvironmentType = EnvironmentType.prod,
            HeadNote = "Encabezado",
            FooterNote = "Pie",
            Active = 1
        };

        var dto = mapper.Map<CompanyProviderDto>(provider);
        var roundTrip = mapper.Map<CompanyProviderEntity>(dto);

        Assert.Equal(provider.CompanyProviderId, dto.CompanyProviderId);
        Assert.Equal(provider.ApiPassword, dto.ApiPassword);
        Assert.Equal(provider.EnvironmentType, dto.EnvironmentType);
        Assert.Equal(provider.FooterNote, roundTrip.FooterNote);
        Assert.Equal(provider.Active, roundTrip.Active);
    }

    [Fact]
    public void Server_command_mappings_preserve_connection_fields_and_ignore_generated_id_on_create()
    {
        var mapper = CreateMapper();
        var create = new CreateServerCommand
        {
            Ip = "10.0.0.5",
            DatabaseName = "billing_test",
            DbUsername = "billing-reader",
            DbPassword = "local-test-only"
        };

        var created = mapper.Map<ServerEntity>(create);
        Assert.Equal(create.Ip, created.Ip);
        Assert.Equal(create.DatabaseName, created.DatabaseName);
        Assert.Equal(create.DbPassword, created.DbPassword);

        var update = new UpdateServerCommand
        {
            ServerId = 31,
            Ip = "10.0.0.6",
            DatabaseName = "billing_next",
            DbUsername = "billing-updater",
            DbPassword = "new-local-test-only"
        };
        var updated = mapper.Map<ServerEntity>(update);
        Assert.Equal(update.ServerId, updated.ServerId);
        Assert.Equal(update.DbUsername, updated.DbUsername);

        var dto = mapper.Map<ServerDto>(updated);
        Assert.Equal(updated.Ip, dto.Ip);
        Assert.Equal(default, dto.CreationDate); // CreationDate is persistence-owned.
    }

    [Fact]
    public void Invoice_read_mapping_round_trips_nested_lines_and_attachments()
    {
        var mapper = CreateMapper();
        var source = new GetInvoiceEntity
        {
            Id = "invoice-17", Company = "company-8", CreatedBy = "operator", State = "issued",
            Preview = "preview", ResolutionNumber = "RES-1", Prefix = "FE", Number = "17",
            TypeDocumentId = 1, Date = "2026-10-22", Time = "09:30:00", EmailDeliveryStatus = "sent",
            SendEmailNotification = true, Consecutive = "FE17", CreatedAt = "created", UpdatedAt = "updated",
            HeadNote = "head", FootNote = "foot", Notes = "notes", Resolution = "resolution",
            QRStr = "qr", TechProviderDefaultFootNote = "provider footer",
            InvoiceLines = [new Poliedro.Billing.Domain.GetInvoice.Entities.InvoiceLineDto
            {
                _id = "line-1", Company = "company-8", Invoice = "invoice-17", UnitMeasureId = 70,
                InvoicedQuantity = "2", LineExtensionAmount = "100", FreeOfChargeIndicator = false,
                TaxTotals = [], Description = "Product", Notes = "line notes", Code = "SKU-1",
                TypeItemIdentificationId = 4, PriceAmount = "50", BaseQuantity = "1", WithHoldingTaxTotal = []
            }],
            Attachments = [new Poliedro.Billing.Domain.GetInvoice.Entities.AttachmentDto
            {
                _id = "attachment-1", Company = "company-8", Invoice = "invoice-17",
                Filename = "invoice.pdf", B64data = "dGVzdA=="
            }],
            PaymentForm = [], TaxTotals = [], WithHoldingTaxTotal = [], AllowanceCharges = []
        };

        var dto = mapper.Map<GetInvoiceDto>(source);
        var roundTrip = mapper.Map<GetInvoiceEntity>(dto);

        Assert.Equal("invoice-17", roundTrip.Id);
        Assert.Equal("SKU-1", dto.InvoiceLine[0].Code);
        Assert.Equal("invoice.pdf", dto.Attachment[0].Filename);
        Assert.Equal("dGVzdA==", roundTrip.Attachments[0].B64data);
    }

    private static IMapper CreateMapper()
    {
        var services = new ServiceCollection();
        services.AddApplication();
        using var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<IMapper>();
    }
}
