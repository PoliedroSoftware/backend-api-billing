"""Read-only deployment checks. Print status and counts, never keys or invoice data."""
import json
from pathlib import Path
import urllib.error
import urllib.request
import subprocess

root = Path('/opt/poliedro-billing')
config = dict(line.split('=', 1) for line in (root / '.secrets/api.env').read_text().splitlines() if '=' in line)
key = config['BillingAdministration__ApiKey']
opener = urllib.request.build_opener(urllib.request.ProxyHandler({}))

def request(url, data=None, headers=None):
    req = urllib.request.Request(url, data=json.dumps(data).encode() if data is not None else None,
                                 headers=headers or {})
    with opener.open(req, timeout=15) as response:
        return json.load(response)

base = 'http://127.0.0.1:15068'
headers = {'X-Billing-Admin-Key': key}
status = request(base + '/api/v1/admin/status', headers=headers)
assert status['licenseEnforcement'] is True
health_api = request(base + '/health')['status']
health_hangfire = request('http://127.0.0.1:15069/health')['status']
assert health_api == health_hangfire == 'Healthy'
try:
    request(base + '/api/v1/admin/clients')
    raise AssertionError('Unauthenticated administration was allowed')
except urllib.error.HTTPError as error:
    assert error.code == 401
catalogs = {name: len(request(base + '/api/v1/admin/' + name, headers=headers)) for name in ('clients', 'providers', 'servers')}
callback = request(config['BillingAdministration__LicenseUrl'], {'companyId': 2147483647, 'companyProviderId': 2147483647},
                   {'Authorization': 'Bearer ' + key, 'X-Odoo-Database': 'odoo_dev', 'Content-Type': 'application/json'})
assert callback['allowed'] is False and callback['code'] == 'license_not_found', callback
candidates = request(base + '/api/v1/admin/automation/resolutions', headers=headers)
decisions = [request(base + f"/api/v1/admin/automation/resolutions/{row['resolutionId']}/eligibility", headers=headers) for row in candidates]
assert all(decision['allowed'] is False for decision in decisions), 'Existing customers need reviewed Odoo contracts before emission'
result = {'api': health_api, 'hangfire': health_hangfire, 'unauthenticated_admin': 401,
          'catalogs': catalogs, 'remote_to_odoo_callback': callback,
          'automation_candidates': len(candidates), 'authorized_for_emission': 0}
mysql = ['docker', 'compose', 'exec', '-T', 'mysql', 'mysql', '--defaults-extra-file=/run/secrets/mysql-admin.cnf', '-N', '-B']
def sql(query):
    return subprocess.run(mysql, input=query.encode(), capture_output=True, check=True).stdout.decode().strip()
inventory = json.loads((root / 'source-inventory.json').read_text())
for table, expected in inventory['counts'].items():
    assert table.replace('_', '').isalnum()
    assert int(sql(f'SELECT COUNT(*) FROM billing.`{table}`')) == expected, table
result['all_original_table_counts_preserved'] = True
result['hangfire_job_states'] = sql('SELECT StateName,COUNT(*) FROM billing_jobs.HangfireJob GROUP BY StateName').splitlines()
result['hangfire_recurring_jobs'] = int(sql("SELECT COUNT(*) FROM billing_jobs.HangfireSet WHERE `Key`='recurring-jobs'"))
result['jobs_emission_enabled'] = 'Billing__EmissionEnabled=true' in (root / '.secrets/hangfire.env').read_text().splitlines()
(root / 'runtime-verification.json').write_text(json.dumps(result, indent=2))
print(json.dumps(result))
