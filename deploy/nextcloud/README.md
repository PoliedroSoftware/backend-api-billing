# Billing en el droplet Nextcloud

Desplegado el 19 de octubre de 2026 en `143.198.12.27`, directorio
`/opt/poliedro-billing`. Desde el 20 de octubre comparte HTTPS con Nextcloud
mediante Caddy en `/opt/poliedro-edge`; se conservan los datos y servicios internos.
No se usa AWS ni se cambia la base de origen registrada como `billing` en DBeaver.

## Servicios y transporte

| Servicio | Dirección en el droplet | Memoria máxima |
|---|---|---|
| MySQL 8.0.46 | 127.0.0.1:33306 | 512 MiB |
| API .NET 10 | 127.0.0.1:15068 | 384 MiB |
| Hangfire .NET 10 | 127.0.0.1:15069 | 256 MiB |
| Retorno privado hacia Odoo local | 127.0.0.1:18070 → equipo local:8070 | — |

Los puertos de backend son loopback. El acceso público autenticado ahora es:

- API y documentación: https://billing.holdinglalabranza.com/scalar/v1
- Hangfire: https://hangfire.holdinglalabranza.com/hangfire

Ver [configuración HTTPS, credenciales y reversión](edge/README.md).
El archivo `tunnel.ps1` conecta por SSH los puertos
15068/15069 al equipo local y publica el retorno 18070 solo en loopback remoto.
La tarea de Windows `Poliedro Billing SSH Tunnel` inicia con la sesión, reconecta
ante cortes y usa la identidad SSH existente. No hay contraseñas en argumentos.
La documentación Scalar de Billing es pública; sus operaciones requieren
credenciales. Hangfire es visible sin contraseña
en modo consulta y bloquea las acciones de ejecutar/eliminar trabajos.
El túnel sigue siendo necesario para el retorno privado de autorización hacia Odoo local.

La API usa `billing_app`, con SELECT/INSERT/UPDATE/DELETE sobre `billing`.
Hangfire usa `billing_jobs` y una base separada del mismo nombre. No se reejecuta
el histórico de 4.957 trabajos que permanece en la copia original de `billing`.
Los trabajos nuevos guardan IDs de resolución, no claves Plemsi ni facturas.

La conexión MySQL de Hangfire requiere `Allow User Variables=True`: la consulta
de tareas recurrentes usa `@rownum`. Sin esta opción la portada puede responder
HTTP 200 mientras `/hangfire/recurring` falla con HTTP 500.
`repair_dashboard.py` aplica la opción conservando un respaldo privado y recrea
solo Hangfire. La verificación HTTPS incluye secciones del panel, CSS/JS y el POST
de consulta de estadísticas con su token CSRF.
Referencia: [opciones oficiales de MySqlConnector](https://mysqlconnector.net/connection-options/).

## Estado y operación

```powershell
Get-ScheduledTask -TaskName 'Poliedro Billing SSH Tunnel'
Start-ScheduledTask -TaskName 'Poliedro Billing SSH Tunnel'
curl.exe http://127.0.0.1:15068/health
curl.exe http://127.0.0.1:15069/health
ssh -i "$env:USERPROFILE\.ssh\id_rsa" root@143.198.12.27
```

Dentro del servidor:

```bash
cd /opt/poliedro-billing
docker compose ps
docker compose logs --tail=50 api hangfire
docker compose up -d
python3 backup_database.py
```

`verify_runtime.py` verifica salud, autenticación, retorno hacia Odoo, conteos de la
copia inicial y ausencia inicial de licencias autorizadas. Es una comprobación
de puesta en marcha: sus aserciones de conteos/licencias deben revisarse cuando
se incorporen clientes o contratos reales. `verify_mysql_admin.py` prueba altas y
activación en un esquema temporal exclusivo y elimina solo ese esquema al terminar.

`Billing__EmissionEnabled=true` está configurado tras las pruebas iniciales.
Cada resolución exige `automatic=1`, vigencia, cliente/proveedor activos y licencia
Odoo autorizada. Los cinco candidatos iniciales no tienen licencia; por tanto no
consultan pendientes ni emiten. Reconciliación y revisión de pendientes cada cinco
minutos. No hay reintentos automáticos de un lote fallido; revisar resultados del
proveedor y persistencia antes de un reintento manual. Una suspensión no revierte
una factura ya enviada. El nuevo despliegue no sustituye automáticamente otros
emisores que sigan funcionando con la base de origen.

## Respaldo y recuperación

Respaldo inicial de Odoo y MySQL en el equipo local:
`C:/Poliedro/odoo/backups/billing-20261019131515` (ACL restringida).
La copia `billing.sql` tiene SHA-256 registrado en `source-inventory.json`;
la restauración verificó la huella y los conteos de las 18 tablas.
Evidencias sin secretos: `restore-verification.json`, `runtime-verification.json`
y `mysql-admin-verification.json` en el directorio remoto.

`/etc/cron.d/poliedro-billing-backup` genera diariamente a las 07:17 UTC un dump
comprimido de `billing` y `billing_jobs`, con permisos privados, en `backups/`.
No hay borrado automático ni copia automática fuera del droplet: supervisar espacio
y copiar respaldos fuera del servidor. La copia inicial también está en el equipo local.
La base y el filestore originales de Odoo se conservaron antes de instalar.

Para recuperar, restaurar primero en otro esquema/volumen vacío y comprobar conteos;
actualizar las conexiones solo después de la verificación. `restore_and_verify.py`
rechaza expresamente una base `billing` que ya contenga tablas. No ejecutar
`docker compose down -v`: eliminaría el volumen de MySQL.

Secretos de operación en `.secrets` (directorio 0700). Los archivos montados tienen
lectura para el UID del contenedor, sin acceso a través del directorio para otros
usuarios del host. No imprimirlos, versionarlos ni incluirlos en imágenes.
`prepare_payload.py` crea un paquete privado fuera de Git, omitiendo todos los
appsettings publicados y colocando la configuración remota por montaje/env.
El paquete contiene el respaldo de datos y debe tratarse como confidencial.

## Validación realizada

- API: 17 pruebas (16 nuevas y el placeholder original); sin llamadas reales a Plemsi.
- Odoo: 8 pruebas aisladas, más callback HTTP activo/suspendido/suscripción pausada.
- Hangfire: 6 pruebas de contrato, denegación, pendientes vacíos y fallo parcial.
- MySQL real: alta y reintento de cliente/proveedor, cambio de active con lectura SQL.
- Odoo local: 121 movimientos, 31 publicados, 3 pagos, 0 ventas; sin cambios contables.
- Dependencias heredadas aún emiten advertencias de compatibilidad y seguridad al
  compilar (.NET/EF/Pomelo, AutoMapper, KubernetesClient y Prometheus). El aislamiento
  de red no sustituye su actualización; no se presenta este despliegue como auditado.
