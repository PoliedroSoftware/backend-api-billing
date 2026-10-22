# Billing y Hangfire: droplet con MySQL en Oracle

La API y Hangfire se ejecutan en `/opt/poliedro-billing` del droplet
`nextcloud-holdinglalabranza` (`143.198.12.27`). MySQL se mantiene en Oracle Cloud,
cuenta `poliedrosoftware`, regi?n `us-ashburn-1`, sistema `poliedroDB`.
El balanceador `lbmysql` publica `150.136.181.118:3306` hacia `10.0.1.13:3306`.

## Conexiones actuales

- API: `https://billing.holdinglalabranza.com/scalar/v1`.
- Hangfire: `https://hangfire.holdinglalabranza.com/hangfire/recurring`.
- Ambos servicios usan `150.136.181.118:3306/billing` con TLS obligatorio.
- Hangfire usa `Hangfire__TablesPrefix=PoliedroJobs`: sus 12 tablas se mantienen
  separadas de las tablas hist?ricas `Hangfire*` de Billing. No conectar el
  trabajador nuevo al prefijo hist?rico, pues podr?a reactivar trabajos antiguos.
- Cada trabajo muestra el nombre del cliente y su resoluci?n. El ID interno
  permanece estable para que un cambio de nombre no duplique la programaci?n.
- Odoo sigue en `odoo_dev` local. El retorno de autorizaci?n usa el t?nel SSH
  hacia `127.0.0.1:18070`. Sin Odoo disponible se deniegan nuevas emisiones.

El firewall de DigitalOcean permite TCP saliente solamente hacia
`150.136.181.118/32:3306` para esta conexi?n. La lista de seguridad p?blica del
balanceador Oracle ya permit?a 3306; no se a?adieron reglas redundantes all?.

`compose.yaml` contiene solamente API y Hangfire. No crea un servidor MySQL ni
un volumen de base de datos local. Las im?genes y la configuraci?n privada
siguen fuera de los artefactos de aplicaci?n; CI conserva esos ajustes.

## Operaci?n

En el droplet:

```bash
cd /opt/poliedro-billing
docker compose ps
python3 verify_runtime.py
python3 /opt/poliedro-edge/verify.py
python3 backup_database.py
```

`mysql_client.py` usa un contenedor de cliente MySQL de corta duraci?n, con un
m?ximo de 128 MiB y sin volumen de datos. Lee `.secrets/oracle-mysql.cnf`, privado
y con `ssl-mode=REQUIRED`. No se coloca la contrase?a en argumentos ni en Git.

El respaldo diario de las 07:17 UTC incluye la base `billing` de Oracle y por
lo tanto tambi?n las tablas `PoliedroJobs*`. Se guarda comprimido y con permisos
privados en `backups/`. No hay transferencia autom?tica fuera del servidor ni
borrado autom?tico de archivos: supervisar su espacio y retenci?n.

`verify_runtime.py` consulta salud de API/Hangfire, autenticaci?n, identidad del
MySQL remoto, TLS, cat?logo de clientes, cola con su prefijo y callback Odoo.
Su aserci?n de cero licencias autorizadas corresponde a la puesta en marcha;
revisarla al habilitar contratos comerciales reales.

## Migraci?n y recuperaci?n

La migraci?n de octubre de 2026 conserva respaldos en el equipo local:
`C:/Poliedro/odoo/backups/billing-migration-20261020/oracle-cutover`.
La evidencia privada de operaci?n y configuraci?n anterior est? en
`/opt/poliedro-billing/backups/oracle-cutover-20261021`.

Antes de retirar el servidor local se detuvieron los trabajadores, se copiaron
sus 12 tablas al prefijo nuevo y se compararon conteos y sumas de comprobaci?n.
El campo FLOAT de programaci?n `HangfireSet.Score` se conserv? mediante valores
decimales exactos porque su representaci?n en mysqldump puede redondearse.
Las tablas de negocio y el hist?rico original en Oracle no se sobrescribieron.

Los scripts `prepare_payload.py`, `restore_and_verify.py` y `configure_runtime.py`
pertenecen al despliegue inicial con MySQL local. No ejecutarlos sobre esta
instalaci?n para restaurar ni cambiar las conexiones actuales. Para recuperar,
restaurar primero el respaldo en un destino aislado, verificarlo y actualizar
las conexiones privadas despu?s. El volumen local retirado se recupera desde
los respaldos, no mediante `docker compose up`.

## Acceso y facturaci?n

Scalar/OpenAPI y el panel Hangfire son p?blicos por solicitud del usuario.
La administraci?n/escritura de Billing exige credenciales. Hangfire est? en
modo consulta y bloquea ejecutar/eliminar trabajos desde el panel.
Cada emisi?n exige cliente y resoluci?n activos y una licencia v?lida en Odoo.
Ver [m?dulo Odoo](../../docs/ODOO_BILLING.md) y
[proxy HTTPS](edge/README.md). No se emiten documentos fiscales para verificar
un despliegue.
