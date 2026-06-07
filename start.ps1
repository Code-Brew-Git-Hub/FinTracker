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

function Invoke-NativeCommand {
    param(
        [Parameter(Mandatory)][string]$FilePath,
        [string[]]$ArgumentList
    )

    $previousErrorAction = $ErrorActionPreference
    $ErrorActionPreference = "Continue"
    try {
        $output = & $FilePath @ArgumentList 2>&1 | ForEach-Object {
            if ($_ -is [System.Management.Automation.ErrorRecord]) {
                $_.ToString()
            } else {
                $_
            }
        }
        return [pscustomobject]@{
            Output   = ($output | Out-String).Trim()
            ExitCode = $LASTEXITCODE
        }
    } finally {
        $ErrorActionPreference = $previousErrorAction
    }
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

$composeCheck = Invoke-NativeCommand -FilePath "docker" -ArgumentList @("compose", "version")
if ($composeCheck.ExitCode -ne 0) {
    Write-Error "Docker Compose is not available. Start Docker Desktop and try again."
}

Write-Host $composeCheck.Output

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
    $upResult = Invoke-NativeCommand -FilePath "docker" -ArgumentList @("compose", "up", "--build", "-d")
    if ($upResult.Output) {
        Write-Host $upResult.Output
    }
} else {
    $useLocalBuild = $false

    Write-Step "Pulling images from GHCR"
    $pullResult = Invoke-NativeCommand -FilePath "docker" -ArgumentList @(
        "compose", "-f", "docker-compose.images.yml", "pull"
    )
    if ($pullResult.Output) {
        Write-Host $pullResult.Output
    }

    if ($pullResult.ExitCode -ne 0) {
        if ($pullResult.Output -match "unauthorized|denied|access forbidden") {
            Write-Host ""
            Write-Warning @"
GHCR images are not publicly accessible (unauthorized).
Falling back to local build from source (first run may take 5-15 min).
To use pre-built images later: ask the maintainer to set packages
fintracker-api and fintracker-frontend to Public on GitHub, or run:
  docker login ghcr.io
"@
            $useLocalBuild = $true
        } else {
            Write-Error @"
Failed to pull images. Check your network connection.
You can build locally: .\start.ps1 -Build
"@
        }
    }

    if ($useLocalBuild) {
        Write-Step "Starting with local build (fallback)"
        $upResult = Invoke-NativeCommand -FilePath "docker" -ArgumentList @("compose", "up", "--build", "-d")
        if ($upResult.Output) {
            Write-Host $upResult.Output
        }
        $Build = $true
    } else {
        Write-Step "Starting containers"
        $upResult = Invoke-NativeCommand -FilePath "docker" -ArgumentList @(
            "compose", "-f", "docker-compose.images.yml", "up", "-d"
        )
        if ($upResult.Output) {
            Write-Host $upResult.Output
        }
    }
}

if ($upResult.ExitCode -ne 0) {
    Write-Error "Docker Compose exited with an error."
}

Write-Step "Container status"
if ($Build) {
    $psResult = Invoke-NativeCommand -FilePath "docker" -ArgumentList @("compose", "ps")
} else {
    $psResult = Invoke-NativeCommand -FilePath "docker" -ArgumentList @(
        "compose", "-f", "docker-compose.images.yml", "ps"
    )
}
if ($psResult.Output) {
    Write-Host $psResult.Output
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
