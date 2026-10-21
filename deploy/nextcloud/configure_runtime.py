"""Apply reviewed deployment settings without printing secrets."""
import argparse
from pathlib import Path
import subprocess

p = argparse.ArgumentParser()
p.add_argument('--enable-licensed-jobs', action='store_true')
args = p.parse_args()
root = Path('/opt/poliedro-billing')
assert Path.cwd().resolve() == root
path = root / '.secrets/hangfire.env'
config = path.read_text().replace('Connection Idle Timeout=60;', '')
if args.enable_licensed_jobs:
    subprocess.run(['python3', 'verify_runtime.py'], check=True)
    config = config.replace('Billing__EmissionEnabled=false', 'Billing__EmissionEnabled=true')
path.write_text(config)
path.chmod(0o600)
subprocess.run(['docker', 'compose', 'up', '-d', 'hangfire'], check=True)
cron = Path('/etc/cron.d/poliedro-billing-backup')
cron.write_text('SHELL=/bin/sh\nPATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin\n17 7 * * * root /usr/bin/python3 /opt/poliedro-billing/backup_database.py >> /opt/poliedro-billing/backup.log 2>&1\n')
cron.chmod(0o644)
print('Runtime configuration applied; private daily backup scheduled at 07:17 UTC.')
