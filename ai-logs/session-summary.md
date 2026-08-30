# AI development session summary

This project was built with Codex using the assignment PDF as the source of requirements.

## Decisions recorded

1. Use Vite + React + TypeScript for a lightweight separate frontend and ASP.NET Core Web API for the backend.
2. Use Ant Design for the dashboard UI, forms, table, actions, confirmations, and feedback messages.
3. Use an in-memory repository because persistence is optional in the assignment, while keeping the repository behind an interface.
4. Implement automatic short codes and custom aliases behind a short-code generator abstraction.
5. Test URL validation, aliases, redirects, platform routing, click counting, disabling, and deleting.

## Verification

The final verification commands are documented in the root README: `dotnet test backend/GulfUrlShortener.slnx` and `npm run build` from `frontend/gulf-url-shortener-ui`.
