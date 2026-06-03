#Requires -Version 5.1
<#
.SYNOPSIS
  Start FinTracker via Docker Compose (self-hosted).

.PARAMETER Build
  Build images from source (docker-compose.yml) instead of GHCR images.

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

Write-Step "Checking Docker"
if (-not (Test-CommandAvailable "docker")) {
    Write-Error @"
Docker was not found. Install Docker Desktop and ensure the engine is running:
https://www.docker.com/products/docker-desktop/
"@
}

$composeVersion = docker compose version 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Error "Docker Compose is not available. Start Docker Desktop and try again."
}

Write-Host $composeVersion

if (-not (Test-Path -LiteralPath ".env")) {
    Write-Step "Creating .env from .env.example"
    if (-not (Test-Path -LiteralPath ".env.example")) {
        Write-Error ".env.example was not found in $Root"
    }
    Copy-Item -LiteralPath ".env.example" -Destination ".env"
    Write-Host "Created .env - set DB_PASSWORD and FINTRACKER_VERSION if needed."
}

$frontendPort = Get-FrontendPort
$frontendUrl = "http://localhost:$frontendPort"

if ($Build) {
    Write-Step "Starting with local build"
    docker compose up --build -d
} else {
    Write-Step "Pulling images from GHCR"
    docker compose -f docker-compose.images.yml pull
    if ($LASTEXITCODE -ne 0) {
        Write-Error @"
Failed to pull images. Possible causes:
  - GHCR packages are private (run: docker login ghcr.io);
  - no network connection.
Try building from source: .\start.ps1 -Build
"@
    }

    Write-Step "Starting containers"
    docker compose -f docker-compose.images.yml up -d
}

if ($LASTEXITCODE -ne 0) {
    Write-Error "Docker Compose exited with an error."
}

Write-Step "Container status"
if ($Build) {
    docker compose ps
} else {
    docker compose -f docker-compose.images.yml ps
}

Write-Host ""
Write-Host "FinTracker is running." -ForegroundColor Green
Write-Host "  Frontend: $frontendUrl"
Write-Host "  API:      http://localhost:5009"
Write-Host "  Swagger:  http://localhost:5009/swagger"
Write-Host ""
Write-Host "Stop: .\stop.ps1  or  docker compose down"

Write-Step "Opening browser"
Start-Process $frontendUrl
