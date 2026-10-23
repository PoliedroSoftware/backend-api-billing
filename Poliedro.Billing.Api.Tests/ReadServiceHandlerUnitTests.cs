using AutoMapper;
using Moq;
using Poliedro.Billing.Application.CustomersId.Queries.CustomersbyId;
using Poliedro.Billing.Application.GetInvoice.Dtos;
using Poliedro.Billing.Application.GetInvoice.Queries;
using Poliedro.Billing.Application.Location.Dtos;
using Poliedro.Billing.Application.Location.Queries.GetAllMunicipalities;
using Poliedro.Billing.Application.PdfInvoice.Queries;
using Poliedro.Billing.Application.SuccessInvoice.Dtos;
using Poliedro.Billing.Application.SuccessInvoice.Queries.GetAllSuccessInvoice;
using Poliedro.Billing.Domain.CustomersId.Entities;
using Poliedro.Billing.Domain.CustomersId.Ports;
using Poliedro.Billing.Domain.GetInvoice.DomainGetInvoice;
using Poliedro.Billing.Domain.GetInvoice.Entities;
using Poliedro.Billing.Domain.Location.Entities;
using Poliedro.Billing.Domain.Location.Ports;
using Poliedro.Billing.Domain.InvoicePos.Entities;
using Poliedro.Billing.Domain.InvoicePos.Ports;
using Poliedro.Billing.Domain.InvoicePos.DomainService.Impl;
using Poliedro.Billing.Domain.PdfInvoice.Service;
using Poliedro.Billing.Domain.SuccessInvoice.Entities;
using Poliedro.Billing.Domain.SuccessInvoice.Ports;

namespace Poliedro.Billing.Api.Tests;

public sealed class ReadServiceHandlerUnitTests
{
    [Fact]
    public async Task Pos_invoice_domain_service_forwards_prefix_and_cancellation_to_repository()
    {
        var repository = new Mock<IInvoicePosRepository>();
        var invoices = new[] { new InvoicePosEntity { number = "7", prefix = "TST" } };
        repository.Setup(x => x.GetAllAsync("TST", It.IsAny<CancellationToken>())).ReturnsAsync(invoices);

        var result = await new InvoicePosDomainService(repository.Object).GetAllAsync("TST", CancellationToken.None);

        Assert.Same(invoices, result);
        repository.Verify(x => x.GetAllAsync("TST", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Municipality_query_returns_mapped_rows_and_consistent_paging_metadata()
    {
        var service = new Mock<IMunicipalityService>();
        var mapper = new Mock<IMapper>();
        var source = new[] { new MunicipalityEntity { Id = 1, Name = "BogotÃ¡" }, new MunicipalityEntity { Id = 2, Name = "Cali" } };
        var mapped = new List<MunicipalityDto> { new(), new() };
        service.Setup(x => x.GetAllAsync("test-key", It.IsAny<CancellationToken>())).ReturnsAsync(source);
        mapper.Setup(x => x.Map<IEnumerable<MunicipalityDto>>(source)).Returns(mapped);

        var result = await new GetAllMunicipalitiesHandler(service.Object, mapper.Object)
            .Handle(new GetAllMunicipalitiesQuery("test-key"), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(2, result.Meta!.Total);
        Assert.Equal(1, result.Meta.TotalPages);
        Assert.Equal(mapped, result.Data);
    }

    [Fact]
    public async Task Pdf_query_delegates_document_identifier_and_authorization_token()
    {
        var service = new Mock<IPdfInvoiceService>();
        var bytes = new byte[] { 1, 2, 3 };
        service.Setup(x => x.GetPdfBytes("pdf-id", "Bearer test")).ReturnsAsync(bytes);

        var result = await new GetPdfInvoiceQueryHandler(service.Object)
            .Handle(new GetPdfInvoiceQuery("pdf-id", "Bearer test"), CancellationToken.None);

        Assert.Same(bytes, result);
    }

    [Fact]
    public async Task Customer_lookup_returns_found_record_or_empty_record()
    {
        var repository = new Mock<ICustomersIdRepository>();
        var customer = new Customers { Id = "c-1", FullName = "Cliente de prueba" };
        repository.Setup(x => x.GetByIdAsync("c-1", "token")).ReturnsAsync(customer);
        repository.Setup(x => x.GetByIdAsync("missing", "token")).ReturnsAsync((Customers)null!);
        var handler = new CustomersIdHandler(repository.Object);

        Assert.Same(customer, await handler.Handle(new CustomersIdQuery("c-1", "token"), CancellationToken.None));
        Assert.Equal(string.Empty, (await handler.Handle(new CustomersIdQuery("missing", "token"), CancellationToken.None)).Id);
    }

    [Fact]
    public async Task Invoice_query_maps_repository_result()
    {
        var repository = new Mock<IGetInvoiceDomainGetInvoice>();
        var mapper = new Mock<IMapper>();
        var invoice = new GetInvoiceEntity();
        var dto = new GetInvoiceDto();
        repository.Setup(x => x.GetByIdAsync("cufe-1", "token", It.IsAny<CancellationToken>())).ReturnsAsync(invoice);
        mapper.Setup(x => x.Map<GetInvoiceDto>(invoice)).Returns(dto);

        var result = await new GetByIdGetInvoiceQueryHandler(repository.Object, mapper.Object)
            .Handle(new GetByIdGetInvoiceQuery("cufe-1", "token"), CancellationToken.None);

        Assert.Same(dto, result);
    }

    [Fact]
    public async Task Success_invoice_query_maps_records_using_requested_paging()
    {
        var repository = new Mock<ISuccessInvoiceRepository>();
        var mapper = new Mock<IMapper>();
        var parameters = new SuccessInvoiceQueryParameters("token", "desc", 2, 10);
        var rows = new[] { new SuccessInvoice { _Id = "id-1", Company = "test", CreatedBy = "test", State = "success", Resolution_Number = "R-1", Number = "1", Date = "2026-01-01", Time = "10:00", Customer = new() { Identification_Number = "1", Name = "Customer" }, Legal_Monetary_Totals = new() { Line_Extension_Amount = "0", Tax_Exclusive_Amount = "0", Tax_Inclusive_Amount = "0", Payable_Amount = "0" } } };
        var dtos = new List<SuccessInvoiceDto>();
        repository.Setup(x => x.GetAllInvoice(parameters)).ReturnsAsync(rows);
        mapper.Setup(x => x.Map<IEnumerable<SuccessInvoiceDto>>(rows)).Returns(dtos);

        var result = await new GetAllSuccessInvoiceHandler(repository.Object, mapper.Object)
            .Handle(new GetAllSuccessInvoiceQuery(parameters), CancellationToken.None);

        Assert.Same(dtos, result);
        repository.Verify(x => x.GetAllInvoice(parameters), Times.Once);
    }
}
