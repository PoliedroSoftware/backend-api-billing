using Poliedro.Billing.Application.FERetail.Commands.CreateFERetail;

namespace Poliedro.Billing.Application.FERetail.Commands.CreateFERetail.Command
{
    public record Customer(
            string IdentificationNumber,
            string Dv,
            string Name,
            string Phone,
            string Address,
            string Email,
            string MerchantRegistration,
            int TypeDocumentIdentificationId,
            int TypeOrganizationId,
            int TypeLiabilityId,
            int MunicipalityId,
            string MunicipalityCode,
            int TypeRegimeId
        );
}

