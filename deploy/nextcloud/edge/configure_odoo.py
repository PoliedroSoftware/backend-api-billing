"""Execute in the local odoo_dev Odoo shell after public HTTPS verification."""
import json
from pathlib import Path

assert env.cr.dbname == 'odoo_dev'
instance = env['poliedro.billing.instance'].sudo().browse(1).exists()
assert instance and instance.company_id.name == 'Poliedro Software'
target = 'https://billing.holdinglalabranza.com'
assert instance.base_url in ('http://127.0.0.1:15068', target)
backup = Path('C:/Poliedro/odoo/backups/billing-20261019131515/remote-deployment/odoo-endpoint-before-https.json')
if not backup.exists():
    backup.write_text(json.dumps({'database': env.cr.dbname, 'instance_id': instance.id,
                                  'base_url': instance.base_url}), encoding='utf-8')
instance.write({'base_url': target})
instance.action_check_health()
assert instance.health == 'up', instance.health_message
clients = instance._request('GET', '/api/v1/admin/clients')
assert isinstance(clients, list)
env.cr.commit()
print(json.dumps({'instance_id': instance.id, 'base_url': instance.base_url,
                  'health': instance.health, 'remote_clients': len(clients),
                  'latency_ms': instance.latency_ms}))
