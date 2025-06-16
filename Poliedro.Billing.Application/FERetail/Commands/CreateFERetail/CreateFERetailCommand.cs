using MediatR;
using Poliedro.Billing.Application.FERetail.Commands.CreateFERetail.Command;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Application.FERetail.Commands.CreateFERetail;

public record CreateFERetailCommand(
    string Date,
    string Time,
    string Prefix,
    int Number,
    OrderReference OrderReference,
    bool SendEmail,
    Attachment Attachment1,
    Attachment Attachment2,
    Customer Customer,
    Payment Payment,
    List<GeneralAllowance> GeneralAllowances,
    List<ItemElectronicEntity> Items,
    string resolution,
    string resolutionText,
    string HeadNote,
    string FootNote,
    string Notes,
    decimal AllowanceTotal,
    decimal InvoiceBaseTotal,
    decimal InvoiceTaxExclusiveTotal,
    decimal InvoiceTaxInclusiveTotal,
    decimal TotalToPay,
    List<TaxTotal> AllTaxTotals,
    List<TaxTotal> AllHoldingsTaxTotals,
    List<CustomSubtotal> CustomSubtotals,
    decimal FinalTotalToPay
) : IRequest<Result<ApiResponseFERetailPos, Error>>;

//public record OrderReference(string IdOrder);

//public record Attachment(string Filename, string B64Data);

//public record Customer(
//    string IdentificationNumber,
//    string Dv,
//    string Name,
//    string Phone,
//    string Address,
//    string Email,
//    string MerchantRegistration,
//    int TypeDocumentIdentificationId,
//    int TypeOrganizationId,
//    int TypeLiabilityId,
//    int MunicipalityId,
//    string MunicipalityCode,
//    int TypeRegimeId
//);

//public record Payment(
//    int PaymentFormId,
//    int PaymentMethodId,
//    string PaymentDueDate,
//    string DurationMeasure
//);

//public record GeneralAllowance(
//    string AllowanceChargeReason,
//    decimal AllowancePercent,
//    decimal Amount,
//    decimal BaseAmount
//);

//public record Item(
//    int UnitMeasureId,
//    decimal LineExtensionAmount,
//    bool FreeOfChargeIndicator,
//    List<AllowanceCharge> AllowanceCharges,
//    List<TaxTotal> TaxTotals,
//    string Description,
//    string Notes,
//    string Code,
//    int TypeItemIdentificationId,
//    decimal PriceAmount,
//    decimal BaseQuantity,
//    decimal InvoicedQuantity
//);

//public record AllowanceCharge(
//    bool ChargeIndicator,
//    string AllowanceChargeReason,
//    decimal MultiplierFactorNumeric,
//    decimal Amount,
//    decimal BaseAmount
//);

//public record TaxTotal(
//    int TaxId,
//    decimal TaxAmount,
//    decimal Percent,
//    decimal TaxableAmount
//);

//public record CustomSubtotal(
//    string Concept,
//    decimal Amount
//);