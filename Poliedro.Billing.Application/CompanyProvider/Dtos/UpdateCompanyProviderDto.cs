using System;
using System.Collections.Generic;
using System.Text;
using Poliedro.Billing.Domain.CompanyProvider.Enums;

namespace Poliedro.Billing.Application.CompanyProvider.Dtos;

public record UpdateCompanyProviderDto
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