"""Daily private database backup on the droplet; failures leave no completed archive."""
from datetime import datetime, timezone
import gzip
import os
from pathlib import Path
import subprocess
from mysql_client import command

os.umask(0o077)
root = Path('/opt/poliedro-billing')
os.chdir(root)
folder = root / 'backups'
folder.mkdir(mode=0o700, exist_ok=True)
stamp = datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%SZ')
temporary = folder / (stamp + '.partial')
with temporary.open('wb') as raw, gzip.GzipFile(fileobj=raw, mode='wb') as target, (folder / 'last-error.log').open('wb') as error:
    process = subprocess.Popen(command('mysqldump') + ['--single-transaction', '--no-tablespaces',
        '--set-gtid-purged=OFF', '--hex-blob', '--routines', '--triggers', '--skip-events',
        '--databases', 'billing'], stdout=subprocess.PIPE, stderr=error)
    while chunk := process.stdout.read(1024 * 1024):
        target.write(chunk)
    assert process.wait() == 0, 'Backup failed; inspect private last-error.log'
final = temporary.with_suffix('.sql.gz')
temporary.rename(final)
print('Database backup completed:', final.name, final.stat().st_size, 'bytes')
