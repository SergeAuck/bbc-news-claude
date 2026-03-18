# AUS News - BBC Headlines Viewer

## Overview

A full-stack app that displays top BBC News headlines using NewsAPI.org. Backend caches results for 3 minutes to minimize API calls.

## Tech Stack

- **Backend**: .NET 9 Minimal API (`backend/AusNews/`)
- **Frontend**: React 19 + TypeScript + Vite 6 (`frontend/`)
- **Testing**: xUnit + Moq (backend), Vitest + Testing Library (frontend)
- **CI/CD**: GitHub Actions (`.github/workflows/`)

## Project Structure

```
backend/
  AusNews/              # Main API project
    Program.cs          # Minimal API setup, single endpoint /api/news
    Services/NewsService.cs  # Fetches from NewsAPI with IMemoryCache (3 min TTL)
    Models/NewsResponse.cs   # DTOs: NewsApiResponse, Article, Source
  AusNews.Tests/        # xUnit tests (5 tests: unit + integration)
  AusNews.sln
frontend/
  src/
    App.tsx             # Main app with react-query, animated layout
    api.ts              # API client, points to backend at localhost:5262
    types.ts            # TypeScript interfaces for NewsAPI response
    components/
      NewsCard.tsx      # Animated article card with image, hover effects
      NewsTicker.tsx    # Scrolling breaking news banner (framer-motion)
      LoadingSpinner.tsx
    App.test.tsx        # 4 tests: header, loading, articles, error
    components/NewsCard.test.tsx  # 5 tests
  vitest.config.ts
.github/workflows/
  frontend.yml          # Lint + Test + Build (on push/PR to main, frontend/** paths)
  backend.yml           # Restore + Build + Test (on push/PR to main, backend/** paths)
```

## API Key Management

- **NewsAPI key**: Stored securely, NEVER commit to repo
- **Local dev**: Stored in .NET User Secrets (already configured, UserSecretsId in .csproj)
- **CI/CD**: GitHub repository secret `NEWSAPI_KEY`
- **appsettings.json**: Contains placeholder `YOUR_API_KEY_HERE` (safe to commit)
- User secrets override appsettings.json via .NET configuration hierarchy

## Running Locally

```bash
# Backend (runs on http://localhost:5262)
cd backend/AusNews && dotnet run

# Frontend (runs on http://localhost:5173)
cd frontend && npm run dev
```

## Key Dependencies

- **Backend**: Microsoft.Extensions.Caching.Memory (in-memory cache)
- **Frontend**: framer-motion (animations), @tanstack/react-query (data fetching + client-side cache)
- **Test**: Moq, Microsoft.AspNetCore.Mvc.Testing, vitest, @testing-library/react, jsdom@24

## Known Issues / Notes

- Vite 8 has rolldown native binding issues on Windows with Node 22; pinned to Vite 6
- jsdom@27 has ESM compatibility issues; pinned to jsdom@24
- Frontend API base URL is in `frontend/src/api.ts` (defaults to localhost:5262)
- Backend CORS allows localhost:5173 and localhost:3000
- React Query refetches every 3 minutes (matching backend cache TTL)

## Commands

```bash
# Backend tests
cd backend && dotnet test AusNews.Tests/AusNews.Tests.csproj

# Frontend tests
cd frontend && npx vitest run

# Frontend lint
cd frontend && npx eslint .

# Frontend build
cd frontend && npm run build
```
