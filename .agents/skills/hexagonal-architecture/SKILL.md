---
name: hexagonal-architecture
description: Apply Clean Architecture, hexagonal ports and adapters, and CQRS when extending Poliedro Billing API. Use for new Odoo integrations, API features, persistence, or external provider work.
---

# Arquitectura hexagonal en Poliedro Billing

Aplica esta guía al agregar o modificar casos de uso en Billing. El objetivo es mantener la lógica del caso de uso independiente de HTTP, MySQL y proveedores externos, siguiendo las convenciones reales del repositorio.

## Límites de dependencias

- `Domain` contiene modelos y contratos/puertos que representan capacidades del negocio. No debe depender de API, EF Core, MySQL, SDKs de proveedores ni detalles HTTP.
- `Application` organiza casos de uso CQRS: comandos/consultas MediatR, handlers, validadores y DTOs. Puede depender de `Domain`, nunca de los proyectos de infraestructura ni de ASP.NET Core.
- `Api` es el adaptador de entrada HTTP: Minimal APIs enlazan y validan transporte, envían el request a MediatR y convierten el resultado al contrato HTTP existente.
- `Infraestructure.Persistence.Mysql` y `Infraestructure.External.*` son adaptadores de salida: implementan puertos y traducen entre el dominio y EF/MySQL o servicios externos.
- El ensamblado de composición es `Api/Program.cs` y las extensiones `Add*` de cada proyecto. Registra puertos a adaptadores allí; no hagas resolución manual con `IServiceProvider` en handlers.
- Conserva el sentido de dependencias de los proyectos: verifica el `.csproj` antes de agregar una referencia. No agregues una referencia de `Domain` o `Application` a infraestructura.

## Flujo de una función nueva

1. Identifica el caso de uso, sus reglas y los datos/servicios que requiere. No coloques reglas de negocio en endpoints ni en repositorios.
2. Añade un `IRequest<Result<TValue, TError>>` en `Application/<Feature>/Commands` o `Queries` y su handler. Usa el `Result` y errores de dominio existentes.
3. Define en `Domain` el puerto necesario para una capacidad externa o de persistencia. Mantén la interfaz enfocada en el caso de uso; evita abstraer cada llamada mecánica sin una razón.
4. Implementa el puerto en el adaptador correspondiente y registra su ciclo de vida en la extensión `AddPersistence` o `AddExternal*` que aplique.
5. Añade un endpoint Minimal API en `Api/Endpoints/v1`, con metadatos OpenAPI coherentes. Mapea el resultado usando `ResultExtension.Match` y registra explícitamente el grupo en `Program.cs`; los prefijos de ruta no son uniformes, compruébalos antes de elegir uno.
6. Agrega validación FluentValidation en Application y registra mapas AutoMapper explícitamente donde corresponda. No presupongas escaneo de perfiles.
7. Verifica el flujo completo con tests de comportamiento útiles; los tests placeholder del repositorio no demuestran que la función opera.

## Excepciones heredadas que no se deben propagar

- La lógica de preparación/envío Plemsi vive parcialmente en `Domain`; las decisiones de arquitectura registradas la describen como un límite flojo. No traslades ni amplíes esa ubicación al crear un caso de uso nuevo.
- El adaptador Plemsi tiene dependencias y registros duplicados existentes. No copies la duplicación para Odoo.
- Los proyectos externos Siigo/TNS incluyen algunas dependencias heredadas a Application. Al tocar registro o contratos, evita añadir acoplamiento nuevo y limita el diff.
- Los nombres `Infraestructure` y algunas rutas son parte del repositorio actual; no hagas renombres masivos.
- `Poliedro.Billing.Common` está vacío y no debe referenciarse.

## Adaptador Odoo

Trata Odoo como un consumidor externo de la API. Define un contrato HTTP explícito (rutas, DTOs, autorización, errores y paginación), limita la lógica del endpoint a transporte y deja el caso de uso en Application. No permitas que modelos Odoo, JSON de transporte o detalles de autenticación invadan Domain. Nunca expongas credenciales de base de datos o proveedor en respuestas ni logs.

## Referencias internas

- `.ai/patterns.md`: patrones concretos del código.
- `.ai/decisions.md`: decisiones e inconsistencias conocidas.
- `.ai/context.md`: mapa de proyectos, configuración, endpoints y riesgos.
- `Poliedro.Billing.Api/Program.cs`: composición y grupos de rutas vigentes.

## Lecturas oficiales

- [Arquitectura de aplicaciones .NET](https://learn.microsoft.com/dotnet/architecture/modern-web-apps-azure/architectural-principles)
- [Inyección de dependencias en .NET](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection)
