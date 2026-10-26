# TeamFlow API (.NET)

An ASP.NET Core 8 Web API for **TeamFlow**, a Jira-style project management app. The solution follows **Clean Architecture** with **CQRS** through MediatR, uses **PostgreSQL** with Entity Framework Core, and secures endpoints with **JWT** access and refresh tokens.

> **Status: work in progress.** The domain model and database schema cover users, projects, members, tasks, sprints, labels, comments, attachments, notifications and activity logs. The API currently exposes the **authentication** endpoints.

---

## Architecture

```
TeamFlow.WebAPI  ──►  TeamFlow.Application  ──►  TeamFlow.Domain
       │                                              ▲
       └──────────►  TeamFlow.Infrastructure  ────────┘
```

| Project | Responsibility |
|---|---|
| **TeamFlow.Domain** | Entities, enums, domain exceptions, repository and unit-of-work interfaces. No external dependencies. |
| **TeamFlow.Application** | Commands, handlers, DTOs and FluentValidation validators, plus MediatR pipeline behaviours. |
| **TeamFlow.Infrastructure** | EF Core `AppDbContext`, entity configurations, migrations, generic repository, unit of work, JWT service and BCrypt password hasher. |
| **TeamFlow.WebAPI** | Controllers, middleware, Serilog, authentication, CORS and Swagger. |

### Request pipeline

Every request sent through MediatR passes through three pipeline behaviours:

1. **ValidationBehavior**: runs FluentValidation validators and rejects invalid input.
2. **LoggingBehavior**: logs each request.
3. **TransactionBehavior**: wraps commands and logs failures.

`GlobalExceptionMiddleware` turns domain exceptions into consistent HTTP responses:

| Exception | Status |
|---|---|
| `ValidationException` | 400 Bad Request |
| `ForbiddenException` | 403 Forbidden |
| `NotFoundException` | 404 Not Found |
| `ConflictException` | 409 Conflict |
| Anything else | 500 Internal Server Error |

## Domain model

| Entity | Notes |
|---|---|
| `User` | Full name, email, password hash, avatar URL |
| `Project`, `ProjectMember` | Members have a `ProjectRole`: `Owner`, `Member`, `Viewer` |
| `ProjectTask` | `TaskStatus` (`ToDo`, `InProgress`, `Done`), `TaskPriority` (`Low`, `Medium`, `High`, `Critical`), position, due date, optional sprint |
| `TaskAssignee`, `TaskLabel` | Many-to-many links between tasks and users or labels |
| `Sprint`, `Label`, `Comment`, `Attachment` | Planning, tagging, discussion and files |
| `Notification` | `TaskAssigned`, `CommentAdded`, `StatusChanged`, `MemberAdded` |
| `ActivityLog` | Project activity history |
| `RefreshToken` | Stored refresh tokens with expiry and revocation |

## Tech stack

- .NET 8, ASP.NET Core Web API
- Entity Framework Core 8 with Npgsql (PostgreSQL 16)
- MediatR, FluentValidation, AutoMapper
- JWT Bearer authentication, BCrypt.Net
- Serilog (console and daily rolling files)
- Swagger / Swashbuckle
- Docker Compose for the database

## Getting started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Docker (for PostgreSQL), or a local PostgreSQL instance
- EF Core CLI: `dotnet tool install --global dotnet-ef`

### 1. Start the database

```bash
docker compose up -d
```

This starts PostgreSQL 16 on port `5432` with database `teamflow`. The connection string is in `TeamFlow.WebAPI/appsettings.json`; change the credentials there and in `docker-compose.yml` for anything beyond local development.

### 2. Set the JWT secret

The API refuses to start without a `JWT_SECRET` environment variable. Use a long random string (at least 32 characters for HMAC-SHA256).

```bash
# Linux / macOS
export JWT_SECRET="replace-with-a-long-random-secret"

# Windows PowerShell
$env:JWT_SECRET="replace-with-a-long-random-secret"
```

### 3. Apply migrations

```bash
dotnet ef database update --project TeamFlow.Infrastructure --startup-project TeamFlow.WebAPI
```

### 4. Run the API

```bash
dotnet run --project TeamFlow.WebAPI
```

The API listens on `http://localhost:5292` (and `https://localhost:7274` with the `https` profile). In development, Swagger UI is available at **http://localhost:5292/swagger**.

Logs are written to the console and to `logs/teamflow-<date>.log`.

### Configuration

| Setting | Where | Default |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | `appsettings.json` | Local PostgreSQL on port 5432 |
| `JwtSettings:Issuer` / `Audience` | `appsettings.json` | `TeamFlow` / `TeamFlowUsers` |
| `JWT_SECRET` | Environment variable | *(required)* |
| `Cors:AllowedOrigins` | `appsettings.json` | `http://localhost:3000` |

Add your frontend's URL to `Cors:AllowedOrigins` (for example `http://localhost:5173` for a Vite dev server).

## API

Base path: `/api/v1`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `POST` | `/auth/register` | — | Create an account, returns tokens and user |
| `POST` | `/auth/login` | — | Log in, returns tokens and user |
| `POST` | `/auth/refresh-token` | — | Exchange a refresh token for a new token pair |
| `POST` | `/auth/logout` | Bearer | Revoke a refresh token |
| `PUT` | `/auth/profile` | Bearer | Update full name and avatar |
| `PATCH` | `/auth/change-password` | Bearer | Change password |

**Tokens:** access tokens are valid for 15 minutes and refresh tokens for 7 days. Refresh tokens are rotated: each use revokes the old token and issues a new one.

## Project structure

```
TeamFlow/
├── TeamFlow.Domain/
│   ├── Entities/
│   ├── Enums/
│   ├── Exceptions/
│   └── Interfaces/            # IRepository<T>, IUnitOfWork
├── TeamFlow.Application/
│   ├── Behaviors/             # Validation, logging, transaction
│   ├── Commands/Auth/
│   ├── Handlers/Auth/
│   ├── Validators/Auth/
│   ├── DTOs/
│   └── Interfaces/            # IJwtService, IPasswordHasher, …
├── TeamFlow.Infrastructure/
│   ├── Identity/              # JWT service, BCrypt hasher
│   ├── Persistence/           # DbContext, configurations, repositories
│   ├── UnitOfWork/
│   ├── Logging/               # Activity log service
│   └── Migrations/
├── TeamFlow.WebAPI/
│   ├── Controllers/
│   ├── Middleware/
│   └── Program.cs
├── docker-compose.yml
└── TeamFlow.slnx
```

## Related

- [teamflow-frontend](https://github.com/eceakin/teamflow-frontend): React client for TeamFlow
