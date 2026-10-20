"""Read the Billing connection already configured in the API; verify DBeaver identity.
Never prints or stores passwords. A dump contains confidential application data.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import subprocess
import pymysql

REPO = Path(__file__).resolve().parents[2]

def connection():
    settings = json.loads((REPO / 'Poliedro.Billing.Api/appsettings.json').read_text(encoding='utf-8-sig'))
    value = settings['ConnectionStrings']['MysqlConnection']
    pairs = {}
    for part in value.split(';'):
        if not part.strip():
            continue
        key, val = part.split('=', 1)
        pairs[key.strip().lower()] = val.strip().strip('"')
    host = pairs.get('server') or pairs.get('host')
    database = pairs.get('database')
    path = Path(os.environ['APPDATA']) / 'DBeaverData/workspace6/General/.dbeaver/data-sources.json'
    sources = json.loads(path.read_text(encoding='utf-8'))['connections']
    matches = [v for v in sources.values() if v.get('name') == 'billing'
               and v.get('configuration', {}).get('host') == host
               and v.get('configuration', {}).get('database') == database]
    assert len(matches) == 1 and database == 'billing', 'DBeaver/API identity mismatch'
    return dict(host=host, port=int(pairs.get('port', 3306)), database=database,
                user=pairs.get('user id') or pairs.get('uid') or pairs.get('user'),
                password=pairs.get('password') or pairs.get('pwd'),
                charset='utf8mb4', connect_timeout=10)

def main():
    p = argparse.ArgumentParser()
    p.add_argument('--output', required=True)
    p.add_argument('--dump', action='store_true')
    args = p.parse_args()
    output = Path(args.output).resolve()
    assert output.is_relative_to(Path('C:/Poliedro/odoo/backups').resolve())
    output.mkdir(parents=True, exist_ok=True)
    cfg = connection()
    db = pymysql.connect(**cfg)
    with db.cursor() as cr:
        cr.execute('SELECT VERSION()')
        version = cr.fetchone()[0]
        cr.execute("SELECT TABLE_NAME,TABLE_TYPE,ENGINE,TABLE_ROWS,DATA_LENGTH+INDEX_LENGTH FROM information_schema.TABLES WHERE TABLE_SCHEMA=%s ORDER BY TABLE_NAME", (cfg['database'],))
        tables = [{'name': r[0], 'type': r[1], 'engine': r[2], 'estimated_rows': r[3], 'bytes': r[4]} for r in cr.fetchall()]
        counts = {}
        for table in tables:
            name = table['name']
            assert re.fullmatch(r'[A-Za-z0-9_]+', name)
            if table['type'] == 'BASE TABLE':
                cr.execute(f'SELECT COUNT(*) FROM `{name}`')
                counts[name] = cr.fetchone()[0]
    db.close()
    evidence = {'source': 'DBeaver: billing', 'version': version, 'tables': tables, 'counts': counts}
    (output / 'source-inventory.json').write_text(json.dumps(evidence, indent=2), encoding='utf-8')
    if args.dump:
        dump = output / 'billing.sql'
        cmd = ['C:/Program Files/MySQL/MySQL Server 8.4/bin/mysqldump.exe', '--host=' + cfg['host'],
               '--port=' + str(cfg['port']), '--user=' + cfg['user'], '--single-transaction', '--skip-lock-tables',
               '--no-tablespaces', '--set-gtid-purged=OFF', '--column-statistics=0', '--hex-blob',
               '--routines', '--triggers', '--skip-events', '--result-file=' + str(dump), cfg['database']]
        result = subprocess.run(cmd, env=dict(os.environ, MYSQL_PWD=cfg['password']), capture_output=True)
        if result.returncode:
            # Tool errors do not include a supplied password, but keep details in private evidence.
            (output / 'dump-error.log').write_bytes(result.stderr)
            raise RuntimeError('Database dump failed; inspect private log')
        evidence['dump_bytes'] = dump.stat().st_size
        evidence['dump_sha256'] = hashlib.sha256(dump.read_bytes()).hexdigest()
        (output / 'source-inventory.json').write_text(json.dumps(evidence, indent=2), encoding='utf-8')
    print(json.dumps({'version': version, 'counts': counts, 'dump_bytes': evidence.get('dump_bytes')}))

if __name__ == '__main__':
    main()
