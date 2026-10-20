"""One-time, root-only bootstrap using private inputs in ci-bootstrap/.

Run manually after reviewing files. Never invoked by GitHub Actions.
Required inputs: receiver.py, compose.ci-candidate.yaml (in the service root),
runner-download.json, <component>-github-ed25519.pub, <component>-registration.json.
"""
import hashlib
import json
import os
from pathlib import Path
import pwd
import shutil
import subprocess
import urllib.request

ROOT = Path('/opt/poliedro-billing')
SOURCE = ROOT / 'ci-bootstrap'
assert os.geteuid() == 0 and Path.cwd().resolve() == ROOT
os.umask(0o077)
SOURCE.chmod(0o700)


def run(args, **kwargs):
    process = subprocess.run(args, capture_output=True, **kwargs)
    if process.returncode:
        (SOURCE / 'private-bootstrap-error.log').write_bytes(process.stdout + process.stderr)
        raise RuntimeError('Bootstrap failed; inspect private-bootstrap-error.log on the server')
    return process.stdout


backup = ROOT / 'backups/ci-bootstrap-20261020'
if not backup.exists():
    backup.mkdir()
    shutil.copy2(ROOT / 'compose.yaml', backup / 'compose.yaml')
    if (ROOT / '.env').exists():
        shutil.copy2(ROOT / '.env', backup / 'compose.env')
    lines = (ROOT / '.env').read_text().splitlines() if (ROOT / '.env').exists() else []
    for component, variable in [('api', 'BILLING_API_IMAGE'), ('hangfire', 'BILLING_HANGFIRE_IMAGE')]:
        image = run(['docker', 'inspect', '--format', '{{.Image}}', f'poliedro-billing-{component}-1']).decode().strip()
        lines = [line for line in lines if not line.startswith(variable + '=')] + [variable + '=' + image]
    (ROOT / '.env').write_text('\n'.join(lines) + '\n')
    shutil.copy2(ROOT / 'compose.ci-candidate.yaml', ROOT / 'compose.yaml')
run(['docker', 'compose', 'config', '-q'], cwd=ROOT)
shutil.copy2(SOURCE / 'receiver.py', '/usr/local/sbin/poliedro-ci-receiver')
os.chmod('/usr/local/sbin/poliedro-ci-receiver', 0o755)

authorized = Path('/root/.ssh/authorized_keys')
if not (backup / 'authorized_keys').exists():
    shutil.copy2(authorized, backup / 'authorized_keys')
existing = authorized.read_text()
for component in ('api', 'hangfire'):
    key = (SOURCE / (component + '-github-ed25519.pub')).read_text().strip()
    assert key.startswith('ssh-ed25519 ')
    if key.split()[1] not in existing:
        existing += f'\nrestrict,command="/usr/local/sbin/poliedro-ci-receiver {component}" {key}\n'
authorized.write_text(existing)
authorized.chmod(0o600)

metadata = json.loads((SOURCE / 'runner-download.json').read_text())
assert metadata['browser_download_url'].startswith('https://github.com/actions/runner/releases/download/')
archive = SOURCE / metadata['name']
if not archive.exists():
    with urllib.request.urlopen(metadata['browser_download_url'], timeout=60) as response, archive.open('wb') as output:
        shutil.copyfileobj(response, output)
assert hashlib.sha256(archive.read_bytes()).hexdigest() == metadata['digest'].removeprefix('sha256:')
print('Official runner archive SHA256 verified.', flush=True)

for component, repo, username in [
    ('api', 'backend-api-billing', 'ci-billing'),
    ('hangfire', 'hangfire', 'ci-hangfire'),
]:
    home = Path('/opt/poliedro-ci') / component
    try:
        user = pwd.getpwnam(username)
    except KeyError:
        run(['useradd', '--system', '--create-home', '--home-dir', str(home), '--shell', '/bin/bash', username])
        user = pwd.getpwnam(username)
    home.mkdir(parents=True, exist_ok=True)
    os.chown(home, user.pw_uid, user.pw_gid)
    home.chmod(0o700)
    if not (home / '.runner').exists():
        # Copy the verified official distribution; no code from a PR runs here.
        run(['tar', '-xzf', str(archive), '-C', str(home)])
        run(['chown', '-R', str(user.pw_uid) + ':' + str(user.pw_gid), str(home)])
        registration = json.loads((SOURCE / (component + '-registration.json')).read_text())
        run(['runuser', '-u', username, '--', str(home / 'config.sh'),
             '--unattended', '--url', 'https://github.com/PoliedroSoftware/' + repo,
             '--token', registration['token'], '--name', 'nextcloud-' + component + '-release',
             '--labels', 'poliedro-droplet-' + component, '--work', '_work'], cwd=home)
    if not (home / '.service').exists():
        run([str(home / 'svc.sh'), 'install', username], cwd=home)
    service = (home / '.service').read_text().strip()
    override = Path('/etc/systemd/system') / (service + '.d')
    override.mkdir(exist_ok=True)
    (override / 'limits.conf').write_text('[Service]\nUMask=0077\nMemoryMax=384M\nCPUQuota=60%\nNoNewPrivileges=true\nPrivateTmp=true\n')
    run(['systemctl', 'daemon-reload'])
    run(['systemctl', 'enable', '--now', service])
    run(['systemctl', 'is-active', '--quiet', service])
    print(f'{component}: isolated deployment runner started as {username}.', flush=True)

print('Bootstrap complete. Application containers, databases and proxy were not restarted.', flush=True)
