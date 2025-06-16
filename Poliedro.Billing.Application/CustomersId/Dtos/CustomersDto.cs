

namespace Poliedro.Billing.Application.CustomersId.Dtos
{
    public record CustomersDto(
     string Id,
     string Company,
     string FullName,
     string Type,
     string Phone,
     string Email,
     List<string> AdditionalEmails,
     bool IsActive,
     DniDto Dni,
     TributaryDto Tributary,
     int __V
 );
}
