#Requires -Version 5.1
<#
.SYNOPSIS
  Stop FinTracker (Docker Compose).
#>
[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
$Root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
Set-Location -LiteralPath $Root

$ComposeDir = Join-Path $Root "docker"
$ComposeBuild = "docker/docker-compose.yml"
$ComposeLocal = "docker/docker-compose.local.yml"
$ComposeServer = "docker/docker-compose.server.yml"
$ComposeImages = "docker/docker-compose.images.yml"

function Invoke-DockerSilently([scriptblock]$Command) {
    $prevEAP = $ErrorActionPreference
    $ErrorActionPreference = "Continue"
    try {
        & $Command 2>&1 | Out-Null
    } finally {
        $ErrorActionPreference = $prevEAP
    }
}

Write-Host ">> Stopping FinTracker" -ForegroundColor Cyan

if (Test-Path -LiteralPath (Join-Path $ComposeDir "docker-compose.local.yml")) {
    Invoke-DockerSilently { docker compose -f $ComposeBuild -f $ComposeLocal down }
    Invoke-DockerSilently { docker compose -f $ComposeImages -f $ComposeLocal down }
}

if (Test-Path -LiteralPath (Join-Path $ComposeDir "docker-compose.server.yml")) {
    Invoke-DockerSilently { docker compose -f $ComposeImages -f $ComposeServer down }
}

if (Test-Path -LiteralPath (Join-Path $ComposeDir "docker-compose.images.yml")) {
    Invoke-DockerSilently { docker compose -f $ComposeImages down }
}

Invoke-DockerSilently { docker compose down }

Write-Host "Done. PostgreSQL data is kept (volume postgres_data)." -ForegroundColor Green
Write-Host "Full DB reset: docker compose -f docker/docker-compose.images.yml down -v"
