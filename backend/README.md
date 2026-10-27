# Level Up Backend

The backend API for Level Up. It is built with ASP.NET Core and handles authentication, profiles, careers, skills, jobs, learning roadmaps and document analysis.

## Requirements

- .NET 8 SDK
- PostgreSQL
- A Supabase project for file storage

## Setup

From the `backend` folder, restore the project:

```powershell
dotnet restore
```

Add the required development secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_POSTGRES_CONNECTION_STRING" --project CapstoneProjectBE
dotnet user-secrets set "Jwt:SigningKey" "YOUR_BASE64_SIGNING_KEY" --project CapstoneProjectBE
dotnet user-secrets set "Supabase:SecretKey" "YOUR_SUPABASE_SECRET_KEY" --project CapstoneProjectBE
```

The JWT signing key must be valid Base64 and contain at least 32 bytes.

## Database setup

Create an empty PostgreSQL database and use its connection string in the setup above.

Create the tables and starter data:

```powershell
dotnet run --project CapstoneProjectBE -- --migrate-database
```

The project migrations create the complete database structure and add the starter careers, skills, jobs and learning resources. No SQL files need to be imported manually.

## Run the API

Trust the local HTTPS certificate if needed:

```powershell
dotnet dev-certs https --trust
```

Start the backend:

```powershell
dotnet run --project CapstoneProjectBE --launch-profile https
```

The API will be available at:

- API: https://localhost:7286
- Swagger: https://localhost:7286/swagger
- Frontend: http://localhost:3000

The frontend origin is allowed by default during development.

## External services

Some features require additional services:

- **n8n** is used for CV analysis, transcript analysis, MIRA, roadmaps and knowledge checks.
- **Supabase** stores uploaded CVs and transcript reports.
- **Bird** sends verification and password-reset emails in production.
- **Google OAuth** supports Google sign-in and sign-up.
- **Adobe PDF Services** is used for transcript OCR and scanned documents.

These services are not required for every part of the API. Their credentials should be stored in .NET User Secrets or environment variables and must not be committed to Git.

For detailed n8n setup, see [n8n/README.md](n8n/README.md).

## Email during development

Development mode does not require Bird. Verification codes are written to the backend console.

To test Bird locally, run the setup script from the repository root:

```powershell
.\backend\scripts\Set-BirdEmail.ps1
```

## Google login

Google login requires a Google OAuth web client. Add the following redirect URI in Google Cloud:

```text
https://localhost:7286/signin-google
```

Then configure the client credentials:

```powershell
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID" --project CapstoneProjectBE
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET" --project CapstoneProjectBE
```

## Tests

Run the backend tests from the `backend` folder:

```powershell
dotnet test CapstoneProjectBE.sln
```

Uploaded CVs and transcripts must be PDF files no larger than 5 MiB.
