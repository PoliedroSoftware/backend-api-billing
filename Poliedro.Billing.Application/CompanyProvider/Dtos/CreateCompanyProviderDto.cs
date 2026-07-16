using Poliedro.Billing.Domain.CompanyProvider.Enums;
using System;
using System.Collections.Generic;
using System.Text;
namespace Poliedro.Billing.Application.CompanyProvider.Dtos;


public record CreateCompanyProviderDto
(
    int CompanyId,
    int ProviderId,
    int ServiceId,
    string? ApiUser,
    string? ApiPassword,
    string? ApiKey,
    EnvironmentType EnvironmentType,
    string? HeadNote,
    string? FooterNote,
    int Active
);