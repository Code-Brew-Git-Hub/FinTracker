#Requires -Version 5.1
<#
.SYNOPSIS
  Stop FinTracker (Docker Compose).
#>
[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-Location -LiteralPath $PSScriptRoot

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

Write-Host ">> Stopping FinTracker" -ForegroundColor Cyan

if (Test-Path -LiteralPath "docker-compose.images.yml") {
    $downResult = Invoke-NativeCommand -FilePath "docker" -ArgumentList @(
        "compose", "-f", "docker-compose.images.yml", "down"
    )
    if ($downResult.Output) {
        Write-Host $downResult.Output
    }
}

$downResult = Invoke-NativeCommand -FilePath "docker" -ArgumentList @("compose", "down")
if ($downResult.Output) {
    Write-Host $downResult.Output
}

if ($downResult.ExitCode -ne 0) {
    Write-Error "Docker Compose exited with an error."
}

Write-Host "Done. PostgreSQL data is kept (volume postgres_data)." -ForegroundColor Green
Write-Host "Full DB reset: docker compose -f docker-compose.images.yml down -v"
