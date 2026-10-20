# HTTPS para Billing, Hangfire y Nextcloud

Aplicado y verificado el 20 de octubre de 2026 en `143.198.12.27`.

| Sitio | Backend privado | Acceso |
|---|---|---|
| https://billing.holdinglalabranza.com/scalar/v1 | 127.0.0.1:15068 | Documentación pública; operaciones autenticadas |
| https://hangfire.holdinglalabranza.com/hangfire | 127.0.0.1:15069 | Público, solo lectura |
| https://nextcloud.holdinglalabranza.com | 127.0.0.1:11000 | Autenticación propia de Nextcloud |

En Namecheap se añadieron solamente los registros A `billing` y `hangfire`,
ambos a `143.198.12.27`, TTL automático. Los registros anteriores, MX y
nameservers se conservaron.

## Accesos

Hangfire se muestra sin contraseña por solicitud del usuario. El navegador
integrado mostraba una página vacía al recibir el desafío de Basic Auth.
Se habilitó `Hangfire__DashboardReadOnly=true` y el proxy permite únicamente GET/HEAD
del panel y POST de `/hangfire/stats`, que solo consulta estadísticas.
Las solicitudes de ejecutar/eliminar trabajos devuelven 403 en el proxy y
también están bloqueadas por Hangfire. No se modificó la programación automática.

Scalar, su JavaScript y `/swagger/v1/swagger.json` permiten GET/HEAD sin contraseña.
La raíz redirige a Scalar. Así el navegador integrado carga la documentación sin
necesitar el cuadro de Basic Auth. Las rutas operativas mantienen autenticación:
sin credenciales, GET administrativo y POST de creación son rechazados con 401.

Usuario del acceso Basic a operaciones Billing: `poliedro`. La contraseña generada está en:

- Servidor: `/opt/poliedro-edge/.secrets/portal-access.txt`, privado para root.
- Equipo autorizado: `C:/Poliedro/odoo/backups/billing-20261019131515/remote-deployment/portal-access.txt`,
  bajo la ACL privada de respaldos (usuario, SYSTEM y administradores).

No incorporar estos archivos ni `.secrets`, `.env`, `data`, `config` o `rollback`
del servidor a Git. La clave administrativa de la API es diferente a la contraseña
del portal. Las solicitudes con el encabezado administrativo válido pasan al
backend, que vuelve a comprobarlo. `/health` de Billing permite GET anónimo para el
monitor de Odoo. Las solicitudes operativas de Billing requieren autenticación.

Odoo local usa la nueva URL HTTPS. El retorno de autorización de licencias sigue
por el túnel SSH a `127.0.0.1:18070`: que un panel sea accesible no autoriza emisión
ni elimina la dependencia del Odoo local.

## Infraestructura

Caddy en `/opt/poliedro-edge`, Compose `poliedro-edge`, red host, límite 192 MiB.
Imagen verificada:
`caddy@sha256:d8542f48d34a9cf4e4c11a478865229840e87e4c96ea3f439101f31a5d35f75f`.
Escucha en 80/443, redirige a HTTPS y administra los certificados automáticamente.
Conservar `data/` para certificados/cuentas ACME y `config/`.

Se siguió el modo de [proxy externo de Nextcloud AIO](https://github.com/nextcloud/all-in-one/blob/main/reverse-proxy.md):
master con `APACHE_PORT=11000`, `APACHE_IP_BINDING=127.0.0.1`, sin publicar 80.
Los puertos administrativos existentes 8080/8443 se conservaron.
Apache fue recreado por el administrador de contenedores del propio AIO.
Los contenedores de datos, Nextcloud, Redis, Office y demás servicios permanecieron
ejecutándose. No se editaron fuentes del proveedor ni se eliminaron volúmenes.

Se conservaron las imágenes originales de master y Apache; no se descargaron
actualizaciones durante el cambio. AIO deduce el canal de imágenes desde el tag
del master, por lo que este debe conservar el tag original comprobado contra su ID,
en lugar de reemplazarlo por `sha256:...`.

## Verificación y operación

```bash
cd /opt/poliedro-edge
docker compose ps
python3 verify.py
docker compose logs --tail=50 proxy
```

`verify.py` verifica certificados y nombres TLS, documentación Scalar/OpenAPI
pública, HTTP 401 en consultas y escrituras administrativas anónimas de Billing,
HTTP 200 con su clave administrativa, acceso público a Hangfire, estadísticas y bloqueo de
acciones de escritura, contrato administrativo de Billing,
salud de MySQL a través de la API, login/estado de Nextcloud y protección WebDAV.
El informe sin secretos se guarda en `verification.json`.
También se comprobó la carga de archivos de Nextcloud en la sesión existente
del navegador y el acceso de Odoo a sus 18 clientes por el nuevo endpoint.

Para cambios de Caddyfile, validar antes de recargar:

```bash
docker compose exec proxy caddy validate --config /etc/caddy/Caddyfile
docker compose exec proxy caddy reload --config /etc/caddy/Caddyfile
```

La [autenticación Basic de Caddy](https://caddyserver.com/docs/caddyfile/directives/basic_auth)
usa un hash bcrypt, y el archivo de entorno de Compose usa formato `raw` para
preservar los caracteres `$`. No imprimir la configuración adaptada: contiene
la clave del encabezado administrativo.

## Respaldo y reversión

`prepare.py` guarda en `rollback/` los inspect originales y la configuración
del master AIO antes de modificar listeners. Copia los certificados existentes
a `data/caddy` y comprueba la autenticación con un listener temporal solo local.
No ejecutarlo de nuevo para rotar contraseñas.

`switch.py` conserva los contenedores originales detenidos, con sufijo
`-before-poliedro-edge` y reinicio automático deshabilitado. Guarda pasos y
resultado en `rollback/switch-state.json`. Una primera tentativa fue revertida
correctamente al detectar la deducción del canal AIO; su estado quedó archivado.
Los contenedores de intentos revertidos también quedan detenidos sin reinicio.

Revertir únicamente esta entrada HTTPS:

```bash
cd /opt/poliedro-edge
python3 switch.py rollback
```

Esto detiene el proxy, devuelve los nombres y políticas de reinicio originales,
inicia master/Apache anteriores y restaura el puerto AIO 443. No elimina datos.
Los enlaces Billing/Hangfire dejan de estar disponibles por HTTPS; permanecen los
puertos privados del túnel. Reponer el endpoint de Odoo a `http://127.0.0.1:15068`
mediante su configuración si se revierte. El valor previo está respaldado en
`remote-deployment/odoo-endpoint-before-https.json` del equipo local.

Conservar copia privada fuera del droplet de `.secrets`, `data`, `config`,
`rollback`, `.env`, Caddyfile y Compose; nunca usar `down -v` ni eliminar los
volúmenes de Nextcloud para revertir este cambio.
