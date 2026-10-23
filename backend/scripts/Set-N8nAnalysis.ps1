param(
    [ValidateSet('All','Cv','Transcript','KnowledgeCheck','Roadmap','Mira')][string]$Workflow = 'All',
    [string]$HeaderName = 'X-LevelUp-API-Key',
    [string]$CvUrl = 'https://n8n-capstone-michelle.southafricanorth.cloudapp.azure.com/webhook/cv-analysis',
    [string]$TranscriptUrl = 'https://n8n-capstone-michelle.southafricanorth.cloudapp.azure.com/webhook/transcript-analyze',
    [string]$KnowledgeCheckUrl = 'https://n8n-capstone-michelle.southafricanorth.cloudapp.azure.com/webhook/knowledge-check-generate',
    [string]$RoadmapUrl = 'https://n8n-capstone-michelle.southafricanorth.cloudapp.azure.com/webhook/roadmap-generate',
    [string]$MiraUrl = 'https://n8n-capstone-michelle.southafricanorth.cloudapp.azure.com/webhook/mira'
)
$ErrorActionPreference = 'Stop'
$projectPath = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\CapstoneProjectBE'))
$urls = @{Cv=$CvUrl;Transcript=$TranscriptUrl;KnowledgeCheck=$KnowledgeCheckUrl;Roadmap=$RoadmapUrl;Mira=$MiraUrl}
$selected = if ($Workflow -eq 'All') {@('Cv','Transcript','KnowledgeCheck','Roadmap','Mira')} else {@($Workflow)}
foreach ($flow in $selected) {
    $uri = $null
    $url = $urls[$flow]
    if (-not [Uri]::TryCreate($url, [UriKind]::Absolute, [ref]$uri) -or $uri.Scheme -ne 'https' -or
        $uri.UserInfo -or $uri.Query -or $uri.Fragment -or -not $uri.AbsolutePath.StartsWith('/webhook/')) {
        throw "Use an HTTPS production webhook URL without query parameters or credentials for $flow."
    }
}
if ($HeaderName -notmatch '^[A-Za-z0-9_-]+$' -or $HeaderName -in @('Host','Content-Type','Content-Length','Connection','Transfer-Encoding','Cookie')) {
    throw 'Enter the exact existing Header Auth credential name.'
}
$secureSecret = Read-Host "Existing n8n Header Auth secret for $Workflow (hidden)" -AsSecureString
$secret = $null
$payload = $null
try {
    $pointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secureSecret)
    try { $secret = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($pointer) }
    finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($pointer) }
    if ([string]::IsNullOrWhiteSpace($secret) -or $secret.Contains("`r") -or $secret.Contains("`n")) { throw 'A single-line header secret is required.' }
    $settings = @{}
    foreach ($flow in $selected) {
        $urlKey = if ($flow -eq 'Cv') {'CvAnalysisUrl'} elseif ($flow -eq 'Transcript') {'TranscriptAnalysisUrl'} else {"${flow}Url"}
        $settings["N8n:${urlKey}"] = $urls[$flow]
        $settings["N8n:${flow}AuthHeaderName"] = $HeaderName
        $settings["N8n:${flow}AuthHeaderValue"] = $secret
    }
    if ($Workflow -eq 'All') {
        $settings['N8n:AuthHeaderName'] = $HeaderName
        $settings['N8n:AuthHeaderValue'] = $secret
    }
    $payload = $settings | ConvertTo-Json -Compress
    $payload | dotnet user-secrets set --project $projectPath | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'Could not save n8n configuration.' }
    Write-Host "n8n configuration saved for $Workflow. Restart the backend after publishing the matching workflows."
} finally { $secret = $null; $payload = $null; $settings = $null; $secureSecret.Dispose() }
