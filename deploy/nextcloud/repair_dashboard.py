"""Enable MySQL session variables required by Hangfire recurring-job pagination.

No database writes or job triggering. Retains the previous private environment.
"""
from datetime import datetime, timezone
import os
from pathlib import Path
import shutil
import subprocess
import time
import urllib.request

root = Path('/opt/poliedro-billing')
assert Path.cwd().resolve() == root
os.umask(0o077)
path = root / '.secrets/hangfire.env'
original = path.read_text()
lines = original.splitlines()
matches = [i for i, line in enumerate(lines) if line.startswith('MYSQL_CONNECTION=')]
assert len(matches) == 1
index = matches[0]
parts = lines[index].split('=', 1)[1].split(';')
parts = [part for part in parts if part and part.split('=', 1)[0].replace(' ', '').lower() != 'allowuservariables']
lines[index] = 'MYSQL_CONNECTION=' + ';'.join(parts) + ';Allow User Variables=True;'
updated = '\n'.join(lines) + '\n'
if updated != original:
    backup = root / 'backups' / ('hangfire-env-before-dashboard-' + datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%SZ') + '.env')
    shutil.copy2(path, backup)
    backup.chmod(0o600)
    temporary = path.with_suffix('.env.tmp')
    temporary.write_text(updated)
    temporary.chmod(0o600)
    temporary.replace(path)
    result = subprocess.run(['docker', 'compose', 'up', '-d', '--no-deps', 'hangfire'], capture_output=True)
    if result.returncode:
        (root / 'dashboard-repair-private-error.log').write_bytes(result.stdout + result.stderr)
        raise RuntimeError('Container recreation failed; restore the private environment backup')
opener = urllib.request.build_opener(urllib.request.ProxyHandler({}))
for attempt in range(30):
    try:
        with opener.open('http://127.0.0.1:15069/hangfire/recurring', timeout=5) as response:
            assert response.status == 200
            assert b'Recurring' in response.read()
        print('Hangfire recurring dashboard: HTTP 200; session variables enabled.')
        break
    except (OSError, AssertionError):
        if attempt == 29:
            raise
        time.sleep(1)
