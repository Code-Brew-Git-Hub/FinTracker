#Requires -Version 5.1
<#
.SYNOPSIS
  Запуск FinTracker через Docker Compose (self-hosted).

.PARAMETER Build
  Собрать образы из исходников (docker-compose.yml) вместо готовых образов GHCR.

.EXAMPLE
  .\start.ps1
  .\start.ps1 -Build
#>
[CmdletBinding()]
param(
    [switch]$Build
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$Root = $PSScriptRoot
Set-Location -LiteralPath $Root

function Write-Step([string]$Message) {
    Write-Host ">> $Message" -ForegroundColor Cyan
}

function Test-CommandAvailable([string]$Name) {
    $null -ne (Get-Command $Name -ErrorAction SilentlyContinue)
}

function Get-FrontendPort {
    $defaultPort = 8080
    if (-not (Test-Path -LiteralPath ".env")) {
        return $defaultPort
    }

    $line = Get-Content -LiteralPath ".env" -Encoding UTF8 |
        Where-Object { $_ -match '^\s*FRONTEND_PORT\s*=\s*(\d+)\s*$' } |
        Select-Object -First 1

    if ($line -match '=\s*(\d+)') {
        return [int]$Matches[1]
    }

    return $defaultPort
}

Write-Step "Проверка Docker"
if (-not (Test-CommandAvailable "docker")) {
    Write-Error @"
Docker не найден. Установите Docker Desktop и убедитесь, что Engine запущен:
https://www.docker.com/products/docker-desktop/
"@
}

$composeVersion = docker compose version 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Error "Docker Compose недоступен. Запустите Docker Desktop и повторите."
}

Write-Host $composeVersion

if (-not (Test-Path -LiteralPath ".env")) {
    Write-Step "Создание .env из .env.example"
    if (-not (Test-Path -LiteralPath ".env.example")) {
        Write-Error "Файл .env.example не найден в $Root"
    }
    Copy-Item -LiteralPath ".env.example" -Destination ".env"
    Write-Host "Создан .env — при необходимости смените DB_PASSWORD и FINTRACKER_VERSION."
}

$frontendPort = Get-FrontendPort
$frontendUrl = "http://localhost:$frontendPort"

if ($Build) {
    Write-Step "Запуск со сборкой из исходников"
    docker compose up --build -d
} else {
    Write-Step "Загрузка готовых образов (GHCR)"
    docker compose -f docker-compose.images.yml pull
    if ($LASTEXITCODE -ne 0) {
        Write-Error @"
Не удалось скачать образы. Возможные причины:
  - пакеты GHCR ещё не опубликованы или приватны (нужен docker login ghcr.io);
  - нет сети.
Попробуйте сборку из исходников: .\start.ps1 -Build
"@
    }

    Write-Step "Запуск контейнеров"
    docker compose -f docker-compose.images.yml up -d
}

if ($LASTEXITCODE -ne 0) {
    Write-Error "Docker Compose завершился с ошибкой."
}

Write-Step "Статус контейнеров"
if ($Build) {
    docker compose ps
} else {
    docker compose -f docker-compose.images.yml ps
}

Write-Host ""
Write-Host "FinTracker запущен." -ForegroundColor Green
Write-Host "  Frontend: $frontendUrl"
Write-Host "  API:      http://localhost:5009"
Write-Host "  Swagger:  http://localhost:5009/swagger"
Write-Host ""
Write-Host "Остановка: .\stop.ps1  или  docker compose down"

Write-Step "Открытие браузера"
Start-Process $frontendUrl
