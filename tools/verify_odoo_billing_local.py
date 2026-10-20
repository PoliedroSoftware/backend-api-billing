"""Read-only HTTP checks; report only statuses, identity and counts, never secrets."""
import argparse
import configparser
import json
from pathlib import Path
import psycopg2
import requests

p = argparse.ArgumentParser()
p.add_argument('--database', default='odoo_dev')
p.add_argument('--port', type=int, default=5068)
p.add_argument('--odoo-port', type=int, default=8070)
a = p.parse_args()
root = Path('C:/Poliedro/odoo')
config = configparser.ConfigParser(interpolation=None)
config.read(root / 'odoo.conf', encoding='utf-8')
opt = config['options']
with psycopg2.connect(host=opt['db_host'], port=opt['db_port'], user=opt['db_user'], password=opt['db_password'], dbname=a.database) as db:
    with db.cursor() as cr:
        cr.execute("SELECT b.admin_key,b.callback_id FROM poliedro_billing_instance b JOIN res_company c ON c.id=b.company_id WHERE c.name=%s AND b.name=%s", ('Poliedro Software', 'Poliedro Billing · Plemsi'))
        key, reference = cr.fetchone()
s = requests.Session()
s.trust_env = False
api = f'http://127.0.0.1:{a.port}'
callback = f'http://127.0.0.1:{a.odoo_port}/poliedro_billing/authorize/{reference}?db={a.database}'
result = {}
for label, method, url, headers, payload in [
    ('admin_unauthorized', 'GET', api + '/api/v1/admin/status', {}, None),
    ('admin_status', 'GET', api + '/api/v1/admin/status', {'X-Billing-Admin-Key': key}, None),
    ('api_health', 'GET', api + '/health', {}, None),
    ('callback_unauthorized', 'POST', callback, {}, {'companyId': 900001, 'companyProviderId': 900001}),
    ('callback_unknown_license', 'POST', callback, {'Authorization': 'Bearer ' + key}, {'companyId': 900001, 'companyProviderId': 900001}),
]:
    try:
        if label.startswith('callback'):
            headers['X-Odoo-Database'] = a.database
        r = s.request(method, url, headers=headers, json=payload, timeout=12, allow_redirects=False)
        data = {'http': r.status_code}
        if label == 'admin_status' and r.status_code == 200:
            data.update({k: r.json().get(k) for k in ('service', 'contractVersion', 'licenseEnforcement')})
        if label.startswith('callback') and r.status_code in (200, 400, 401):
            data.update(r.json())
        if label == 'api_health' and r.status_code in (200, 503):
            data['status'] = r.json().get('status')
        result[label] = data
    except (requests.RequestException, ValueError) as exc:
        result[label] = {'error': type(exc).__name__}
print(json.dumps(result, indent=2))
