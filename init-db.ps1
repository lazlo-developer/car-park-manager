
# Simple PowerShell script to always regenerate .env and secrets
$ErrorActionPreference = 'Stop'

$envFile = ".env"
$secretName = "ConnectionStrings:CarParkDatabase"
$projectPath = "CarParkManager.API"

# Always generate new password
$POSTGRES_PASSWORD = -join ((48..57) + (65..90) + (97..122) | Get-Random -Count 20 | % {[char]$_})
$POSTGRES_USER = "carpark_user"
$POSTGRES_DB = "carparkdb"
$POSTGRES_PORT = "5432"

# Write .env file
@(
    "POSTGRES_PASSWORD=$POSTGRES_PASSWORD"
    "POSTGRES_USER=$POSTGRES_USER"
    "POSTGRES_DB=$POSTGRES_DB"
    "POSTGRES_PORT=$POSTGRES_PORT"
) | Set-Content -Path $envFile

# Build connection string
$CONN_STR = "Host=localhost;Port=$POSTGRES_PORT;Database=$POSTGRES_DB;Username=$POSTGRES_USER;Password=$POSTGRES_PASSWORD"

# Set .NET user secret
dotnet user-secrets set $secretName $CONN_STR --project $projectPath

# Print summary
Write-Host "`n[.env]"
Write-Host "POSTGRES_USER=$POSTGRES_USER"
Write-Host "POSTGRES_DB=$POSTGRES_DB"
Write-Host "POSTGRES_PASSWORD=$POSTGRES_PASSWORD"
Write-Host "POSTGRES_PORT=$POSTGRES_PORT"
Write-Host "`n[.NET User Secret]"
Write-Host "$secretName=Host=localhost;Port=$POSTGRES_PORT;Database=$POSTGRES_DB;Username=$POSTGRES_USER;Password=$POSTGRES_PASSWORD"
Write-Host "`nTo verify: dotnet user-secrets list --project $projectPath"
Write-Host "`nRun: docker compose up -d"