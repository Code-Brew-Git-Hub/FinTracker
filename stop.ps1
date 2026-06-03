#Requires -Version 5.1
<#
.SYNOPSIS
  Остановка FinTracker (Docker Compose).
#>
[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-Location -LiteralPath $PSScriptRoot

Write-Host ">> Остановка FinTracker" -ForegroundColor Cyan

if (Test-Path -LiteralPath "docker-compose.images.yml") {
    docker compose -f docker-compose.images.yml down 2>$null
}

docker compose down 2>$null

Write-Host "Готово. Данные PostgreSQL сохранены (volume postgres_data)." -ForegroundColor Green
Write-Host "Полный сброс БД: docker compose -f docker-compose.images.yml down -v"
