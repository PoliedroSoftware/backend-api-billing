"""Exercise the deployed image against a new disposable MySQL schema, never billing."""
import json
from pathlib import Path
import secrets
import re
import subprocess
import time
import urllib.request

root = Path('/opt/poliedro-billing')
assert Path.cwd().resolve() == root
schema = 'billing_admin_rehearsal'
container = 'poliedro-billing-admin-rehearsal'
mysql = ['docker', 'compose', 'exec', '-T', 'mysql', 'mysql', '--defaults-extra-file=/run/secrets/mysql-admin.cnf', '-N', '-B']
def sql(query):
    result = subprocess.run(mysql, input=query.encode(), capture_output=True)
    if result.returncode:
        (root / 'rehearsal-private-error.log').write_bytes(result.stderr)
        code = re.search(rb'ERROR (\d+)', result.stderr)
        raise AssertionError('Isolated SQL setup failed, code ' + (code[1].decode() if code else 'unknown'))
    return result.stdout.decode().strip()
assert sql("SELECT COUNT(*) FROM information_schema.schemata WHERE schema_name='billing_admin_rehearsal'") == '0'
assert subprocess.run(['docker', 'container', 'inspect', container], capture_output=True).returncode != 0
password = secrets.token_urlsafe(36)
config = dict(line.split('=', 1) for line in (root / '.secrets/api.env').read_text().splitlines() if '=' in line)
connection = f'Server=127.0.0.1;Port=33306;Database={schema};User=billing_rehearsal;Password={password};Pooling=True;MaximumPoolSize=5;'
config['MYSQL_CONNECTION'] = connection
config['ASPNETCORE_URLS'] = 'http://127.0.0.1:15078'
config['ConnectionStrings__MysqlConnection'] = connection
env_file = root / '.secrets/rehearsal.env'
env_file.write_text('\n'.join(k + '=' + v for k, v in config.items()) + '\n')
env_file.chmod(0o600)
sql(f"CREATE DATABASE {schema}; CREATE USER 'billing_rehearsal'@'%' IDENTIFIED BY '{password}'; GRANT SELECT,INSERT,UPDATE,DELETE ON {schema}.* TO 'billing_rehearsal'@'%';")
created = True
try:
    for table in ('billing_company', 'company_provider', 'server', 'provider'):
        sql(f'CREATE TABLE {schema}.{table} LIKE billing.{table};')
    sql(f'INSERT INTO {schema}.server SELECT * FROM billing.server ORDER BY serverid LIMIT 1; INSERT INTO {schema}.provider SELECT * FROM billing.provider;')
    server = int(sql(f'SELECT serverid FROM {schema}.server LIMIT 1'))
    subprocess.run(['docker', 'run', '-d', '--name', container, '--network', 'host', '--memory', '192m',
        '--env-file', str(env_file), '-v', str(root / '.secrets/api-settings.json') + ':/app/appsettings.json:ro',
        'poliedro-billing-api'], check=True, capture_output=True)
    opener = urllib.request.build_opener(urllib.request.ProxyHandler({}))
    def request(path, payload=None, method=None):
        req = urllib.request.Request('http://127.0.0.1:15078' + path, method=method,
            data=json.dumps(payload).encode() if payload is not None else None,
            headers={'Content-Type': 'application/json', 'X-Billing-Admin-Key': config['BillingAdministration__ApiKey']})
        with opener.open(req, timeout=10) as response:
            return json.load(response)
    for attempt in range(20):
        try:
            assert request('/health')['status'] == 'Healthy'
            break
        except Exception:
            if attempt == 19: raise
            time.sleep(1)
    client = {'companyId': 0, 'name': 'ISOLATED DEPLOYMENT TEST', 'nit': 'TEST-ONLY', 'email': ''}
    first = request('/api/v1/admin/clients', client)
    assert first['active'] is False
    assert request('/api/v1/admin/clients', client)['companyId'] == first['companyId']
    company = first['companyId']
    provider = {'companyProviderId': 0, 'companyId': company, 'serviceId': server, 'environmentType': 0, 'apiKey': 'synthetic-not-a-plemsi-credential', 'active': False}
    first_provider = request('/api/v1/admin/providers', provider)
    assert request('/api/v1/admin/providers', provider)['companyProviderId'] == first_provider['companyProviderId']
    for enabled in (True, False):
        state = request(f'/api/v1/admin/clients/{company}/state', {'active': enabled}, 'PUT')
        assert state['active'] is enabled
        assert sql(f'SELECT active FROM {schema}.billing_company WHERE company_id={company}') == str(int(enabled))
    assert 'apiKey' not in json.dumps(request('/api/v1/admin/providers'))
    assert sql(f'SELECT COUNT(*) FROM {schema}.billing_company') == '1'
    result = {'mysql_admin_roundtrip': True, 'client_retry_no_duplicates': True, 'provider_retry_no_duplicates': True,
              'active_true_false_readback': True, 'credential_not_returned': True, 'billing_customers_untouched': True}
    (root / 'mysql-admin-verification.json').write_text(json.dumps(result, indent=2))
    print(json.dumps(result))
finally:
    subprocess.run(['docker', 'rm', '-f', container], capture_output=True)
    # These exact resources were created above only after asserting they did not exist.
    sql(f"DROP DATABASE {schema}; DROP USER 'billing_rehearsal'@'%';")
    env_file.unlink()
