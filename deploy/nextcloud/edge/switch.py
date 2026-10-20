"""Controlled AIO frontend migration; retains the original containers for rollback.

Run from /opt/poliedro-edge as root: python3 switch.py apply|rollback
Only mastercontainer, Apache and the new proxy are changed.
"""
import copy
import http.client
import json
import os
from pathlib import Path
import socket
import subprocess
import sys
import time
import urllib.request

ROOT = Path('/opt/poliedro-edge')
assert Path.cwd().resolve() == ROOT
os.umask(0o077)
MASTER = 'nextcloud-aio-mastercontainer'
APACHE = 'nextcloud-aio-apache'
SUFFIX = '-before-poliedro-edge'
STATE = ROOT / 'rollback/switch-state.json'


class DockerConnection(http.client.HTTPConnection):
    def connect(self):
        self.sock = socket.socket(socket.AF_UNIX, socket.SOCK_STREAM)
        self.sock.connect('/var/run/docker.sock')


def docker(method, path, data=None, accepted=(200, 201, 204, 304)):
    connection = DockerConnection('localhost', timeout=90)
    body = json.dumps(data).encode() if data is not None else None
    connection.request(method, '/v1.44/' + path, body,
                       {'Content-Type': 'application/json'} if body else {})
    response = connection.getresponse()
    raw = response.read()
    connection.close()
    if response.status not in accepted:
        (ROOT / 'switch-private-error.log').write_bytes(raw)
        raise RuntimeError(f'Docker request failed ({response.status}); inspect private log')
    return json.loads(raw) if raw else None


def run(args):
    result = subprocess.run(args, capture_output=True)
    if result.returncode:
        (ROOT / 'switch-private-error.log').write_bytes(result.stdout + result.stderr)
        raise RuntimeError('Command failed; inspect private log')
    return result.stdout


def inspect(name):
    return docker('GET', f'containers/{name}/json')


def save(state):
    STATE.write_text(json.dumps(state, indent=2))


def stop(name):
    docker('POST', f'containers/{name}/stop?t=30')


def start(name):
    docker('POST', f'containers/{name}/start')


def rename(name, new_name):
    docker('POST', f'containers/{name}/rename?name={new_name}')


def wait_healthy(name, seconds=90):
    for _ in range(seconds):
        state = inspect(name)['State']
        if state.get('Health', {}).get('Status') == 'healthy':
            return
        if not state['Running']:
            raise RuntimeError(name + ' is not running')
        time.sleep(1)
    raise RuntimeError(name + ' health timeout')


def rollback(state):
    run(['docker', 'compose', 'stop', 'proxy'])
    # Retain failed containers as well; no volume or application data is deleted.
    for name in (APACHE, MASTER):
        if state.get(name + '_renamed'):
            current = docker('GET', f'containers/{name}/json', accepted=(200, 404))
            if current and 'Id' in current:
                docker('POST', f'containers/{name}/update', {'RestartPolicy': {'Name': 'no'}})
                stop(name)
                rename(name, name + '-edge-reverted-' + str(int(time.time())))
            rename(name + SUFFIX, name)
            original = json.loads((ROOT / 'rollback' / (name + '.json')).read_text())[0]
            docker('POST', f'containers/{name}/update', {'RestartPolicy': original['HostConfig']['RestartPolicy']})
            start(name)
    if state.get(MASTER + '_renamed'):
        run(['docker', 'cp', str(ROOT / 'apache-configure.php'), MASTER + ':/tmp/poliedro-apache-configure.php'])
        run(['docker', 'exec', MASTER, 'php', '/tmp/poliedro-apache-configure.php', 'restore'])
    state['rolled_back'] = True
    save(state)
    print('Original AIO frontend containers restored', flush=True)


def apply():
    if STATE.exists():
        previous = json.loads(STATE.read_text())
        assert previous.get('rolled_back'), 'Migration already active; inspect state before retrying'
        STATE.rename(STATE.with_name('switch-state-rolled-back-' + str(int(time.time())) + '.json'))
    assert (ROOT / '.secrets/proxy.env').exists(), 'Run preparation first'
    original = {}
    for name in (MASTER, APACHE):
        original[name] = json.loads((ROOT / 'rollback' / (name + '.json')).read_text())[0]
        current = inspect(name)
        assert current['Id'] == original[name]['Id'], 'Existing container changed since preparation'
        assert current['State']['Running'], 'Existing service is not running'
    # The AIO-managed Apache image must remain exactly the current image.
    for name in (MASTER, APACHE):
        image = docker('GET', 'images/' + original[name]['Config']['Image'] + '/json')
        assert image['Id'] == original[name]['Image']
    state = {'started_at': time.time(), 'original_ids': {n: v['Id'] for n, v in original.items()}}
    save(state)
    try:
        spec = copy.deepcopy(original[MASTER]['Config'])
        # AIO derives its child image channel from the master's image tag.
        # Keep that tag, verify its immutable ID above, and never pull during this change.
        spec['Image'] = original[MASTER]['Config']['Image']
        env = [v for v in spec['Env'] if not v.startswith(('APACHE_PORT=', 'APACHE_IP_BINDING='))]
        spec['Env'] = env + ['APACHE_PORT=11000', 'APACHE_IP_BINDING=127.0.0.1']
        spec['HostConfig'] = copy.deepcopy(original[MASTER]['HostConfig'])
        spec['HostConfig']['PortBindings'].pop('80/tcp', None)
        networks = original[MASTER]['NetworkSettings']['Networks']
        assert list(networks) == ['nextcloud-aio'], 'Review unexpected AIO networks before switching'
        spec['HostConfig']['NetworkMode'] = 'nextcloud-aio'
        spec['NetworkingConfig'] = {'EndpointsConfig': {'nextcloud-aio': {}}}
        spec['Hostname'] = MASTER
        docker('POST', f'containers/{MASTER}/update', {'RestartPolicy': {'Name': 'no'}})
        stop(MASTER)
        rename(MASTER, MASTER + SUFFIX)
        state[MASTER + '_renamed'] = True
        save(state)
        docker('POST', 'containers/create?name=' + MASTER, spec)
        start(MASTER)
        wait_healthy(MASTER)
        run(['docker', 'cp', str(ROOT / 'apache-configure.php'), MASTER + ':/tmp/poliedro-apache-configure.php'])
        run(['docker', 'exec', MASTER, 'php', '/tmp/poliedro-apache-configure.php', 'check'])
        print('AIO master ready; switching only the HTTPS frontend', flush=True)
        docker('POST', f'containers/{APACHE}/update', {'RestartPolicy': {'Name': 'no'}})
        stop(APACHE)
        rename(APACHE, APACHE + SUFFIX)
        state[APACHE + '_renamed'] = True
        save(state)
        run(['docker', 'exec', MASTER, 'php', '/tmp/poliedro-apache-configure.php', 'create'])
        assert inspect(APACHE)['Image'] == original[APACHE]['Image']
        run(['docker', 'compose', 'up', '-d', 'proxy'])
        wait_healthy(APACHE)
        opener = urllib.request.build_opener(urllib.request.ProxyHandler({}))
        with opener.open('https://nextcloud.holdinglalabranza.com/status.php', timeout=20) as response:
            status = json.load(response)
        assert status['installed'] and not status['maintenance'] and not status['needsDbUpgrade']
        # Other services must remain running and retain their current images/containers.
        state['completed'] = True
        state['completed_at'] = time.time()
        save(state)
        print(json.dumps({'migration_complete': True, 'nextcloud_status': status,
                          'rollback_containers_retained': True}))
    except Exception:
        rollback(state)
        raise


if sys.argv[1:] == ['apply']:
    apply()
elif sys.argv[1:] == ['rollback']:
    rollback(json.loads(STATE.read_text()))
else:
    raise SystemExit('Use apply or rollback')
