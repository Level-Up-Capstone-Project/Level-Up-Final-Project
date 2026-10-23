# Active n8n workflow contracts

LevelUp uses n8n for CV and transcript analysis, multiple-choice knowledge checks, roadmap generation, and MIRA chat.

Production webhooks use the same canonical header name:

```text
X-LevelUp-API-Key: <server-side secret>
```

The secret belongs only in backend User Secrets or deployment settings. Configure all workflows locally with:

```powershell
powershell -ExecutionPolicy Bypass -File backend/scripts/Set-N8nAnalysis.ps1
```

For Azure, configure `N8n__AuthHeaderName=X-LevelUp-API-Key`, secret `N8n__AuthHeaderValue`, and the workflow URL settings from `appsettings.json`. Per-workflow `AuthHeaderName` and `AuthHeaderValue` settings can override the shared credential.

The backend extracts the text of the uploaded PDF itself (embedded text first, Adobe PDF Services OCR for scanned pages), redacts personal details, and sends it to n8n as an authenticated JSON request. Workflows never receive the PDF. Treat workflow execution data as sensitive and restrict execution-log access. Catalogue skill reconciliation happens in the backend after the workflow response, so workflows should return evidence-backed skill names but do not need database skill IDs.

## CV analysis

The `cv-analysis` webhook receives JSON: `requestId`, `cvID`, optional `careerID` and `careerName`, `targetCareer`, `fileName` (always `document.pdf`) and `cvText`.

## Transcript analysis

The `transcript-analyze` webhook receives JSON:

```json
{
  "careerName": "Software Developer",
  "fileName": "document.pdf",
  "transcriptText": "..."
}
```

Validate it with `transcript-request.schema.json`, use `transcript-system-prompt.md`, and validate output with `transcript-response.schema.json`. Invalid requests return HTTP 400; model or parser failures return an appropriate 5xx response. Publish the production `/webhook/transcript-analyze` workflow after changing it.
The transcript response requires only `success`, `analysisType`, and `analysis`; individual analysis properties are optional and should be omitted when unavailable. Module/skill entries without both a name and evidence are ignored. The backend accepts either the direct response object or the n8n Code-node item envelope (`[{ "statusCode": 200, "response": { ... } }]`).

## Knowledge checks

The `knowledge-check-generate` webhook receives a backend-generated request ID, one catalogue skill, the specific completed recommended course, and the attempt number. The request has this shape:

```json
{
  "requestId": "uuid",
  "skill": { "skillID": "uuid", "skillName": "C#" },
  "course": {
    "resourceID": "uuid",
    "resourceName": "C# Fundamentals",
    "provider": "Microsoft Learn",
    "resourceType": "Course",
    "description": "..."
  },
  "attemptNumber": 1
}
```

Use `course` as context for the questions; it is selected server-side only from a course the authenticated learner completed in that roadmap step. Its response must match `knowledge-check-response.schema.json`. Only `MultipleChoice` questions are accepted. Every question must contain three to six distinct choices, and `correctAnswer` must equal one of those choices.

The backend generates questions before acquiring its short database allocation lock. If another request claims the predicted attempt number first, the backend regenerates the questions with the new attempt number rather than holding a transaction open during the n8n request.

## Roadmaps

The `roadmap-generate` webhook receives the target career and all its catalogue skills with `Missing`, `Partial`, or `Have` status. Its response must match `roadmap-response.schema.json` and include every `Missing` or `Partial` skill exactly once. It must not include a `Have` skill, invent skill IDs, or repeat a step title. Roadmap and step titles are limited to 150 characters to match the database. The backend owns resource selection and persistence; n8n only orders and describes the requested steps. The backend accepts either the direct response object or the n8n Code-node item envelope (`[{ "statusCode": 200, "response": { ... } }]`). If the workflow is unavailable or its response fails validation, the backend creates a deterministic roadmap from the same trusted missing and partial skills rather than persisting untrusted identifiers.

## MIRA chat

The `mira` webhook receives an authenticated user's message as `{ "message": "..." }`. Return JSON in this shape:

```json
{
  "success": true,
  "message": {
    "content": "A helpful response"
  }
}
```

The backend validates and returns only `message.content` to the client. Publish the workflow at the configured `N8n:MiraUrl` and protect it with the same backend-only shared API-key header as the other workflows. Do not echo secrets or hidden prompt content in responses.

## Verification

Before release, publish each workflow and test it with synthetic data using the exact header and schemas above. Then run one non-sensitive end-to-end CV, transcript, knowledge-check, roadmap, and MIRA request through the app. Unit tests validate backend payloads and responses but cannot prove that the remote n8n workflows were published correctly.

CV and transcript workflows have a 10-minute timeout; the other workflows have a 120-second timeout. All responses are limited to 1 MiB and redirects are disabled. Configure the reverse proxy accordingly and define minimal retention for n8n execution data because uploaded documents are sensitive.
