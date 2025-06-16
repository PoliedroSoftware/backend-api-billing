using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Api.Common.Extensions;
using Poliedro.Billing.Application.BillingPos.Commands.CreateBillingPos;
using Swashbuckle.AspNetCore.Annotations;

namespace Poliedro.Billing.Api.Controllers.v1.Billing;

[Route("api/v1/billing")]
[ApiController]
public class BillingController(IMediator mediator, IMockPendingInvoiceService mockPendingInvoiceService) : ControllerBase
{
    [SwaggerOperation(Summary = "Create new Billing")]
    [SwaggerResponse(StatusCodes.Status200OK, "The operation was successful.", typeof(CreateBillingPosCommand))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect request parameters.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks valid authentication credentials.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error processing the request.", typeof(ProblemDetails))]
    [HttpPost]
    public async Task<IResult> Create(
        [FromBody] CreateBillingPosCommand createBillingCommand)
    {
        var result = await mediator.Send(createBillingCommand);
        return result.Match(onSuccess => TypedResults.Ok());
    }

    [SwaggerOperation(Summary = "Get mock pending invoices")]
    [SwaggerResponse(StatusCodes.Status200OK, "List of mock invoices", typeof(List<PendingInvoice>))]
    [HttpGet("mock-pending-invoices")]
    public IActionResult GetMockPendingInvoices()
    {
        var invoices = mockPendingInvoiceService.GenerateMockInvoices();
        return Ok(invoices);
    }
}
public interface IMockPendingInvoiceService
{
    List<PendingInvoice> GenerateMockInvoices(int count = 1000);
}
public class PendingInvoice
{
    public DateTime Date { get; set; }
    public int InvoiceNumber { get; set; }
    public string Client { get; set; }
    public decimal Amount { get; set; }
}
public class MockBillingService : IMockPendingInvoiceService
{
    private static readonly Random _random = new();
    private static readonly List<string> _clients = new()
    {
        "John Doe", "Jane Smith", "Michael Johnson", "Emily Davis",
        "David Wilson", "Sarah Brown", "James Anderson", "Jessica Martinez",
        "Robert Taylor", "Laura Thomas"
    };

    public List<PendingInvoice> GenerateMockInvoices(int count = 1000)
    {
        List<PendingInvoice> invoices = new();
        DateTime startDate = DateTime.Now.AddYears(-1);
        DateTime endDate = DateTime.Now;

        for (int i = 0; i < count; i++)
        {
            invoices.Add(new PendingInvoice
            {
                Date = RandomDate(startDate, endDate),
                InvoiceNumber = _random.Next(100000, 999999),
                Client = _clients[_random.Next(_clients.Count)],
                Amount = Math.Round((decimal)(_random.NextDouble() * (5000 - 50) + 50), 2)
            });
        }

        return invoices;
    }

    private static DateTime RandomDate(DateTime start, DateTime end)
    {
        int range = (end - start).Days;
        return start.AddDays(_random.Next(range));
    }
}

