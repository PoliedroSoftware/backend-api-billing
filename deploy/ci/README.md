# Release y despliegue en el droplet

Rama de entrega: `release/v1.0.0`. Workflow: `.github/workflows/droplet-release.yml`.

Cada push compila y prueba la solución en GitHub (`ubuntu-24.04`, SDK .NET 8 y 10).
Los pull requests ejecutan esas comprobaciones, sin acceso al droplet ni secretos
del entorno. El paquete de publicación reemplaza todos los appsettings por un
objeto vacío y excluye archivos privados. No se publica una imagen en Docker Hub
ni se utiliza AWS para esta rama.

El job de despliegue usa el entorno GitHub `droplet`, restringido a
`release/v1.0.0`, y un runner dedicado del repositorio en el droplet. Los runners
se conectan hacia GitHub por HTTPS; no se amplió la regla de entrada SSH.
Se ejecutan con usuarios separados (`ci-billing` / `ci-hangfire`), sin grupo Docker,
con directorios privados y límites de memoria. No asignarles trabajos de PR.

El runner usa SSH hacia `127.0.0.1`, con clave de host fijada y una clave distinta
por repositorio. Cada clave tiene `restrict` y un comando forzado para un único
servicio. No admite shell, forwarding ni comandos arbitrarios. Los secretos
GitHub `DROPLET_DEPLOY_KEY` y `DROPLET_KNOWN_HOSTS` están en el entorno `droplet`;
la variable `DROPLET_HOST` apunta a loopback.

## Receptor instalado

`receiver.py` se instala, mediante una sesión administrativa revisada, como
`/usr/local/sbin/poliedro-ci-receiver`. No se sustituye desde los paquetes de CI.
`bootstrap.py` registra los runners con tokens temporales obtenidos de GitHub,
verifica el SHA256 del paquete oficial y conserva la configuración previa.

El receptor:

1. Valida comando, commit, tamaño y SHA256 del archivo recibido.
2. Serializa ambos repositorios con `/opt/poliedro-billing/.ci-deploy.lock`.
3. Rechaza rutas inseguras, enlaces, secretos y configuración de producción.
4. Construye una imagen con el runtime .NET fijado por digest y etiqueta el commit.
5. Actualiza solo el servicio correspondiente con `--no-deps --no-build`.
6. Verifica el contrato administrativo y las licencias (API) o el panel de tareas
   recurrentes en solo lectura (Hangfire), más los checks HTTPS existentes.
7. Si falla después del cambio de imagen, restaura la imagen anterior y verifica
   que vuelva a responder. Si todo pasa, guarda el resultado en `ci-state/`.

MySQL, Nextcloud, Caddy, volúmenes y secretos no se recrean desde CI. La configuración
de infraestructura se mantiene en este repo y se aplica mediante mantenimiento
revisado, no desde el archivo subido por un job. El callback hacia Odoo local
conserva su túnel privado y las reglas de licencia existentes.

## Operación

En el droplet:

```bash
cd /opt/poliedro-billing
cat ci-state/api.json
cat ci-state/hangfire.json
docker compose ps
python3 /opt/poliedro-edge/verify.py
```

Los archivos `ci-state/*.json` contienen commit, imagen, hash y versión anterior,
sin credenciales. Los registros detallados de compilación/despliegue permanecen
privados en `releases/<servicio>/<commit-intento>/private-deploy.log`.

`.env` fija `BILLING_API_IMAGE` y `BILLING_HANGFIRE_IMAGE`. Para una reversión manual,
usar la imagen `previous_image` del estado del servicio, actualizar solo su variable
y ejecutar `docker compose up -d --no-deps --no-build api` (o `hangfire`), seguido
de las verificaciones. No ejecutar `down -v`.

Las imágenes y paquetes anteriores se conservan para recuperación. Supervisar el
espacio del droplet y retirar versiones antiguas solo tras decidir la retención.
GitHub conserva los artefactos de cada ejecución durante siete días.

La rama `releasecandidate/v1.0.0` y los otros destinos heredados mantienen sus
workflows anteriores. El despliegue ECS excluye expresamente `release/v1.0.0`.
No se modificó la rama predeterminada del repositorio.

Referencias: [entornos y concurrencia de GitHub Actions](https://docs.github.com/en/actions/how-tos/deploy/configure-and-manage-deployments/control-deployments).
