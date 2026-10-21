import io
import json
import os
from pathlib import Path
import tarfile
import tempfile
from types import SimpleNamespace
import unittest
from unittest.mock import patch

import receiver


class ReceiverTests(unittest.TestCase):
    def test_rejects_paths_links_and_private_configuration(self):
        for name in ('../secret', '/etc/passwd', 'a/../../secret', r'a\secret',
                     'appsettings.Production.json', '.env', 'key.pem'):
            with self.subTest(name=name):
                with self.assertRaises(ValueError):
                    receiver.validate_member(tarfile.TarInfo(name))
        link = tarfile.TarInfo('link')
        link.type = tarfile.SYMTYPE
        with self.assertRaises(ValueError):
            receiver.validate_member(link)

    def test_rejects_corrupted_upload(self):
        with tempfile.TemporaryDirectory() as directory:
            with self.assertRaises(ValueError):
                receiver.receive(io.BytesIO(b'bad upload'), Path(directory) / 'payload', '0' * 64)

    def test_failed_verification_restores_previous_image(self):
        revision = 'a' * 40
        data = io.BytesIO()
        with tarfile.open(fileobj=data, mode='w:gz') as archive:
            for name, content in {
                'appsettings.json': b'{}',
                'release.json': json.dumps({'component': 'api', 'revision': revision}).encode(),
                'Poliedro.Billing.Api.dll': b'fake assembly for mocked deployment',
                'runtimes/linux/native/library.so': b'fake native dependency',
            }.items():
                member = tarfile.TarInfo(name)
                member.size = len(content)
                archive.addfile(member, io.BytesIO(content))
        payload = data.getvalue()
        checksum = receiver.hashlib.sha256(payload).hexdigest()
        calls = []

        def fake_run(args, log):
            calls.append(args)
            if args == ['python3', '/opt/poliedro-edge/verify.py']:
                raise RuntimeError('Simulated failing readiness check')

        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            # Must preserve the other service's deployment pin during rollback.
            (root / '.env').write_text('BILLING_HANGFIRE_IMAGE=hangfire:untouched\n')
            with patch.object(receiver, 'ROOT', root), \
                 patch.object(receiver.sys, 'argv', ['receiver.py', 'api']), \
                 patch.object(receiver.sys, 'stdin', SimpleNamespace(buffer=io.BytesIO(payload))), \
                 patch.dict(os.environ, {'SSH_ORIGINAL_COMMAND': f'deploy {revision} {checksum}'}), \
                 patch.object(receiver.subprocess, 'check_output', return_value='sha256:previous\n'), \
                 patch.object(receiver, 'run', side_effect=fake_run), \
                 patch.object(receiver, 'wait_service') as wait:
                with self.assertRaises(RuntimeError):
                    receiver.main()
            settings = (root / '.env').read_text()
            self.assertIn('BILLING_API_IMAGE=sha256:previous', settings)
            self.assertIn('BILLING_HANGFIRE_IMAGE=hangfire:untouched', settings)
            self.assertEqual(wait.call_count, 2)
            updates = [args for args in calls if args[:3] == ['docker', 'compose', 'up']]
            self.assertEqual(len(updates), 2)
            self.assertTrue(all(args[-1] == 'api' for args in updates))
            self.assertFalse((root / 'ci-state/api.json').exists())
            native_dir = next((root / 'releases').glob('api/*/publish/runtimes/linux/native'))
            self.assertEqual(native_dir.stat().st_mode & 0o777, 0o755)

    def test_restricted_key_rejects_arbitrary_shell_command(self):
        with patch.object(receiver.sys, 'argv', ['receiver.py', 'api']), \
             patch.dict(os.environ, {'SSH_ORIGINAL_COMMAND': 'bash -c id'}):
            with self.assertRaises(ValueError):
                receiver.main()


if __name__ == '__main__':
    unittest.main()
