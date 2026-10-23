param([string]$FromEmail)

$ErrorActionPreference = 'Stop'
$projectPath = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\CapstoneProjectBE'))
$secureApiKey = Read-Host 'Bird API key (hidden)' -AsSecureString
$apiKey = $null
$payload = $null

try {
    $pointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secureApiKey)
    try { $apiKey = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($pointer) }
    finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($pointer) }

    if ([string]::IsNullOrWhiteSpace($apiKey) -or $apiKey.Contains("`r") -or $apiKey.Contains("`n")) {
        throw 'A single-line Bird API key is required.'
    }

    $settings = @{
        'Email:Provider' = 'Bird'
        'Bird:ApiKey' = $apiKey.Trim()
    }

    if ($FromEmail) {
        $parsedAddress = $null
        if (-not [Net.Mail.MailAddress]::TryCreate($FromEmail, [ref]$parsedAddress)) {
            throw 'FromEmail must be a valid email address verified by Bird.'
        }
        $settings['Bird:FromEmail'] = $parsedAddress.Address
    }

    $payload = $settings | ConvertTo-Json -Compress
    # Piped JSON avoids placing the secret in command arguments or PowerShell history.
    $payload | dotnet user-secrets set --project $projectPath | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'Could not save the Bird email configuration.' }

    Write-Host 'Bird email configuration saved in backend User Secrets. Restart the backend to activate it.'
} finally {
    $apiKey = $null
    $payload = $null
    $settings = $null
    $secureApiKey.Dispose()
}
