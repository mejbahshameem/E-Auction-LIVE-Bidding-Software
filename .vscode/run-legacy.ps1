param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("Restore", "Build", "Run", "Stop")]
    [string]$Action,
    [int]$Port = 8080,
    [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$solutionPath = Join-Path $repoRoot "MvcApplication1.sln"
$webProjectPath = Join-Path $repoRoot "MvcApplication1"
$iisUserHome = Join-Path $repoRoot ".iisexpress"
$iisExpressDefaultPath = "C:\Program Files\IIS Express\iisexpress.exe"

function Get-VSWherePath {
    $vswherePath = "C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe"
    if (Test-Path $vswherePath) {
        return $vswherePath
    }

    return $null
}

function Get-MSBuildPath {
    $msbuildCommand = Get-Command msbuild -ErrorAction SilentlyContinue
    if ($null -ne $msbuildCommand) {
        return $msbuildCommand.Source
    }

    $vswherePath = Get-VSWherePath
    if ($null -ne $vswherePath) {
        $installationPath = & $vswherePath -latest -requires Microsoft.Component.MSBuild -property installationPath
        if ($installationPath) {
            $candidate = Join-Path $installationPath "MSBuild\Current\Bin\MSBuild.exe"
            if (Test-Path $candidate) {
                return $candidate
            }
        }
    }

    $frameworkMsbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
    if (Test-Path $frameworkMsbuild) {
        return $frameworkMsbuild
    }

    throw "MSBuild not found. Install Visual Studio Build Tools with MSBuild workload."
}

function Get-NuGetPath {
    $nugetCommand = Get-Command nuget -ErrorAction SilentlyContinue
    if ($null -ne $nugetCommand) {
        return $nugetCommand.Source
    }

    $vswherePath = Get-VSWherePath
    if ($null -ne $vswherePath) {
        $installationPath = & $vswherePath -latest -property installationPath
        if ($installationPath) {
            $candidate = Join-Path $installationPath "Common7\IDE\CommonExtensions\Microsoft\NuGet\NuGet.exe"
            if (Test-Path $candidate) {
                return $candidate
            }
        }
    }

    return $null
}

function Get-IISExpressPath {
    if (Test-Path $iisExpressDefaultPath) {
        return $iisExpressDefaultPath
    }

    $iisExpressCommand = Get-Command iisexpress -ErrorAction SilentlyContinue
    if ($null -ne $iisExpressCommand) {
        return $iisExpressCommand.Source
    }

    throw "IIS Express not found. Install IIS Express or Visual Studio."
}

function Invoke-Restore {
    if (-not (Test-Path $solutionPath)) {
        throw "Solution not found: $solutionPath"
    }

    $nugetPath = Get-NuGetPath
    if ($null -ne $nugetPath) {
        Write-Host "Restoring packages with NuGet: $nugetPath"
        & $nugetPath restore $solutionPath
        return
    }

    $packagesPath = Join-Path $repoRoot "packages"
    if (Test-Path $packagesPath) {
        Write-Host "NuGet.exe not found, but 'packages' folder exists. Skipping restore."
        return
    }

    $msbuildPath = Get-MSBuildPath
    Write-Host "NuGet.exe not found. Restoring with MSBuild: $msbuildPath"
    & $msbuildPath $solutionPath /t:Restore /p:RestorePackagesConfig=true
}

function Invoke-Build {
    if (-not (Test-Path $solutionPath)) {
        throw "Solution not found: $solutionPath"
    }

    $msbuildPath = Get-MSBuildPath
    Write-Host "Building solution with MSBuild: $msbuildPath"
    & $msbuildPath $solutionPath /p:Configuration=$Configuration /m
}

function Invoke-Run {
    if (-not (Test-Path $webProjectPath)) {
        throw "Web project path not found: $webProjectPath"
    }

    if (-not (Test-Path $iisUserHome)) {
        New-Item -Path $iisUserHome -ItemType Directory -Force | Out-Null
    }

    $iisPath = Get-IISExpressPath
    $args = @("/path:$webProjectPath", "/port:$Port", "/userhome:$iisUserHome", "/systray:false")

    Write-Host "Starting IIS Express on http://localhost:$Port"
    Write-Host "Use VS Code task 'Stop IIS Express' when done."
    & $iisPath @args
}

function Invoke-Stop {
    $processes = Get-Process iisexpress -ErrorAction SilentlyContinue
    if ($null -eq $processes) {
        Write-Host "No IIS Express process found."
        return
    }

    $processes | Stop-Process -Force
    Write-Host "Stopped IIS Express."
}

switch ($Action) {
    "Restore" { Invoke-Restore }
    "Build" { Invoke-Build }
    "Run" { Invoke-Run }
    "Stop" { Invoke-Stop }
}
