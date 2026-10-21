# Administración de Billing desde Odoo 19

El addon propio `poliedro_billing`, en `C:/Poliedro/odoo/custom-addons`, administra
el servicio Plemsi y sus licencias comerciales desde la empresa Poliedro Software.
Depende de Suscripciones Enterprise (`sale_subscription`).

## Uso

1. En Odoo, abrir **Facturación Plemsi → Estado del servicio** y comprobar conexión.
2. Preparar producto/planes (operación idempotente) y actualizar el catálogo de servidores.
3. En **Clientes y licencias**, seleccionar o crear el contacto real, su servidor de
   Billing y ambiente Plemsi. Completar NIT, vigencia, plan y precio acordado.
4. Introducir la clave Plemsi y sincronizar. El cliente/proveedor se concilia por
   NIT y empresa/servidor; las identidades ambiguas generan conflicto, no duplicados.
   La clave se elimina del campo de Odoo después del éxito y permanece en Billing.
5. Crear la cotización de suscripción, revisarla y confirmarla con el flujo nativo.
   El botón del addon únicamente crea un borrador; nunca publica facturas ni pagos.
6. Activar la licencia del cliente. Una licencia activa requiere
   fechas vigentes, suscripción confirmada/en curso del mismo cliente y empresa,
   y una línea del producto de licencia. El precio no se inventa automáticamente.

La suspensión es individual: escribe `billing_company.active=false` y deniega
la licencia en Odoo. También se puede suspender desde **Clientes de la API**, sin
crear primero una suscripción. La activación se hace desde su licencia y exige
el contrato vigente. No existe un interruptor global en Odoo.
El proceso API permanece disponible para consultas/administración. No revoca
una emisión que ya estuviera en curso. Una suscripción pausada/cerrada o vencida
también se deniega. No se interpreta automáticamente una factura impaga como una
regla comercial de suspensión; la política de cobro permanece en Suscripciones.

## Configuración de la API

Variables de entorno (nunca guardar sus valores secretos en Git):

- `BillingAdministration__Enabled=true`
- `BillingAdministration__ApiKey`: misma clave administrativa guardada en el servicio Odoo, mínimo 32 caracteres.
- `BillingAdministration__LicenseUrl`: `https://ODOO/poliedro_billing/authorize/REFERENCIA?db=BASE`

Se permite HTTP solo para loopback. El despliegue debe permitir conexiones desde
Billing hacia Odoo. La API también envía `X-Odoo-Database` con la base indicada en
la URL, necesario para el enrutamiento sin sesión de Odoo 19 con varias bases.
La autorización se solicita antes de preparar el lote y antes
de enviar cada factura FE/POS. Si la autoridad no responde, deniega; no se conserva
una autorización anterior. El modo deshabilitado conserva la compatibilidad del
despliegue anterior, pero **no ofrece control desde Odoo** y el panel lo muestra
como incompatible. Activarlo es obligatorio en la instancia administrada.

Con administración habilitada, los grupos existentes de clientes, proveedores,
servidores y resoluciones también requieren `X-Billing-Admin-Key`. Revisar cualquier
consumidor anterior de esos endpoints antes de desplegar. La ruta de emisión
conserva su contrato; agrega la comprobación de cliente/proveedor/resolución activos
y la autorización de licencia. El alcance de licencia es Plemsi FE/POS, no Siigo/TNS.

Contrato administrativo v1 protegido por `X-Billing-Admin-Key`:

- `GET /api/v1/admin/status`: identidad del servicio y autoridad de licencias configurada.
- `GET /api/v1/admin/clients`, `/providers`, `/servers`: catálogos sin credenciales.
- `POST /api/v1/admin/clients`: alta/actualización idempotente por NIT; devuelve ID.
- `POST /api/v1/admin/providers`: alta/actualización Plemsi por empresa/servidor; devuelve ID.
- `PUT /api/v1/admin/clients/{id}/state`: cambia únicamente `active` del cliente.
- `GET /api/v1/admin/automation/resolutions`: resoluciones automáticas vigentes con cliente/proveedor activos.
- `GET /api/v1/admin/automation/resolutions/{id}/eligibility`: consulta la licencia antes de leer pendientes.

No crea automáticamente una cuenta en Plemsi, una resolución DIAN ni un servidor
MySQL. Usa credenciales Plemsi y servidores existentes. Resoluciones y rangos siguen
siendo responsabilidad de la configuración de Billing; el addon controla clientes
y licencias, sin ejecutar envíos fiscales desde su interfaz administrativa.

## Despliegue actual: droplet Nextcloud y Odoo local

API y Hangfire están en `/opt/poliedro-billing` del droplet
`nextcloud-holdinglalabranza` (`143.198.12.27`). MySQL está en Oracle, sistema
`poliedroDB`, accesible por `150.136.181.118:3306/billing` con TLS.
Odoo sigue en `odoo_dev` local.
Odoo consulta Billing por HTTPS. El retorno de licencias hacia Odoo local continúa
por SSH; ver [operación y recuperación](../deploy/nextcloud/README.md).

| Servicio | Acceso desde este equipo |
|---|---|
| Odoo / Facturación Plemsi | `http://localhost:8070/odoo/action-1202` |
| API del droplet | `https://billing.holdinglalabranza.com/scalar/v1` |
| Hangfire del droplet | `https://hangfire.holdinglalabranza.com/hangfire` |

Ambos portales tienen certificados TLS válidos. Scalar/OpenAPI se muestra sin
contraseña; las operaciones de Billing requieren autenticación. Hangfire se
muestra sin contraseña en modo consulta, por solicitud del usuario.
`odoo_dev`, instancia 1 de Poliedro Software, usa
`https://billing.holdinglalabranza.com`; se verificaron estado disponible y lectura
de sus 18 clientes. Las credenciales del portal están fuera de Git, según
[el procedimiento HTTPS](../deploy/nextcloud/edge/README.md).

La empresa Poliedro Software tiene el producto de licencia y planes mensual/anual.
Se cargaron 18 clientes y 23 servidores como catálogo, sin crear ventas ni licencias
comerciales automáticamente. Hay que indicar precio, vigencia y contrato de cada
cliente para habilitar su emisión en este nuevo despliegue. Las credenciales y
resoluciones existentes conservan sus datos. Las tablas de negocio de Oracle
se conservaron; se añadieron solamente las tablas de la cola nueva.
El catálogo contiene NIT ausentes, marcadores `[NULL]` y un NIT repetido: esas
identidades requieren conciliación antes del alta de licencias. Se conservan los
datos recibidos; no se inventa un documento o contacto para resolver el conflicto.

Hangfire utiliza las tablas `PoliedroJobs*` dentro de `billing` en Oracle,
independientes del histórico `Hangfire*`. Conserva la cola del droplet y muestra
los nombres de clientes en sus trabajos. Consulta elegibilidad antes de buscar facturas; la API vuelve a
autorizar antes de cada emisión. No se usaron envíos fiscales en las pruebas.

**Este equipo debe estar encendido, con la sesión y Odoo disponibles.** Si se
interrumpe Odoo o el túnel, se deniegan nuevas emisiones. La tarea programada
`Poliedro Billing SSH Tunnel` inicia con la sesión y reconecta automáticamente.

## Lanzador de desarrollo anterior

Construir Release y usar:

```powershell
dotnet build Poliedro.Billing.sln -c Release
& C:/Poliedro/odoo/.venv/Scripts/python.exe -B tools/start_odoo_billing_local.py
& C:/Poliedro/odoo/.venv/Scripts/python.exe -B tools/verify_odoo_billing_local.py
```

El lanzador consulta la clave en PostgreSQL local y la pasa al proceso mediante
su entorno, sin imprimirla ni colocarla en argumentos. Escucha en `127.0.0.1:5068`;
`5062` pertenece al servicio de contactos EDS en este equipo. Usa las conexiones
MySQL existentes de la API: **local describe el proceso, no implica una base MySQL
aislada**. No ejecuta migraciones ni modifica clientes al arrancar.
El lanzador no instala inicio automático de Windows ni despliega a AWS.

## Comprobación

```powershell
dotnet test Poliedro.Billing.Api.Tests/Poliedro.Billing.Api.Tests.csproj -c Release
```

Pruebas significativas: autenticación, reintentos idempotentes con SQLite aislado,
respuestas sin claves, rechazo previo al emisor para cliente/proveedor/resolución
inactivos, autorización explícita y denegación ante fallo de Odoo.
Los tests del addon prueban cotización sin movimientos contables, vigencias,
suspensión offline, separación de empresas y permisos/secretos.
La imagen desplegada también se probó contra un esquema MySQL desechable:
alta y reintento sin duplicados de cliente/proveedor, lectura de `active=true/false`
y exclusión de claves en el catálogo. El esquema se eliminó al terminar.

Antes de instalar en `odoo_dev`: respaldo de base/filestore, ensayo en copia con
cron/correo/WhatsApp neutralizados y comprobación HTTP del callback. Los datos
sintéticos solo pertenecen a la copia; no deben sincronizarse al MySQL real.

## Consulta de resoluciones por cliente

`GET /api/v1/admin/clients/{id}/resolutions` exige la misma clave administrativa.
Devuelve las resoluciones FE/POS marcadas activas del cliente, incluyendo las vencidas
identificadas como tales, rangos y numeraci?n registrada en Billing. Consulta Plemsi
por prefijo y comprueba tambi?n el n?mero de resoluci?n y estado Emitted antes de
mostrar el ?ltimo consecutivo encontrado. No expone claves ni datos del comprador.
La consulta externa tiene un l?mite de seis segundos y muestra estado no disponible
o no encontrado cuando no puede confirmar el documento en la p?gina m?s reciente.
La fecha/hora es la del documento, sin asumir zona horaria ni confundir updatedAt
con la emisi?n. `date_current` se conserva como actualizaci?n hist?rica de Billing.

En Odoo, ?Ver resoluci?n y ?ltima emisi?n? abre una consulta temporal por cliente,
con actualizaci?n manual. No modifica resoluciones, numeraci?n, licencias ni facturas.
