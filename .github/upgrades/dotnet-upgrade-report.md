# .NET 10 Upgrade Report

## Project target framework modifications

| Project name                                                    | Old Target Framework | New Target Framework | Commits                                           |
|:----------------------------------------------------------------|:--------------------:|:--------------------:|:--------------------------------------------------|
| Poliedro.Billing.Domain                                         | net8.0               | net10.0              | 7ccbbbcb, 1785d6f5                                |
| Poliedro.Billing.Application                                    | net8.0               | net10.0              | 0c45d6e3                                          |
| Poliedro.Billing.Infraestructure.Persistence.Mysql              | net8.0               | net10.0              | a49c79df, a158beaf                                |
| Poliedro.Billing.Infraestructure.External.TNS                   | net8.0               | net10.0              | 3d805178, ce8f08fc                                |
| Poliedro.Billing.Infraestructure.External.Plemsi                | net8.0               | net10.0              | f87d8b52                                          |
| Poliedro.Billing.Infraestructure.External.Siigo                 | net8.0               | net10.0              | 168e4723                                          |
| Poliedro.Billing.Domain.Test                                    | net8.0               | net10.0              | 57ebdce2                                          |
| Poliedro.Billing.Api.Tests                                      | net8.0               | net10.0              | 05ec7f58                                          |
| Poliedro.Billing.Api                                            | net8.0               | net10.0              | 44015f72                                          |

## NuGet Packages

| Package Name                                        | Old Version    | New Version | Commits                                           |
|:----------------------------------------------------|:--------------:|:-----------:|:--------------------------------------------------|
| FluentValidation.AspNetCore                         | 11.3.0         | 11.3.1      | ecf1f65e                                          |
| Microsoft.AspNetCore.Mvc                            | 2.2.0          | Removed     | e139246d, ad3ccf64, 4f271483, e4d47199, 21947f63 |
| Microsoft.EntityFrameworkCore                       | 8.0.1          | 10.0.0      | 4f271483                                          |
| Microsoft.EntityFrameworkCore.Tools                 | 8.0.1, 8.0.6   | 10.0.0      | 4f271483, ecf1f65e                                |
| Microsoft.Extensions.Configuration                  | 8.0.0, 9.0.0   | 10.0.0      | e4d47199, 21ee36c9, 21947f63                      |
| Microsoft.Extensions.Configuration.Abstractions     | 9.0.0          | 10.0.0      | e139246d                                          |
| Microsoft.Extensions.DependencyInjection.Abstractions | 8.0.1, 8.0.6 | 10.0.0      | ad3ccf64, 21ee36c9, ecf1f65e                      |
| Newtonsoft.Json                                     | 13.0.1         | 13.0.4      | 21ee36c9                                          |

## All commits

| Commit ID   | Description                                                                                           |
|:------------|:------------------------------------------------------------------------------------------------------|
| 22b82b43    | Commit upgrade plan                                                                                   |
| e139246d    | Update dependencies in Poliedro.Billing.Domain.csproj                                                 |
| 7ccbbbcb    | Update target framework to net10.0 in Domain.csproj                                                   |
| 1785d6f5    | Removed unused Newtonsoft.Json using directive                                                        |
| 0c45d6e3    | Update target framework to net10.0 in Application.csproj                                              |
| ad3ccf64    | Update dependencies in Poliedro.Billing.Application.csproj                                            |
| a49c79df    | Update target framework to net10.0 in Mysql.csproj                                                    |
| 4f271483    | Update EF Core packages to v10 in Mysql.csproj                                                        |
| 5cf8b73c    | Update dependencies in Mysql.csproj                                                                   |
| a158beaf    | Store final changes for Poliedro.Billing.Infraestructure.Persistence.Mysql                           |
| 3d805178    | Update target framework to net10.0 in TNS.csproj                                                      |
| e4d47199    | Update dependencies in Poliedro.Billing.Infraestructure.External.TNS.csproj                           |
| 631bf470    | Removed unused Newtonsoft.Json using directive in SettingsService.cs                                  |
| ce8f08fc    | Store final changes for Poliedro.Billing.Infraestructure.External.TNS                                |
| 986bfead    | Update dependencies in Plemsi.csproj                                                                  |
| 21ee36c9    | Update NuGet package versions in Plemsi.csproj                                                        |
| f87d8b52    | Update target framework to net10.0 in Plemsi.csproj                                                   |
| 21947f63    | Update package references in Siigo.csproj                                                             |
| 168e4723    | Update target framework to net10.0 in Siigo.csproj                                                    |
| bde9537e    | Replace Newtonsoft.Json with System.Text.Json in SiigoDomainService.cs                                |
| 57ebdce2    | Update target framework in Poliedro.Billing.Domain.Test.csproj                                        |
| 05ec7f58    | Update target framework in Poliedro.Billing.Api.Tests.csproj                                          |
| 44015f72    | Update target framework to net10.0 in Poliedro.Billing.Api.csproj                                     |
| dd10d3e8    | Update Poliedro.Billing.Api.csproj dependencies                                                       |
| ecf1f65e    | Update package versions in Poliedro.Billing.Api.csproj                                                |

## Project feature upgrades

### Poliedro.Billing.Domain

- Updated target framework from net8.0 to net10.0
- Upgraded Microsoft.Extensions.Configuration.Abstractions to version 10.0.0
- Removed deprecated Microsoft.AspNetCore.Mvc package
- Removed unused Newtonsoft.Json using directives from ApiResponse.cs

### Poliedro.Billing.Application

- Updated target framework from net8.0 to net10.0
- Upgraded Microsoft.Extensions.DependencyInjection.Abstractions to version 10.0.0
- Removed deprecated Microsoft.AspNetCore.Mvc package

### Poliedro.Billing.Infraestructure.Persistence.Mysql

- Updated target framework from net8.0 to net10.0
- Upgraded Microsoft.EntityFrameworkCore and Microsoft.EntityFrameworkCore.Tools to version 10.0.0
- Added Newtonsoft.Json version 13.0.4 package reference
- Removed deprecated Microsoft.AspNetCore.Mvc package
- Note: Pomelo.EntityFrameworkCore.MySql remains at version 9.0.0 as version 10.0.0 is not yet available. This causes a compatibility warning (NU1608) with EntityFrameworkCore.Relational 10.0.0, but the project compiles successfully.

### Poliedro.Billing.Infraestructure.External.TNS

- Updated target framework from net8.0 to net10.0
- Upgraded Microsoft.Extensions.Configuration to version 10.0.0
- Added Newtonsoft.Json version 13.0.4 package reference
- Removed deprecated Microsoft.AspNetCore.Mvc package
- Removed unused Newtonsoft.Json using directives from SettingsService.cs

### Poliedro.Billing.Infraestructure.External.Plemsi

- Updated target framework from net8.0 to net10.0
- Upgraded Microsoft.Extensions.Configuration to version 10.0.0
- Upgraded Microsoft.Extensions.DependencyInjection.Abstractions to version 10.0.0
- Upgraded Newtonsoft.Json from version 13.0.1 to 13.0.4

### Poliedro.Billing.Infraestructure.External.Siigo

- Updated target framework from net8.0 to net10.0
- Upgraded Microsoft.Extensions.Configuration to version 10.0.0
- Removed deprecated Microsoft.AspNetCore.Mvc package
- Replaced Newtonsoft.Json with System.Text.Json in SiigoDomainService.cs for better performance and .NET 10 compatibility

### Poliedro.Billing.Domain.Test

- Updated target framework from net8.0 to net10.0

### Poliedro.Billing.Api.Tests

- Updated target framework from net8.0 to net10.0

### Poliedro.Billing.Api

- Updated target framework from net8.0 to net10.0
- Upgraded FluentValidation.AspNetCore from version 11.3.0 to 11.3.1
- Upgraded Microsoft.EntityFrameworkCore.Tools to version 10.0.0
- Upgraded Microsoft.Extensions.DependencyInjection.Abstractions to version 10.0.0
- Removed deprecated Microsoft.AspNetCore.Mvc package (functionality included with framework)
- Note: Package KubernetesClient 15.0.1 has a known moderate severity vulnerability (GHSA-w7r3-mgwf-4mqq). Consider updating this package as a next step.

## Important Notes

### Pomelo.EntityFrameworkCore.MySql Compatibility

The Pomelo.EntityFrameworkCore.MySql package remains at version 9.0.0 as there is currently no version 10.0.0 available. This causes compatibility warnings (NU1608) with Microsoft.EntityFrameworkCore.Relational 10.0.0, but:
- All projects compile successfully
- The warnings do not prevent the application from running
- Monitor for Pomelo updates that support EF Core 10.0.0

### Security Vulnerability

The KubernetesClient package (version 15.0.1) in Poliedro.Billing.Api has a known moderate severity vulnerability. Consider updating this package to a newer, secure version as a follow-up action.

## Next steps

1. **Test the application thoroughly** to ensure all functionality works correctly with .NET 10
2. **Update KubernetesClient package** to address the security vulnerability
3. **Monitor for Pomelo.EntityFrameworkCore.MySql updates** that support EF Core 10.0.0 to eliminate compatibility warnings
4. **Review and update other outdated packages** (e.g., PDFsharp 6.1.1 → 6.2.3)
5. **Run all unit tests** to validate the upgrade
6. **Update deployment configurations** to use .NET 10 runtime
7. **Update CI/CD pipelines** to use .NET 10 SDK