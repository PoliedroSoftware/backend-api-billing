"""Run on the new droplet deployment only. Never overwrite a populated database."""
import gzip
import hashlib
import json
from pathlib import Path
import re
import subprocess

root = Path('/opt/poliedro-billing')
assert Path.cwd().resolve() == root
mysql = ['docker', 'compose', 'exec', '-T', 'mysql', 'mysql',
         '--defaults-extra-file=/run/secrets/mysql-admin.cnf', '-N', '-B', 'billing']

def query(sql):
    result = subprocess.run(mysql, input=sql.encode(), capture_output=True, check=True)
    return result.stdout.decode().strip()

inventory = json.loads((root / 'source-inventory.json').read_text())
digest = hashlib.sha256()
with gzip.open(root / 'billing.sql.gz', 'rb') as source:
    while chunk := source.read(1024 * 1024):
        digest.update(chunk)
assert digest.hexdigest() == inventory['dump_sha256'], 'Backup checksum mismatch'
assert query("SELECT COUNT(*) FROM information_schema.tables WHERE table_schema='billing'") == '0', 'Refuse to overwrite populated database'
with gzip.open(root / 'billing.sql.gz', 'rb') as source, (root / 'restore-private.log').open('wb') as log:
    process = subprocess.Popen(mysql, stdin=subprocess.PIPE, stdout=log, stderr=log)
    while chunk := source.read(1024 * 1024):
        process.stdin.write(chunk)
    process.stdin.close()
    assert process.wait() == 0, 'Restore failed; inspect private log'
counts = {}
for name, count in inventory['counts'].items():
    assert re.fullmatch(r'[A-Za-z0-9_]+', name)
    counts[name] = int(query(f'SELECT COUNT(*) FROM `{name}`'))
    assert counts[name] == count, f'Row count mismatch: {name}'
result = {'restored': True, 'checksum_verified': True, 'counts_match': True, 'counts': counts}
(root / 'restore-verification.json').write_text(json.dumps(result, indent=2))
print(json.dumps(result))
