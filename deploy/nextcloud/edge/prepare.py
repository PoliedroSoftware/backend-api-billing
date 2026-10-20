"""Prepare and validate the new HTTPS edge without changing existing listeners."""
import base64
import json
import os
from pathlib import Path
import secrets
import shutil
import subprocess
import time
import urllib.error
import urllib.request

root = Path('/opt/poliedro-edge')
assert Path.cwd().resolve() == root
os.umask(0o077)
def run(args, **kwargs):
    result = subprocess.run(args, capture_output=True, **kwargs)
    if result.returncode:
        (root / 'prepare-private-error.log').write_bytes(result.stderr)
        raise RuntimeError('Preparation failed; inspect private log')
    return result.stdout

private = root / '.secrets'
assert not (private / 'proxy.env').exists(), 'Already prepared; do not implicitly rotate credentials'
private.mkdir(mode=0o700, exist_ok=True)
(root / 'rollback').mkdir(mode=0o700, exist_ok=True)
for name in ('nextcloud-aio-mastercontainer', 'nextcloud-aio-apache'):
    (root / 'rollback' / (name + '.json')).write_bytes(run(['docker', 'inspect', name]))
if not (root / 'rollback/master-config').exists():
    shutil.copytree('/var/lib/docker/volumes/nextcloud_aio_mastercontainer/_data', root / 'rollback/master-config')
source_caddy = Path('/var/lib/docker/volumes/nextcloud_aio_apache/_data/caddy')
if source_caddy.exists() and not (root / 'data/caddy').exists():
    shutil.copytree(source_caddy, root / 'data/caddy')
(root / 'config').mkdir(exist_ok=True)
image = json.loads(run(['docker', 'image', 'inspect', 'caddy:2-alpine']))[0]['RepoDigests'][0]
(root / '.env').write_text('CADDY_IMAGE=' + image + '\n')
password = secrets.token_urlsafe(30)
hashed = run(['docker', 'run', '--rm', '-i', image, 'caddy', 'hash-password', '--algorithm', 'bcrypt', '--bcrypt-cost', '12'],
             input=(password + '\n').encode()).decode().strip()
assert hashed.startswith('$2')
api_config = dict(line.split('=', 1) for line in Path('/opt/poliedro-billing/.secrets/api.env').read_text().splitlines() if '=' in line)
(private / 'proxy.env').write_text('BILLING_ADMIN_KEY=' + api_config['BillingAdministration__ApiKey'] + '\nPORTAL_PASSWORD_HASH=' + hashed + '\n')
(private / 'portal-access.txt').write_text(
    'Billing: https://billing.holdinglalabranza.com/scalar/v1\n'
    'Hangfire: https://hangfire.holdinglalabranza.com/hangfire\n'
    'Usuario: poliedro\nClave: ' + password + '\n')
run(['docker', 'compose', 'config', '-q'])
run(['docker', 'compose', 'run', '--rm', '--no-deps', 'proxy', 'caddy', 'validate', '--config', '/etc/caddy/Caddyfile'])
probe = (root / 'Caddyfile').read_text().replace('admin 127.0.0.1:2019', 'admin off')
for host in ('nextcloud', 'billing', 'hangfire'):
    probe = probe.replace(host + '.holdinglalabranza.com {', 'http://' + host + '.holdinglalabranza.com:18081 {\n    bind 127.0.0.1')
(root / 'Probe.Caddyfile').write_text(probe)
run(['docker', 'run', '-d', '--name', 'poliedro-edge-probe', '--network', 'host', '--memory', '192m',
     '--env-file', str(private / 'proxy.env'), '-v', str(root / 'Probe.Caddyfile') + ':/etc/caddy/Caddyfile:ro', image])
opener = urllib.request.build_opener(urllib.request.ProxyHandler({}))
def check(host, path, headers, expected):
    req = urllib.request.Request('http://127.0.0.1:18081' + path, headers=dict(headers, Host=host + '.holdinglalabranza.com'))
    try:
        response = opener.open(req, timeout=10)
        code = response.status
    except urllib.error.HTTPError as error:
        code = error.code
    assert code == expected, (host, path, code, expected)
try:
    for attempt in range(15):
        try:
            check('billing', '/health', {}, 200)
            break
        except urllib.error.URLError:
            if attempt == 14: raise
            time.sleep(1)
    auth = {'Authorization': 'Basic ' + base64.b64encode(('poliedro:' + password).encode()).decode()}
    check('billing', '/scalar/v1', {}, 401)
    check('billing', '/scalar/v1', auth, 200)
    check('billing', '/api/v1/admin/status', {'X-Billing-Admin-Key': api_config['BillingAdministration__ApiKey']}, 200)
    check('hangfire', '/hangfire', {}, 401)
    check('hangfire', '/hangfire', auth, 200)
finally:
    run(['docker', 'rm', '-f', 'poliedro-edge-probe'])
print(json.dumps({'prepared': True, 'image': image, 'authentication_verified': True,
                  'existing_services_changed': False, 'rollback_saved': True}))
