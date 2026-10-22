"""Run a short-lived MySQL client against Oracle; no database daemon is required."""
from pathlib import Path

ROOT = Path('/opt/poliedro-billing')


def command(tool='mysql'):
    if tool not in ('mysql', 'mysqldump'):
        raise ValueError('Unsupported MySQL tool')
    credentials = ROOT / '.secrets/oracle-mysql.cnf'
    if not credentials.is_file():
        raise RuntimeError('Private Oracle MySQL client configuration is missing')
    return ['docker', 'run', '--rm', '-i', '--network', 'host', '--memory', '128m',
            '--mount', f'type=bind,src={credentials},dst=/run/secrets/mysql-client.cnf,readonly',
            'mysql:8.0.46', tool, '--defaults-extra-file=/run/secrets/mysql-client.cnf']
