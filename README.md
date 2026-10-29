# LevelUp Capstone

https://levelupnow.co.za

LevelUp is a career-development platform with a Next.js frontend and an
ASP.NET Core backend. Both applications live in this repository but remain
separate projects that run as separate development processes.

## Repository structure

```text
Level-Up-Final-Project/
|-- frontend/              # Next.js application
|-- backend/               # ASP.NET Core solution and API project
|-- .gitignore
`-- README.md
```

## Prerequisites

- Node.js and npm
- .NET SDK required by the backend project
- PostgreSQL
- Git

## Local configuration

Create `frontend/.env.local` after copying the frontend project:

```env
NEXT_PUBLIC_API_BASE_URL=https://localhost:7286
```

Do not commit `.env.local` or any credentials. Backend secrets such as the
database connection string, JWT signing key, and storage credentials should be
configured with .NET User Secrets or environment variables.

The backend CORS policy must allow the local frontend origin:

```text
http://localhost:3000
```

## Running locally

Run the applications in two terminals.

Frontend:

```powershell
cd frontend
npm install
npm run dev
```

Backend:

```powershell
cd backend
dotnet restore
dotnet run --project .\CapstoneProjectBE\CapstoneProjectBE.csproj
```

The expected local addresses are:

- Frontend: `http://localhost:3000`
- Backend API: `https://localhost:7286`
- Swagger: `https://localhost:7286/swagger`

## Development workflow

Create feature branches from `main`, keep credentials out of Git, and verify
both projects before opening a pull request:

```powershell
cd frontend
npm run typecheck
npm test
npm run build

cd ..\backend
dotnet test CapstoneProjectBE.sln --configuration Release
```
