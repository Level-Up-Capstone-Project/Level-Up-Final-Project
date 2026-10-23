param([string]$CredentialsPath)

$ErrorActionPreference = 'Stop'
$projectPath = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\CapstoneProjectBE'))
$clientSecret = $null
$payload = $null
$secureSecret = $null
try {
    if ($CredentialsPath) {
        $credentials = Get-Content -LiteralPath $CredentialsPath -Raw | ConvertFrom-Json
        $clientId = $credentials.client_credentials.client_id
        $clientSecret = $credentials.client_credentials.client_secret
    } else {
        $clientId = Read-Host 'Adobe PDF Services Client ID'
        $secureSecret = Read-Host 'Adobe PDF Services Client Secret' -AsSecureString
        $secretPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secureSecret)
        try { $clientSecret = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($secretPointer) }
        finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($secretPointer) }
    }
    if ([string]::IsNullOrWhiteSpace($clientId) -or [string]::IsNullOrWhiteSpace($clientSecret)) {
        throw 'Client ID and Client Secret are required. Use the Adobe PDF Services credentials JSON.'
    }
    $payload = @{
        'Adobe:ClientId' = $clientId.Trim()
        'Adobe:ClientSecret' = $clientSecret.Trim()
        'Ocr:Provider' = 'Adobe'
    } | ConvertTo-Json -Compress
    # Piped JSON avoids placing the secret in command arguments or PowerShell history.
    $payload | dotnet user-secrets set --project $projectPath | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'Could not save the Adobe configuration.' }
    Write-Host 'Adobe OCR credentials saved in backend User Secrets. Restart the backend to activate Adobe OCR.'
} finally {
    $clientSecret = $null
    $payload = $null
    $credentials = $null
    if ($secureSecret) { $secureSecret.Dispose() }
}
