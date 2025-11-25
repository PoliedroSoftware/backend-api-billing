# .NET 10 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 10 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10 upgrade.
3. Upgrade Poliedro.Billing.Domain\Poliedro.Billing.Domain.csproj
4. Upgrade Poliedro.Billing.Application\Poliedro.Billing.Application.csproj
5. Upgrade Poliedro.Billing.Infraestructure.Persistence.Mysql\Poliedro.Billing.Infraestructure.Persistence.Mysql.csproj
6. Upgrade Poliedro.Billing.Infraestructure.External.TNS\Poliedro.Billing.Infraestructure.External.TNS.csproj
7. Upgrade Poliedro.Billing.Infraestructure.External.Plemsi\Poliedro.Billing.Infraestructure.External.Plemsi.csproj
8. Upgrade Poliedro.Billing.Infraestructure.External.Siigo\Poliedro.Billing.Infraestructure.External.Siigo.csproj
9. Upgrade Poliedro.Billing.Domain.Test\Poliedro.Billing.Domain.Test.csproj
10. Upgrade Poliedro.Billing.Api.Tests\Poliedro.Billing.Api.Tests.csproj
11. Upgrade Poliedro.Billing.Api\Poliedro.Billing.Api.csproj

## Settings

This section contains settings and data used by execution steps.

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                                        | Current Version | New Version | Description                                                      |
|:----------------------------------------------------|:---------------:|:-----------:|:-----------------------------------------------------------------|
| FluentValidation.AspNetCore                         | 11.3.0          | 11.3.1      | Package is deprecated and should be updated                      |
| Microsoft.AspNetCore.Mvc                            | 2.2.0           |             | Deprecated package - functionality included with framework or replace with 2.3.0 |
| Microsoft.EntityFrameworkCore                       | 8.0.1           | 10.0.0      | Recommended for .NET 10                                          |
| Microsoft.EntityFrameworkCore.Tools                 | 8.0.1; 8.0.6    | 10.0.0      | Recommended for .NET 10                                          |
| Microsoft.Extensions.Configuration                  | 8.0.0; 9.0.0    | 10.0.0      | Recommended for .NET 10                                          |
| Microsoft.Extensions.Configuration.Abstractions     | 9.0.0           | 10.0.0      | Recommended for .NET 10                                          |
| Microsoft.Extensions.DependencyInjection.Abstractions | 8.0.1; 8.0.6  | 10.0.0      | Recommended for .NET 10                                          |
| Newtonsoft.Json                                     | 13.0.1          | 13.0.4      | Recommended for .NET 10                                          |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### Poliedro.Billing.Domain modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Configuration.Abstractions should be updated from `9.0.0` to `10.0.0` (*recommended for .NET 10*)
  - Microsoft.AspNetCore.Mvc should be removed or replaced with version `2.3.0` (*deprecated package*)

#### Poliedro.Billing.Application modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.DependencyInjection.Abstractions should be updated from `8.0.1` to `10.0.0` (*recommended for .NET 10*)
  - Microsoft.AspNetCore.Mvc should be removed or replaced with version `2.3.0` (*deprecated package*)

#### Poliedro.Billing.Infraestructure.Persistence.Mysql modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore should be updated from `8.0.1` to `10.0.0` (*recommended for .NET 10*)
  - Microsoft.EntityFrameworkCore.Tools should be updated from `8.0.1` to `10.0.0` (*recommended for .NET 10*)
  - Microsoft.AspNetCore.Mvc should be removed or replaced with version `2.3.0` (*deprecated package*)

#### Poliedro.Billing.Infraestructure.External.TNS modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Configuration should be updated from `8.0.0` to `10.0.0` (*recommended for .NET 10*)
  - Microsoft.AspNetCore.Mvc should be removed or replaced with version `2.3.0` (*deprecated package*)

#### Poliedro.Billing.Infraestructure.External.Plemsi modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Configuration should be updated from `9.0.0` to `10.0.0` (*recommended for .NET 10*)
  - Microsoft.Extensions.DependencyInjection.Abstractions should be updated from `8.0.1` to `10.0.0` (*recommended for .NET 10*)
  - Newtonsoft.Json should be updated from `13.0.1` to `13.0.4` (*recommended for .NET 10*)

#### Poliedro.Billing.Infraestructure.External.Siigo modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Configuration should be updated from `8.0.0` to `10.0.0` (*recommended for .NET 10*)
  - Microsoft.AspNetCore.Mvc should be removed or replaced with version `2.3.0` (*deprecated package*)

#### Poliedro.Billing.Domain.Test modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### Poliedro.Billing.Api.Tests modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### Poliedro.Billing.Api modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore.Tools should be updated from `8.0.6` to `10.0.0` (*recommended for .NET 10*)
  - Microsoft.Extensions.DependencyInjection.Abstractions should be updated from `8.0.6` to `10.0.0` (*recommended for .NET 10*)
  - FluentValidation.AspNetCore should be updated from `11.3.0` to `11.3.1` (*deprecated package*)
  - Microsoft.AspNetCore.Mvc functionality is included with framework reference (*remove package*)
