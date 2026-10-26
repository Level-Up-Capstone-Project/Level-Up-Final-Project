# CapstoneProjectBE
Backend API for an AI-powered career development platform, handling authentication, user profiles, career and skill data, personalised recommendations, learning paths, and AI integrations.

## AI document analysis through n8n

After validating the file type, signature and 5 MiB size limit, the backend extracts the PDF text (PdfPig, with Adobe PDF Services OCR for scanned pages and always for transcripts), redacts personal information, and sends the text as JSON (`cvText` / `transcriptText`) to the authenticated production n8n webhook. n8n returns schema-validated structured analysis. The backend reconciles CV and transcript skill results against the canonical database catalogue and supplements them with deterministic matches from the extracted text, including common variants such as `C Sharp`/`C#` and `RESTful APIs`/`REST APIs`. This reconciliation is local, so existing n8n request contracts remain compatible. If CV analysis cannot use n8n because it is unavailable, misconfigured, times out, or returns an error or invalid response, the backend uses the deterministic text analyser and saves the same result shape. Transcript analysis continues to require n8n. Configure OCR with `Ocr__Provider=Adobe`, `Adobe__ClientId` and `Adobe__ClientSecret`.

CVs and transcripts must be PDF. The original user filename is replaced with `document.pdf` before the request, and personal details in the extracted text are redacted before the request. Require user consent, restrict n8n execution-log access, minimize execution retention, and use synthetic files for workflow tests. See [the workflow contracts](n8n/README.md).

Roadmap generation validates every returned workflow skill against the trusted career catalogue. For each weak skill, the backend reuses active database resources first and tells n8n only how many additional resource slots are missing. Valid public HTTPS candidates are normalized, deduplicated by provider identifier or canonical URL, persisted, and reused for later users with the same weak skill. If n8n is unavailable or returns malformed, incomplete, duplicate, or unknown data, the backend generates one deterministic roadmap step per missing or partial career skill and uses the resources already stored in the database.

## Google sign-in / sign-up

The existing Google buttons open the backend OAuth flow in a popup. ASP.NET's Google handler handles authorization-code exchange, PKCE and correlation cookies. The callback obtains Google's verified identity, looks up its stable subject in `external_logins`, and issues the same application JWT used by password login. Sign In only accepts an already linked Google identity; account creation is allowed only when the flow starts from Sign Up after the frontend's terms check. The JWT is delivered with `postMessage` to the configured frontend origin, not through a URL. The frontend verifies both the message origin and the popup window before accepting it, and uses the backend's `isNewUser` and `profileCompleted` values to select onboarding.

Setup is required before Google login can run:

1. Apply the EF schema migration described below. It includes the Google identity mapping table.
2. In Google Cloud Console, configure Google Auth Platform branding, audience/test users (while in testing), and a Web application OAuth client. Request only basic profile/email information.
3. Add this exact authorized redirect URI: `https://localhost:7286/signin-google`. The popup returns to the backend, so localhost:3000 is not the OAuth redirect URI. In production, also register `https://YOUR_BACKEND_HOST/signin-google`.
4. Configure the credentials and the frontend origin:

```powershell
dotnet user-secrets set "Authentication:Google:ClientId" 'YOUR_GOOGLE_CLIENT_ID' --project backend/CapstoneProjectBE
dotnet user-secrets set "Authentication:Google:ClientSecret" 'YOUR_GOOGLE_CLIENT_SECRET' --project backend/CapstoneProjectBE
dotnet user-secrets set "Frontend:Origin" 'http://localhost:3000' --project backend/CapstoneProjectBE
```

5. Restart the backend, allow popups on the frontend, and test both buttons. Signup still requires the existing terms checkbox. Google-created accounts need no email OTP. Local password accounts are not silently linked by matching email: use password login for those accounts until an authenticated account-linking flow is added.

With credentials absent, `/api/auth/google/config` reports `configured: false` and the buttons show a configuration message. Do not put the Google client secret in NEXT_PUBLIC variables. For Azure, use `Authentication__Google__ClientId`, `Authentication__Google__ClientSecret`, and `Frontend__Origin`. Set the origin to the exact public frontend origin, and preserve HTTPS host/scheme through your proxy so Google receives the registered callback URI. Configure trusted proxy forwarding for the chosen host; do not trust arbitrary forwarded headers.

## Verification email

Local Development uses `DevelopmentEmailService` by default. Registration and resend requests write the verification code to the backend console and do not require a shared Bird credential. This provider is rejected outside the Development environment.

To test real Bird delivery locally, run this from the repository root. The script prompts for the API key without adding it to command history and stores it in .NET User Secrets:

```powershell
.\backend\scripts\Set-BirdEmail.ps1
```

Use a different Bird-verified sender when necessary:

```powershell
.\backend\scripts\Set-BirdEmail.ps1 -FromEmail 'verified-sender@example.com'
```

Restart the backend after changing providers. A registration that previously failed during email delivery may already have created an inactive account; use **Resend verification code** instead of registering the same address again.

Production defaults to Bird and validates its configuration during startup. Configure `Email__Provider=Bird`, `Bird__ApiKey`, `Bird__BaseUrl`, `Bird__FromEmail`, and optionally `Bird__FromName` as protected application settings or secret references. Never commit or send the API key through chat.

The same Bird key and verified sender deliver password-reset codes and password-changed notifications. Apply the `AddPasswordResetAndSignupPreferences` migration before deploying the feature. The public flow uses `POST /api/auth/password-reset/request` and `POST /api/auth/password-reset/complete`; request responses do not disclose whether an account exists. Codes expire after 15 minutes, are limited to five attempts, and are removed after a successful reset. Google-only accounts continue to recover through Google sign-in rather than gaining a password through this flow.

## Automatic uploads and transcript analysis

Selecting a CV or PDF transcript starts uploading/analysis automatically when signed in and consent is granted. No analysis-card click is required. Cancel/removal/replacement/consent withdrawal aborts the request; failed runs require an explicit retry rather than looping uploads. Continue remains enabled only after analysis completes. No screens or test navigation controls were added.

`POST /api/transcripts/analyse` (also `/analyze`) authenticates the upload, validates PDF limits, extracts and redacts the text and sends it to the configured n8n production webhook, validates the structured response, and saves the report in Supabase. See [the n8n setup and response contract](n8n/README.md). The header secret stays on the backend. `GET /api/transcripts/current/analysis` retrieves the signed-in user’s latest saved report.

Verification covers the CV/transcript OCR and JSON workflow contracts, Google identity creation/returning users/unverified or inactive users/email collisions, transcript isolation from CV records, and frontend automatic start/cancellation/Strict Mode duplicate protection and popup origin validation. The published n8n workflows must still be tested after their shared secret is configured.

Document Analysis is a two-step frontend flow: CV completion leads to transcript upload, then both results are displayed together. CV quality, ATS and formatting remain separate from the transcript average. Dashboard career readiness is fetched from `/api/dashboard` after analysis; it is not calculated from transcript marks in the browser.

Document analysis clients can send `Accept: application/x-ndjson` to the CV and transcript analyse endpoints. The authenticated request streams `progress` events (completed stage percentages), followed by one `result` event or an `error` event with status/details. Existing JSON clients keep their original responses. The UI reaches 100% only after receiving the result; OCR stages can take time between milestones. Proxies must allow streaming without response buffering.

Text extraction (OCR) happens in the backend; the n8n AI workflows only receive the redacted JSON text.

## Database migrations

The EF migrations cover the application schema, including Google identities, transcript reports, multiple-choice assessment options, signup preferences and password reset codes. For a new empty database, run:

```powershell
dotnet run --project backend/CapstoneProjectBE -- --migrate-database
```

Existing LevelUp databases created before this baseline must be adopted once. The adoption command verifies all required legacy tables, adds the compatible missing columns/indexes and records the current EF baseline without recreating user data:

```powershell
dotnet run --project backend/CapstoneProjectBE -- --adopt-existing-database
```

Back up the database before adoption. The command refuses incomplete schemas and refuses databases that already contain the baseline migration. It runs the compatibility changes and migration-history update in one transaction, then applies any later migrations. After adoption, use `--migrate-database` for future migrations. The former manual SQL setup scripts and feature-specific setup switches are no longer used.

The `AddDiscoveredLearningResources` migration adds canonical URLs and provider-owned resource identifiers. If migrations cannot be run through the application command, execute the equivalent idempotent script at `backend/scripts/20261026-add-discovered-learning-resources.sql` against the same PostgreSQL database. Use one method only; both record the EF migration history entry.

The application accesses Postgres through the authenticated ASP.NET backend, not through Supabase's public Data API. The baseline enables row-level security on user-owned tables and revokes direct `anon` and `authenticated` table access. If a future feature intentionally uses the Data API, add narrowly scoped grants and ownership policies in a new reviewed migration.

## AI workflows

CV and transcript analysis send OCR-extracted JSON text to n8n; MIRA, knowledge checks and roadmap generation send bounded JSON payloads. All five workflows use `N8nAnalysisService` with the canonical `X-LevelUp-API-Key` header. Configure their shared secret with `backend/scripts/Set-N8nAnalysis.ps1` or deployment secret settings. See [the exact request, response and deployment contracts](n8n/README.md). Remote workflows must be published and smoke-tested after configuration.

MIRA chat is served by the authenticated `/api/mira` endpoint and the `N8n:MiraUrl` workflow. It uses the same n8n header secret; the deployed workflow must return `{ "success": true, "message": { "content": "..." } }`. Set the shared `N8n:AuthHeaderValue` secret (or `N8n:MiraAuthHeaderValue`) and publish the `mira` webhook before testing chat.
