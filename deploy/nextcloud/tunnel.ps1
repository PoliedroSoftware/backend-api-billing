# Private transport for local Odoo and the remote Billing services. No credentials in arguments.
$ErrorActionPreference = 'Stop'
$taskMutex = New-Object System.Threading.Mutex($false, 'Local\PoliedroBillingTunnel')
if (-not $taskMutex.WaitOne(0)) { exit 0 }
try {
    while ($true) {
        & "$env:WINDIR\System32\OpenSSH\ssh.exe" -N -T -i "$env:USERPROFILE\.ssh\id_rsa" `
          -o BatchMode=yes -o StrictHostKeyChecking=yes -o ExitOnForwardFailure=yes `
          -o ServerAliveInterval=20 -o ServerAliveCountMax=3 -o ConnectTimeout=10 `
          -L 127.0.0.1:15068:127.0.0.1:15068 -L 127.0.0.1:15069:127.0.0.1:15069 `
          -R 127.0.0.1:18070:127.0.0.1:8070 root@143.198.12.27
        Start-Sleep -Seconds 10
    }
} finally { $taskMutex.ReleaseMutex(); $taskMutex.Dispose() }
