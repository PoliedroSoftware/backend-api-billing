"""Read-only public TLS/authentication and Nextcloud checks; never print credentials."""
import http.cookiejar
import json
from pathlib import Path
import re
import ssl
import socket
import urllib.error
import urllib.parse
import urllib.request

root = Path('/opt/poliedro-edge')
key = dict(line.split('=', 1) for line in (root / '.secrets/proxy.env').read_text().splitlines())['BILLING_ADMIN_KEY']
machine = {'X-Billing-Admin-Key': key}
opener = urllib.request.build_opener(urllib.request.ProxyHandler({}),
                                    urllib.request.HTTPCookieProcessor(http.cookiejar.CookieJar()))
results = {}
for label, host, path, headers, expected in [
    ('billing_denies_anonymous_admin', 'billing', '/api/v1/admin/status', {}, 401),
    ('billing_docs', 'billing', '/scalar/v1', {}, 200),
    ('billing_docs_javascript', 'billing', '/scalar/scalar.js', {}, 200),
    ('billing_openapi', 'billing', '/swagger/v1/swagger.json', {}, 200),
    ('billing_admin', 'billing', '/api/v1/admin/status', machine, 200),
    ('billing_health', 'billing', '/health', {}, 200),
    ('hangfire_dashboard', 'hangfire', '/hangfire', {}, 200),
    ('hangfire_recurring', 'hangfire', '/hangfire/recurring', {}, 200),
    ('hangfire_enqueued', 'hangfire', '/hangfire/jobs/enqueued', {}, 200),
    ('hangfire_servers', 'hangfire', '/hangfire/servers', {}, 200),
    ('nextcloud_status', 'nextcloud', '/status.php', {}, 200),
    ('nextcloud_login', 'nextcloud', '/login', {}, 200),
    ('nextcloud_webdav_protected', 'nextcloud', '/remote.php/dav/', {}, 401),
]:
    request = urllib.request.Request(f'https://{host}.holdinglalabranza.com{path}', headers=headers)
    try:
        response = opener.open(request, timeout=20)
    except urllib.error.HTTPError as error:
        response = error
    assert response.code == expected, (label, response.code, expected)
    content = response.read()
    results[label] = {'http': response.code}
    if label == 'billing_admin':
        data = json.loads(content)
        assert data['service'] == 'poliedro-billing' and data['licenseEnforcement']
    elif label == 'billing_openapi':
        data = json.loads(content)
        assert data['openapi'].startswith('3.') and data['paths']
    elif label == 'billing_docs':
        assert b'Poliedro Billing API' in content and b'/swagger/v1/swagger.json' in content
    elif label == 'billing_health':
        assert json.loads(content)['status'] == 'Healthy'
    elif label == 'nextcloud_status':
        data = json.loads(content)
        assert data['installed'] and not data['maintenance'] and not data['needsDbUpgrade']
    elif label == 'hangfire_dashboard':
        assert b'Hangfire' in content
        html = content.decode('utf-8')
        # Check what the dashboard actually loads; the main HTML alone misses storage errors.
        for asset in re.findall(r'(?:src|href)="(/hangfire/(?:css|js)[^"]+)"', html):
            req = urllib.request.Request('https://hangfire.holdinglalabranza.com' + asset)
            with opener.open(req, timeout=20) as resource:
                assert resource.status == 200 and resource.read()
        header = re.search(r'<meta name="csrf-header" content="([^"]+)"', html).group(1)
        token = re.search(r'<meta name="csrf-token" content="([^"]+)"', html).group(1)
        metrics = re.findall(r'data-metric="([^"]+)"', html)
        payload = urllib.parse.urlencode([('metrics[]', metric) for metric in metrics]).encode()
        req = urllib.request.Request('https://hangfire.holdinglalabranza.com/hangfire/stats',
                                     data=payload, headers={
                                         header: token, 'Content-Type': 'application/x-www-form-urlencoded'})
        with opener.open(req, timeout=20) as stats:
            assert stats.status == 200
            json.load(stats)
        results['hangfire_assets_and_statistics'] = {'http': 200}
        for action in ('trigger', 'remove'):
            req = urllib.request.Request('https://hangfire.holdinglalabranza.com/hangfire/recurring/' + action,
                                         data=b'jobs%5B%5D=nonexistent-readonly-check',
                                         headers={header: token, 'Content-Type': 'application/x-www-form-urlencoded'})
            try:
                result = opener.open(req, timeout=20)
                code = result.status
            except urllib.error.HTTPError as error:
                code = error.code
            assert code == 403, (action, code)
            results['hangfire_blocks_' + action] = {'http': code}
request = urllib.request.Request('https://billing.holdinglalabranza.com/api/v1/admin/clients',
                                 data=b'{}', headers={'Content-Type': 'application/json'})
try:
    response = opener.open(request, timeout=20)
    code = response.status
except urllib.error.HTTPError as error:
    code = error.code
assert code == 401
results['billing_denies_anonymous_write'] = {'http': code}
for host in ('billing', 'hangfire', 'nextcloud'):
    hostname = host + '.holdinglalabranza.com'
    with socket.create_connection((hostname, 443), timeout=15) as sock:
        with ssl.create_default_context().wrap_socket(sock, server_hostname=hostname) as secure:
            certificate = secure.getpeercert()
            results[host + '_tls'] = {'verified': True, 'expires': certificate['notAfter']}
(root / 'verification.json').write_text(json.dumps(results, indent=2))
print(json.dumps(results, indent=2))
