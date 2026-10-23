---
name: dotnet10-best-practices
description: Use current .NET 10 and ASP.NET Core practices when building, reviewing, testing, or operating Poliedro Billing services and APIs.
---

# Buenas prácticas .NET 10 para Billing

Usa esta guía para código nuevo o cambios funcionales. Confirma las APIs y recomendaciones contra la documentación oficial enlazada; .NET y ASP.NET Core evolucionan entre versiones.

## Versión y compatibilidad

- Los proyectos principales de Billing apuntan a `net10.0`; `WorkerServiceBilling` y `Poliedro.Billing.Common` apuntan a `net8.0` y están fuera de la solución principal. Respeta el target del proyecto que editas.
- Comprueba SDKs instalados, `TargetFramework`, referencias transitivas y compatibilidad del paquete antes de usar una API nueva o subir paquetes.
- No actualices paquetes ni cambies versiones de EF/Pomelo de manera oportunista. Revisa compatibilidad de runtime, proveedor, migraciones y CI en un cambio explícito.
- Mantén nullable e implicit usings activados según los `.csproj`; elimina advertencias que introduzca tu cambio, sin convertir este trabajo en limpieza de advertencias históricas.

## ASP.NET Core y HTTP

- Sigue el patrón Minimal API del servicio. Mantén endpoints finos: binding, metadatos, dispatch a MediatR y representación HTTP.
- Declara nombres, tags, resumen y respuestas OpenAPI útiles (`Produces`/tipos de respuesta). Confirma el prefijo real y el registro manual en `Program.cs`.
- Respeta `ResultExtension.Match` y `ProblemDetails`; no filtres excepciones internas, stack traces, secretos, SQL ni datos personales al cliente.
- Propaga `CancellationToken` desde la solicitud a MediatR, I/O, HTTP y EF Core cuando las firmas lo permitan.
- Configura autenticación/autorización deliberadamente para endpoints nuevos; la presencia de un esquema Bearer en Swagger no demuestra que el endpoint esté protegido. No afirmes que un endpoint queda seguro sin probar sus políticas en ejecución.
- Evita estados mutables globales y trabajo bloqueante dentro de la solicitud. Para integraciones externas, define timeout y manejo de errores de forma explícita.

## Async, EF Core y datos

- Usa `async`/`await` para I/O y métodos EF asíncronos (`ToListAsync`, `SaveChangesAsync`, etc.) con el token de cancelación. No uses `.Result`, `.Wait()` ni `Task.Run` para envolver I/O.
- No ejecutes operaciones paralelas sobre la misma instancia de `DbContext`; cada operación debe terminar antes de iniciar la siguiente sobre ese contexto.
- Usa consultas proyectadas y acotadas; añade paginación en listados potencialmente grandes. No cargues entidades completas si el caso de uso solo necesita unos campos.
- Usa `AsNoTracking` para lecturas EF sin actualización, cuando aplique. Conserva transacciones y orden secuencial donde el flujo de facturación dependa de numeración o efectos remotos.
- No registres conexiones, tokens ni payloads sensibles. No cambies datos de Billing/DIAN en producción como parte de pruebas.

## DI, configuración y resiliencia

- Usa constructor injection y los ciclos de vida apropiados. Evita Service Locator, `BuildServiceProvider()` durante registro y Singletons que capturen servicios scoped.
- Para configuración nueva, usa Options tipadas, valida al iniciar y almacena secretos fuera del código y de `appsettings*.json` versionados.
- Reutiliza `IHttpClientFactory`/clientes tipados para integraciones HTTP nuevas; establece base address, timeout, headers y políticas de resiliencia según contrato.
- Registra servicios en extensiones de composición existentes (`AddApplication`, `AddPersistence`, `AddExternal*`) para que el wiring siga siendo verificable.

## Errores, logs y mantenimiento

- Devuelve errores de negocio mediante los tipos `Error`/`Result` existentes; usa excepciones para fallos excepcionales y no como flujo de control normal.
- Inyecta `ILogger<T>`; usa logs estructurados y datos operativos mínimos (identificadores correlacionables, proveedor, etapa). Evita PII y secretos.
- Mantén DTOs de API separados de entidades EF y tipos específicos de un proveedor. No alteres contratos existentes sin considerar los clientes actuales.
- No introduzcas dependencias nuevas para resolver problemas que el framework ya cubre.

## Pruebas y verificación

- Escribe pruebas con aserciones sobre reglas y resultados. `Poliedro.Billing.Api.Tests` usa `TestServer` para rutas HTTP y `WireMock.Net` para contratos HTTP externos; `Poliedro.Billing.Domain.Test` cubre reglas puras del dominio.
- No llames servicios reales ni emitas documentos DIAN desde pruebas. Usa base SQLite aislada o servidores HTTP simulados con puerto dinámico cuando el componente lo requiera.
- `dotnet test` verde verifica solo los caminos cubiertos: revisa qué ejecutan los tests antes de declarar validado el flujo completo de emisión.
- Para una modificación funcional, ejecuta al menos build de la solución y pruebas relevantes; si tocas `WorkerServiceBilling`, compílalo aparte porque está fuera de la solución.
- Comprueba el endpoint y su contrato de respuesta cuando sea posible. Separa fallos ambientales (por ejemplo, MySQL remoto) de fallos de código.
- Nunca uses facturas reales, credenciales reales ni emisión DIAN como mecanismo de prueba.

## Enlaces oficiales

- [.NET 10: novedades](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-10/overview)
- [ASP.NET Core Minimal APIs (.NET 10)](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-10.0)
- [Inyección de dependencias](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection)
- [Guías de inyección de dependencias](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection-guidelines)
- [Options pattern y validación](https://learn.microsoft.com/dotnet/core/extensions/options)
- [Programación asíncrona en EF Core](https://learn.microsoft.com/ef/core/miscellaneous/async)
