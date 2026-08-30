# GulfShort

A small URL shortener built for the Gulf Full Stack Developer assignment.

## Stack

- ASP.NET Core Web API / .NET 10
- React + TypeScript + Vite
- Ant Design
- Axios
- TanStack React Query
- xUnit
- In-memory storage

## Run locally

Start the API (the default HTTP profile uses port `5000`):

```bash
cd backend
dotnet run --project GulfUrlShortener.Api
```

In a second terminal, start the frontend:

```bash
cd frontend/gulf-url-shortener-ui
npm install
npm run dev
```

Open the Vite URL shown in the terminal, normally `http://localhost:5173`.

During local development, Vite proxies `/api` and `/r` to the backend on port `5000`, avoiding browser CORS and localhost resolution issues. If the API runs on another port, set `VITE_API_URL` before starting the frontend, for example `VITE_API_URL=http://localhost:5278 npm run dev`.

Run backend tests:

```bash
dotnet test backend/GulfUrlShortener.slnx
```

## API contract

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/links` | Create a link |
| GET | `/api/links` | List links and statistics |
| GET | `/api/links/{code}` | Get one link |
| PATCH | `/api/links/{code}/disable` | Disable a link |
| DELETE | `/api/links/{code}` | Delete a link |
| GET | `/r/{code}` | Resolve and redirect |

Create body:

```json
{
  "url": "https://example.com",
  "alias": "optional-alias",
  "defaultUrl": "https://example.com/default",
  "iosUrl": "https://example.com/ios",
  "androidUrl": "https://example.com/android"
}
```

`url` and all supplied destination URLs must be absolute HTTP or HTTPS URLs. A custom alias must be 3-32 characters containing only letters, numbers, `_`, or `-`. Duplicate aliases return `409 Conflict`. Disabled links return `410 Gone` from the redirect endpoint, while missing links return `404 Not Found`.

The redirect chooses iOS or Android destinations from the visitor's `User-Agent`, otherwise it uses `defaultUrl` or the original URL. Clicks are counted only for active links.

## Design notes

- `LinkService` owns business rules and depends on interfaces for storage and code generation.
- The frontend separates UI components, API access, and server-state management: Axios handles HTTP, while TanStack React Query caches links and invalidates the list after mutations.
- API errors are normalized through ASP.NET Core `ProblemDetails`, and click updates are synchronized for concurrent redirect requests.
- `InMemoryLinkRepository` uses `ConcurrentDictionary` and can be replaced by a database repository later.
- Automatic codes use a random six-character generator; aliases are a second code-generation path.
- No authentication or deployment infrastructure is included because it is outside the assignment scope.
- The base short-link URL is configurable in `backend/GulfUrlShortener.Api/appsettings.json`.

## AI session logs

The assignment permits AI assistance but asks for session logs. A development summary is included in [`ai-logs/session-summary.md`](ai-logs/session-summary.md).
