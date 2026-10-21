"""Deploy the reviewed read-only Hangfire build before exposing its dashboard."""
from pathlib import Path
import os
import shutil
import subprocess
import tarfile
import time
import urllib.request

root = Path('/opt/poliedro-billing')
assert Path.cwd().resolve() == root
os.umask(0o077)
backup = root / 'backups/dashboard-readonly-20261020'
assert not backup.exists(), 'Already attempted; inspect deployment before retrying'
backup.mkdir()
env = root / '.secrets/hangfire.env'
shutil.copy2(env, backup / 'hangfire.env')

def run(args):
    result = subprocess.run(args, capture_output=True)
    if result.returncode:
        (backup / 'private-error.log').write_bytes(result.stdout + result.stderr)
        raise RuntimeError('Deployment command failed; inspect private log')
    return result.stdout

old_image = run(['docker', 'inspect', '--format', '{{.Image}}', 'poliedro-billing-hangfire-1']).decode().strip()
(backup / 'image-id.txt').write_text(old_image)
run(['docker', 'tag', old_image, 'poliedro-billing-hangfire:before-readonly-20261020'])
stage = root / 'hangfire-readonly-stage'
stage.mkdir()
with tarfile.open(root / 'hangfire-readonly.tar.gz') as tar:
    tar.extractall(stage, filter='data')
for config in (root / 'hangfire').glob('appsettings*.json'):
    shutil.copy2(config, stage / config.name)
(root / 'hangfire').rename(backup / 'publish')
stage.rename(root / 'hangfire')
lines = [line for line in env.read_text().splitlines() if not line.startswith('Hangfire__DashboardReadOnly=')]
env.write_text('\n'.join(lines) + '\nHangfire__DashboardReadOnly=true\n')
run(['docker', 'compose', 'build', 'hangfire'])
run(['docker', 'compose', 'up', '-d', '--no-deps', 'hangfire'])
opener = urllib.request.build_opener(urllib.request.ProxyHandler({}))
for attempt in range(40):
    try:
        with opener.open('http://127.0.0.1:15069/hangfire/recurring', timeout=5) as response:
            html = response.read()
            assert response.status == 200
            assert b'data-url="/hangfire/recurring/trigger"' not in html
            assert b'data-url="/hangfire/recurring/remove"' not in html
        print('Read-only Hangfire deployed; recurring dashboard HTTP 200.')
        break
    except (OSError, AssertionError):
        if attempt == 39:
            raise
        time.sleep(1)
