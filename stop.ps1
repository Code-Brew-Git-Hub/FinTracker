#Requires -Version 5.1
<#
.SYNOPSIS
  Stop FinTracker (Docker Compose).
#>
[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-Location -LiteralPath $PSScriptRoot

# Docker writes progress to stderr; PowerShell treats that as a terminating error with Stop.
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

if (Test-Path -LiteralPath "docker-compose.images.yml") {
    Invoke-DockerSilently { docker compose -f docker-compose.images.yml down }
}

Invoke-DockerSilently { docker compose down }

Write-Host "Done. PostgreSQL data is kept (volume postgres_data)." -ForegroundColor Green
Write-Host "Full DB reset: docker compose -f docker-compose.images.yml down -v"
