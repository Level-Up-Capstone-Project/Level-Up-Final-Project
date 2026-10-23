# Level Up Frontend

A responsive career development application built with Next.js and TypeScript. The existing dark screen designs use React Native Web components inside Next.js.

## Requirements

- Node.js 22.13 or later
- npm

## Install and run

Install all dependencies with one command:

```bash
npm ci
```

Create `.env.local`:

```dotenv
NEXT_PUBLIC_API_BASE_URL=https://localhost:7286
```

Start the frontend:

```bash
npm run dev
```

Open http://localhost:3000 in a browser. Expo Go is no longer required.

## Production and validation

```bash
npm run typecheck
npm test
npm run build
npm start
```

`npm start` serves a completed production build on port 3000.

## Backend integration

The backend is expected at https://localhost:7286 with Swagger at https://localhost:7286/swagger. Its local CORS policy must allow http://localhost:3000, and its HTTPS certificate must be trusted by the browser.

Connected flows:

- Registration, email verification, resend verification (60-second cooldown), and sign-in use `/api/auth`.
- Jobs load every page from `/api/jobs`; details load on selection. `/api/job-matches` supplies match scores and skill evidence. Saved jobs use authenticated GET, POST and DELETE `/api/saved-jobs` requests.
- CV analysis uploads multipart field `file` to `/api/cvs`, then posts to `/api/cvs/{cvid}/analyse`. Selecting a document automatically uploads and starts analysis after consent. CVs must be PDF, at most 5 MiB.
- PDF transcripts automatically upload to `/api/transcripts/analyse`; the backend sends the original PDF to the authenticated n8n AI workflow for conservative subject/mark extraction. Results are saved separately and do not replace the current CV.
- Both existing Google buttons use the backend OAuth popup flow once its Google credentials and identity mapping table are configured; see `../backend/README.md`.
- MIRA chat uses the authenticated `/api/mira` endpoint and its published, authenticated n8n workflow.
- Forgot-password sends a time-limited email code through Bird; enter the code and a new password to complete recovery.
- JWTs stay in application memory and are passed as bearer tokens for protected calls. Reloading requires sign-in again.
- Validation errors and Problem Details appear in the UI. A missing match score is shown as unavailable.

Current integration notes:

- Profile Settings loads and saves backend profile fields and the selected target career. Signup industry, phone, target roles and work-area preferences are stored with the profile and restored after signing in again.
- Learning roadmaps, resource completion and saved resources load from authenticated backend endpoints. When no roadmap or target career exists, the mission screen loads skill-based career suggestions; choosing one requests roadmap generation through the backend's n8n workflow. A saved target career also generates its roadmap automatically when one does not yet exist.
- The dashboard/readiness adapter and Skills Match screen are connected. Skills Match loads the selected career's weighted gap and status breakdown, or the user's tracked skills when no career is selected. The mission-completion Knowledge Check starts a generated assessment from the current roadmap item and submits answers to the authenticated assessment API.

Run the backend with the HTTPS launch profile (requires the .NET 8 SDK and configured database/JWT secrets):

```powershell
dotnet dev-certs https --trust
dotnet run --project ../backend/CapstoneProjectBE --launch-profile https
```

Copy `.env.example` to `.env.local` before starting Next.js, and restart Next.js after changing its public environment variables. The URL is the backend origin, without an `/api` suffix.

To check the integration against a running backend, register a new email, obtain the development email verification code, verify, sign out and sign in again. Confirm signup preferences remain after signing in. Then browse jobs, open details, save/unsave a job, and select a PDF under 5 MiB with consent to confirm analysis starts automatically. Test password recovery with a Bird-verified sender and test MIRA and knowledge checks only after the corresponding n8n workflows are published and authenticated. Automated transport tests use controlled responses; they do not establish live database, storage, email or workflow connectivity.

## Optional area search

Set `NEXT_PUBLIC_PLACES_API_URL` to an available area search proxy. The local proxy can be started with `npm run start:places` when its required configuration is supplied.
