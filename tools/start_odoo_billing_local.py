"""Start Billing on loopback using the key held in local Odoo, never command-line secrets.

Run with C:/Poliedro/odoo/.venv/Scripts/python.exe. Build Release first.
No billing/customer writes are performed by this launcher.
"""
import argparse
import configparser
import json
import os
from pathlib import Path
import subprocess
import socket
import psycopg2

parser = argparse.ArgumentParser()
parser.add_argument('--database', default='odoo_dev')
parser.add_argument('--port', type=int, default=5068)
parser.add_argument('--odoo-port', type=int, default=8070)
args = parser.parse_args()
assert args.database == 'odoo_dev' or args.database.startswith('odoo_billing_test_')
assert args.port in (5068, 15068) and args.odoo_port in (8070, 18079)
with socket.socket() as check:
    assert check.connect_ex(('127.0.0.1', args.port)) != 0, 'The chosen port already has a running service'
root = Path('C:/Poliedro/odoo')
repo = Path(__file__).resolve().parents[1]
config = configparser.ConfigParser(interpolation=None)
config.read(root / 'odoo.conf', encoding='utf-8')
opt = config['options']
with psycopg2.connect(host=opt['db_host'], port=opt['db_port'], user=opt['db_user'], password=opt['db_password'], dbname=args.database) as db:
    with db.cursor() as cr:
        cr.execute("SELECT b.admin_key,b.callback_id FROM poliedro_billing_instance b JOIN res_company c ON c.id=b.company_id WHERE c.name=%s AND b.name=%s", ('Poliedro Software', 'Poliedro Billing · Plemsi'))
        rows = cr.fetchall()
assert len(rows) == 1, 'Configure exactly one local Poliedro Billing instance first'
key, reference = rows[0]
process_env = dict(os.environ,
    ASPNETCORE_URLS=f'http://127.0.0.1:{args.port}',
    ASPNETCORE_ENVIRONMENT='Production',
    BillingAdministration__Enabled='true', BillingAdministration__ApiKey=key,
    BillingAdministration__LicenseUrl=f'http://127.0.0.1:{args.odoo_port}/poliedro_billing/authorize/{reference}?db={args.database}',
    Logging__LogLevel__Default='Warning', Logging__LogLevel__Microsoft='Warning')
log_path = root / 'tmp' / f'billing-api-{args.port}.log'
with log_path.open('ab') as log:
    process = subprocess.Popen(['dotnet', str(repo / 'Poliedro.Billing.Api/bin/Release/net10.0/Poliedro.Billing.Api.dll')],
                               cwd=repo / 'Poliedro.Billing.Api', env=process_env, stdout=log, stderr=log,
                               creationflags=subprocess.CREATE_NO_WINDOW)
info = {'pid': process.pid, 'port': args.port, 'odoo_database': args.database, 'log': str(log_path)}
(root / 'tmp' / f'billing-api-{args.port}.json').write_text(json.dumps(info), encoding='utf-8')
print(json.dumps(info))
