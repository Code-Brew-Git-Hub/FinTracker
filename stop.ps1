#Requires -Version 5.1
<#
.SYNOPSIS
  Stop FinTracker (Docker Compose).
#>
[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-Location -LiteralPath $PSScriptRoot

Write-Host ">> Stopping FinTracker" -ForegroundColor Cyan

if (Test-Path -LiteralPath "docker-compose.images.yml") {
    docker compose -f docker-compose.images.yml down 2>$null
}

docker compose down 2>$null

Write-Host "Done. PostgreSQL data is kept (volume postgres_data)." -ForegroundColor Green
Write-Host "Full DB reset: docker compose -f docker-compose.images.yml down -v"
