#!/usr/bin/python3
"""Root-owned forced SSH command. Each key is restricted to one service.

Install manually as /usr/local/sbin/poliedro-ci-receiver; never replace this
receiver, Compose, proxy configuration or runtime secrets from a CI archive.
"""
import fcntl
import hashlib
import json
import os
from pathlib import Path, PurePosixPath
import re
import shutil
import subprocess
import sys
import tarfile
import time
import urllib.request

ROOT = Path('/opt/poliedro-billing')
RUNTIME = 'mcr.microsoft.com/dotnet/aspnet@sha256:d5633778c754e89a386882eddf8dbee6942447a8a4b8267c19cf1e1158d0b0ac'
SERVICES = {'api': ('Poliedro.Billing.Api.dll', 'BILLING_API_IMAGE', 15068),
            'hangfire': ('PoliedroHangFire.dll', 'BILLING_HANGFIRE_IMAGE', 15069)}


def validate_member(member):
    path = PurePosixPath(member.name)
    if (not member.isfile() or path.is_absolute() or '..' in path.parts
            or any(part.startswith('.') for part in path.parts)
            or '\\' in member.name or len(member.name) > 500):
        raise ValueError('Archive contains an unsafe entry')
    if path.name.lower().startswith('appsettings') and member.name != 'appsettings.json':
        raise ValueError('Runtime settings must remain on the droplet')
    if path.suffix.lower() in ('.pfx', '.p12', '.pem', '.key', '.env'):
        raise ValueError('Credential files are not accepted in a release')
    return path


def receive(stream, destination, expected):
    digest = hashlib.sha256()
    total = 0
    with destination.open('xb') as output:
        while chunk := stream.read(1024 * 1024):
            total += len(chunk)
            if total > 300 * 1024 * 1024:
                raise ValueError('Archive exceeds 300 MiB')
            digest.update(chunk)
            output.write(chunk)
    if digest.hexdigest() != expected:
        raise ValueError('Archive SHA256 mismatch')


def run(args, log):
    process = subprocess.run(args, cwd=ROOT, stdout=log, stderr=log, timeout=300)
    if process.returncode:
        raise RuntimeError('Deployment command failed; private details are on the droplet')


def set_image(variable, image):
    path = ROOT / '.env'
    lines = path.read_text().splitlines() if path.exists() else []
    lines = [line for line in lines if not line.startswith(variable + '=')]
    temporary = ROOT / '.env.ci-tmp'
    temporary.write_text('\n'.join(lines + [variable + '=' + image]) + '\n')
    temporary.replace(path)


def check_service(component):
    port = SERVICES[component][2]
    opener = urllib.request.build_opener(urllib.request.ProxyHandler({}))
    if component == 'api':
        settings = dict(line.split('=', 1) for line in (ROOT / '.secrets/api.env').read_text().splitlines() if '=' in line)
        req = urllib.request.Request(f'http://127.0.0.1:{port}/api/v1/admin/status',
                                     headers={'X-Billing-Admin-Key': settings['BillingAdministration__ApiKey']})
        with opener.open(req, timeout=10) as response:
            data = json.load(response)
        assert data['licenseEnforcement'] and data['contractVersion'] == 1
        with opener.open(f'http://127.0.0.1:{port}/health', timeout=10) as response:
            assert json.load(response)['status'] == 'Healthy'
    else:
        with opener.open(f'http://127.0.0.1:{port}/hangfire/recurring', timeout=10) as response:
            html = response.read()
            assert response.status == 200
            assert b'data-url="/hangfire/recurring/trigger"' not in html
            assert b'data-url="/hangfire/recurring/remove"' not in html


def wait_service(component):
    for attempt in range(30):
        try:
            check_service(component)
            return
        except Exception:
            if attempt == 29:
                raise RuntimeError('Service verification failed')
            time.sleep(2)


def main():
    os.umask(0o077)
    component = sys.argv[1] if len(sys.argv) == 2 else ''
    if component not in SERVICES:
        raise ValueError('Unknown service')
    command = os.environ.get('SSH_ORIGINAL_COMMAND', '')
    if command == 'status':
        path = ROOT / 'ci-state' / (component + '.json')
        print(path.read_text() if path.exists() else json.dumps({'component': component, 'status': 'ready'}))
        return
    match = re.fullmatch(r'deploy ([0-9a-f]{40}) ([0-9a-f]{64})', command)
    if not match:
        raise ValueError('Only deploy <commit> <archive-sha256> or status is accepted')
    revision, checksum = match.groups()
    # Both repos share this lock; deployments cannot interfere with each other.
    with (ROOT / '.ci-deploy.lock').open('a') as lock:
        fcntl.flock(lock, fcntl.LOCK_EX)
        release = ROOT / 'releases' / component / (revision + '-' + str(time.time_ns()))
        release.mkdir(parents=True)
        archive = release / 'release.tar.gz'
        receive(sys.stdin.buffer, archive, checksum)
        publish = release / 'publish'
        publish.mkdir()
        with tarfile.open(archive, 'r:gz') as tar:
            members = tar.getmembers()
            if len(members) > 10000 or sum(m.size for m in members) > 1024**3:
                raise ValueError('Archive contents exceed limits')
            names = [validate_member(member).as_posix() for member in members]
            if len(set(names)) != len(names):
                raise ValueError('Duplicate archive entries')
            tar.extractall(publish, members=members, filter='data')
        assert json.loads((publish / 'appsettings.json').read_text()) == {}
        assert json.loads((publish / 'release.json').read_text()) == {'component': component, 'revision': revision}
        entrypoint, variable, _ = SERVICES[component]
        assert (publish / entrypoint).is_file()
        image = f'poliedro-billing-{component}:{revision}'
        old_image = subprocess.check_output(['docker', 'inspect', '--format', '{{.Image}}',
                                             f'poliedro-billing-{component}-1'], text=True).strip()
        (release / 'previous-image.txt').write_text(old_image)
        (release / 'Dockerfile').write_text(
            f'FROM {RUNTIME}\nWORKDIR /app\nCOPY publish/ ./\nUSER app\n'
            f'ENTRYPOINT ["dotnet", "{entrypoint}"]\n')
        with (release / 'private-deploy.log').open('wb') as log:
            run(['docker', 'build', '--network=none', '--pull=false',
                 '--label', f'org.opencontainers.image.revision={revision}', '-t', image, str(release)], log)
            try:
                set_image(variable, image)
                run(['docker', 'compose', 'up', '-d', '--no-deps', '--no-build', component], log)
                wait_service(component)
                # Includes public TLS/docs, authentication, read-only jobs and Nextcloud.
                run(['python3', '/opt/poliedro-edge/verify.py'], log)
            except Exception:
                set_image(variable, old_image)
                run(['docker', 'compose', 'up', '-d', '--no-deps', '--no-build', component], log)
                wait_service(component)
                print('Verification failed; the previous service image was restored.', flush=True)
                raise
        state = {'component': component, 'revision': revision, 'image': image,
                 'archive_sha256': checksum, 'previous_image': old_image,
                 'verified_at': time.strftime('%Y-%m-%dT%H:%M:%SZ', time.gmtime()), 'status': 'deployed'}
        (ROOT / 'ci-state').mkdir(exist_ok=True)
        (ROOT / 'ci-state' / (component + '.json')).write_text(json.dumps(state, indent=2))
        print(json.dumps(state))


if __name__ == '__main__':
    try:
        main()
    except Exception as error:
        # Never return application responses, configuration or credentials to a CI log.
        print('Deployment rejected or rolled back: ' + type(error).__name__, file=sys.stderr)
        sys.exit(1)
