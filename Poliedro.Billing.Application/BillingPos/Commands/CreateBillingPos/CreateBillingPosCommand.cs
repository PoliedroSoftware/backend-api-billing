using MediatR;
using Poliedro.Billing.Domain.BillingPos;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;

namespace Poliedro.Billing.Application.BillingPos.Commands.CreateBillingPos;

public record CreateBillingPosCommand(
    int Number,
    string Date,
    string Time,
    SoftwareManufacturerEntity SoftwareManufacturer,
    string Resolution,
    string Prefix,
    string HeadNote,
    string FootNote,
    PayPointInfoEntity PayPointInfo,
    PaymentPosEntity Payment,
    string InvoiceBaseTotal,
    string InvoiceTaxExclusiveTotal,
    string InvoiceTaxInclusiveTotal,
    string TotalToPay,
    List<TaxTotalPosEntity> AllTaxTotals,
    List<ItemFERetailEntity> Items
    ) : IRequest<Result<ApiResponseBillingPos, Error>>;




