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

function Invoke-NativeCommand([scriptblock]$Command) {
    $prevEAP = $ErrorActionPreference
    $ErrorActionPreference = "Continue"
    try {
        $rawOutput = & $Command 2>&1
        $exitCode = $LASTEXITCODE
        if ($null -eq $exitCode -or $exitCode -eq "") {
            $exitCode = if ($?) { 0 } else { 1 }
        }
        $output = $rawOutput | ForEach-Object {
            if ($_ -is [System.Management.Automation.ErrorRecord]) { $_.ToString() } else { $_ }
        } | Out-String
        [PSCustomObject]@{
            Output   = $output
            ExitCode = [int]$exitCode
        }
    } finally {
        $ErrorActionPreference = $prevEAP
    }
}

function Get-EnvValue([string]$Name, [string]$Default = "") {
    if (-not (Test-Path -LiteralPath ".env")) {
        return $Default
    }

    $line = Get-Content -LiteralPath ".env" -Encoding UTF8 |
        Where-Object { $_ -match "^\s*$([regex]::Escape($Name))\s*=\s*(.+?)\s*$" } |
        Select-Object -First 1

    if ($line -match '=\s*(.+?)\s*$') {
        return $Matches[1].Trim().Trim('"').Trim("'")
    }

    return $Default
}

function Get-DeployMode {
    $mode = (Get-EnvValue "DEPLOY_MODE" "local").ToLowerInvariant()
    if ($mode -eq "server") { return "server" }
    return "local"
}

function Get-FrontendPort {
    $portText = Get-EnvValue "FRONTEND_PORT" "8080"
    if ($portText -match '^\d+$') {
        return [int]$portText
    }
    return 8080
}

function Get-ApiPort {
    $portText = Get-EnvValue "API_PORT" "5009"
    if ($portText -match '^\d+$') {
        return [int]$portText
    }
    return 5009
}

$deployMode = Get-DeployMode
$isServer = $deployMode -eq "server"

if ($isServer) {
    Write-Step "Deploy mode: server"
    Write-Host "Server mode is intended for Linux. On Windows use DEPLOY_MODE=local in .env"
    Write-Host "or run deploy/start-server.sh on the server VM."
    Write-Host ""
}

Write-Step "Checking Docker"
if (-not (Test-CommandAvailable "docker")) {
    Write-Error @"
Docker was not found. Install Docker Desktop and ensure the engine is running:
https://www.docker.com/products/docker-desktop/
"@
}

$composeVersion = Invoke-NativeCommand { docker compose version }
if ($composeVersion.ExitCode -ne 0) {
    Write-Error "Docker Compose is not available. Start Docker Desktop and try again."
}

Write-Host $composeVersion.Output

if (-not (Test-Path -LiteralPath ".env")) {
    Write-Step "Creating .env from .env.example"
    if (-not (Test-Path -LiteralPath ".env.example")) {
        Write-Error ".env.example was not found in $Root"
    }
    Copy-Item -LiteralPath ".env.example" -Destination ".env"
    Write-Host "Created .env - set DB_PASSWORD and FINTRACKER_VERSION if needed."
}

if ($isServer) {
    $domain = Get-EnvValue "DOMAIN"
    if ([string]::IsNullOrWhiteSpace($domain)) {
        Write-Error "Server mode requires DOMAIN in .env"
    }
}

$frontendPort = Get-FrontendPort
$apiPort = Get-ApiPort
$publicUrl = Get-EnvValue "PUBLIC_URL" "https://$(Get-EnvValue 'DOMAIN')"
$frontendUrl = if ($isServer) { $publicUrl } else { "http://localhost:$frontendPort" }
$apiUrl = if ($isServer) { "$publicUrl/api" } else { "http://localhost:$apiPort" }
$swaggerUrl = if ($isServer) { "$publicUrl/api/swagger" } else { "http://localhost:$apiPort/swagger" }

$composeFiles = @()
if ($isServer) {
    $composeFiles = @("-f", "docker-compose.images.yml", "-f", "docker-compose.server.yml")
} elseif ($Build) {
    $composeFiles = @("-f", "docker-compose.yml", "-f", "docker-compose.local.yml")
} else {
    $composeFiles = @("-f", "docker-compose.images.yml", "-f", "docker-compose.local.yml")
}

$composeExitCode = 0

if ($Build -and -not $isServer) {
    Write-Step "Starting with local build"
    $upResult = Invoke-NativeCommand { docker compose -f docker-compose.yml -f docker-compose.local.yml up --build -d }
    Write-Host $upResult.Output
    $composeExitCode = $upResult.ExitCode
} elseif ($isServer) {
    Write-Step "Pulling images from GHCR"
    $pullResult = Invoke-NativeCommand { docker compose @composeFiles pull }
    Write-Host $pullResult.Output
    if ($pullResult.ExitCode -ne 0) {
        Write-Error "Could not pull images for server deploy. Check GHCR access and FINTRACKER_VERSION."
    }

    Write-Step "Starting server stack (npm proxy on port $(Get-EnvValue 'FINTRACKER_HOST_PORT' '8082'))"
    $upResult = Invoke-NativeCommand { docker compose @composeFiles up -d }
    Write-Host $upResult.Output
    $composeExitCode = $upResult.ExitCode
} else {
    $useLocalBuild = $false

    Write-Step "Pulling images from GHCR"
    $pullResult = Invoke-NativeCommand { docker compose -f docker-compose.images.yml -f docker-compose.local.yml pull }
    Write-Host $pullResult.Output

    if ($pullResult.ExitCode -ne 0) {
        Write-Host ""
        $ghcrHint = ""
        if ($pullResult.Output -match "unauthorized|denied|access forbidden") {
            $ghcrHint = @"

GHCR images may be private. Make packages fintracker-api and fintracker-frontend
Public on GitHub, or run: docker login ghcr.io
"@
        }
        Write-Warning @"
Could not pull pre-built images from GHCR (exit $($pullResult.ExitCode)):

$($pullResult.Output.Trim())
$ghcrHint
Falling back to local build from source (first run may take 5-15 min).
"@
        $useLocalBuild = $true
    }

    if ($useLocalBuild) {
        Write-Step "Starting with local build (fallback)"
        $upResult = Invoke-NativeCommand { docker compose -f docker-compose.yml -f docker-compose.local.yml up --build -d }
        Write-Host $upResult.Output
        $composeExitCode = $upResult.ExitCode
        $Build = $true
    } else {
        Write-Step "Starting containers"
        $upResult = Invoke-NativeCommand { docker compose -f docker-compose.images.yml -f docker-compose.local.yml up -d }
        Write-Host $upResult.Output
        $composeExitCode = $upResult.ExitCode
    }
}

if ($composeExitCode -ne 0) {
    Write-Error "Docker Compose exited with an error."
}

Write-Step "Container status"
if ($Build -and -not $isServer) {
    $psResult = Invoke-NativeCommand { docker compose -f docker-compose.yml -f docker-compose.local.yml ps }
} else {
    $psResult = Invoke-NativeCommand { docker compose @composeFiles ps }
}
Write-Host $psResult.Output

Write-Host ""
Write-Host "FinTracker is running." -ForegroundColor Green
Write-Host "  Frontend: $frontendUrl"
Write-Host "  API:      $apiUrl"
if (-not $isServer) {
    Write-Host "  Swagger:  $swaggerUrl"
}
Write-Host ""
Write-Host "Stop: .\stop.ps1  or  docker compose down"

if (-not $isServer) {
    Write-Step "Opening browser"
    Start-Process $frontendUrl
}
