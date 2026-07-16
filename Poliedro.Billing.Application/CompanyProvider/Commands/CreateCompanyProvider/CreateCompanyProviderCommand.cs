using System;
using System.Collections.Generic;
using System.Text;

using MediatR;
using Poliedro.Billing.Application.CompanyProvider.Dtos;

namespace Poliedro.Billing.Application.CompanyProvider.Commands.CreateCompanyProvider;

public record CreateCompanyProviderCommand(CreateCompanyProviderDto Data) : IRequest<CompanyProviderDto>;
