"""Prepare private deployment artifacts outside Git. No secrets are printed."""
import configparser
import gzip
import json
from pathlib import Path
import secrets
import shutil
import tarfile
import psycopg2

repo = Path(__file__).resolve().parents[2]
root = Path('C:/Poliedro/odoo')
info = json.loads((root / 'tmp/billing-environment.json').read_text())
output = Path(info['directory']) / 'remote-deployment'
payload = output / 'payload'
payload.mkdir(exist_ok=True)
secret_dir = payload / '.secrets'
assert not secret_dir.exists(), 'Refuse to rotate an already prepared deployment implicitly'
secret_dir.mkdir()
for name in ('compose.yaml', 'Dockerfile.api', 'Dockerfile.hangfire', 'restore_and_verify.py',
             'verify_runtime.py', 'verify_mysql_admin.py', 'backup_database.py', 'configure_runtime.py'):
    shutil.copy2(repo / 'deploy/nextcloud' / name, payload / name)
(payload / '.dockerignore').write_text('.secrets\n*.sql*\n*.tar*\n', encoding='utf-8')
for kind in ('api', 'hangfire'):
    published = output / ('publish-' + kind)
    assert published.is_dir()
    destination = payload / kind
    shutil.copytree(published, destination, ignore=shutil.ignore_patterns('appsettings*', '*.pdb'))
    (destination / 'appsettings.json').write_text('{}', encoding='utf-8')

c = configparser.ConfigParser(interpolation=None)
c.read(root / 'odoo.conf', encoding='utf-8')
o = c['options']
with psycopg2.connect(host=o['db_host'], port=o['db_port'], user=o['db_user'], password=o['db_password'], dbname='odoo_dev') as db:
    with db.cursor() as cr:
        cr.execute("SELECT admin_key,callback_id FROM poliedro_billing_instance WHERE id=1")
        admin_key, callback = cr.fetchone()
root_password, api_password, jobs_password = [secrets.token_urlsafe(36) for _ in range(3)]
(secret_dir / 'mysql_root').write_text(root_password, encoding='utf-8')
(secret_dir / 'mysql-admin.cnf').write_text('[client]\nuser=root\npassword=' + root_password + '\n', encoding='utf-8')
(secret_dir / 'init.sql').write_text(
    "CREATE DATABASE IF NOT EXISTS billing CHARACTER SET utf8mb4;\n"
    "CREATE DATABASE IF NOT EXISTS billing_jobs CHARACTER SET utf8mb4;\n"
    f"CREATE USER 'billing_app'@'%' IDENTIFIED BY '{api_password}';\n"
    f"CREATE USER 'billing_jobs'@'%' IDENTIFIED BY '{jobs_password}';\n"
    "GRANT SELECT,INSERT,UPDATE,DELETE ON billing.* TO 'billing_app'@'%';\n"
    "GRANT ALL ON billing_jobs.* TO 'billing_jobs'@'%';\n", encoding='utf-8')
api_connection = f'Server=127.0.0.1;Port=33306;Database=billing;User=billing_app;Password={api_password};Pooling=True;MaximumPoolSize=30;ConnectionTimeout=10;'
job_connection = f'Server=127.0.0.1;Port=33306;Database=billing_jobs;User=billing_jobs;Password={jobs_password};Pooling=True;Max Pool Size=30;Connection Lifetime=300;Connection Timeout=5;Default Command Timeout=60;Allow User Variables=True;'
source = json.loads((repo / 'Poliedro.Billing.Api/appsettings.json').read_text(encoding='utf-8-sig'))
settings = {k: source[k] for k in ('ApiPlemsi', 'ApiPlemsiQa', 'ApiTNS', 'ApiSiigo', 'Enviroment', 'CosumerFinal')}
settings.update({'Logging': {'LogLevel': {'Default': 'Warning', 'Microsoft': 'Warning'}},
                 'AllowedHosts': '*', 'AllowedOrigins': [],
                 'ConnectionStrings': {'MysqlConnection': api_connection}})
(secret_dir / 'api-settings.json').write_text(json.dumps(settings), encoding='utf-8')
(secret_dir / 'api.env').write_text('\n'.join([
    'MYSQL_CONNECTION=' + api_connection,
    'BillingAdministration__Enabled=true', 'BillingAdministration__ApiKey=' + admin_key,
    f'BillingAdministration__LicenseUrl=http://127.0.0.1:18070/poliedro_billing/authorize/{callback}?db=odoo_dev',
    'DOTNET_GCHeapHardLimit=0x10000000', 'Logging__LogLevel__Default=Warning',
]) + '\n', encoding='utf-8')
(secret_dir / 'hangfire.env').write_text('\n'.join([
    'MYSQL_CONNECTION=' + job_connection,
    'Billing__Enabled=true', 'Billing__BaseUrl=http://127.0.0.1:15068', 'Billing__AdminKey=' + admin_key,
    'Billing__IntervalMinutes=5', 'Billing__EmissionEnabled=false', 'Hangfire__WorkerCount=2',
    'External__ClientsUrl=http://127.0.0.1:15068/health',
    'DOTNET_GCHeapHardLimit=0x08000000', 'Logging__LogLevel__Default=Warning',
]) + '\n', encoding='utf-8')
with (output / 'billing.sql').open('rb') as src, gzip.open(payload / 'billing.sql.gz', 'wb') as target:
    shutil.copyfileobj(src, target)
shutil.copy2(output / 'source-inventory.json', payload / 'source-inventory.json')
archive = output / 'billing-deploy.tar.gz'
with tarfile.open(archive, 'w:gz') as tar:
    for path in payload.iterdir():
        tar.add(path, arcname=path.name)
print(json.dumps({'archive': str(archive), 'bytes': archive.stat().st_size, 'original_appsettings_files_packaged': False}))
