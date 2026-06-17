# Coffee Mug Assignment — Inventory API

A small inventory and orders API built with .NET 10, EF Core, and MediatR.

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) — the app runs on PostgreSQL, which is started via Docker

### 1. Start PostgreSQL

The app runs on PostgreSQL only, so start the database before the API:

```bash
docker compose up -d
```

This starts Postgres 17 on port `5432` (database `inventorydb`, username and password both `inventory`). The connection string in [appsettings.json](CoffeeMugAssignment/appsettings.json) already points at it:

```json
"DefaultConnection": "Host=localhost;Port=5432;Database=inventorydb;Username=inventory;Password=inventory"
```

To stop it later, run `docker compose down` — add `-v` to also delete the data volume.

### 2. Build and run

```bash
dotnet restore
dotnet build
dotnet run --project CoffeeMugAssignment
```

The app runs in the `Development` environment by default. On startup it applies migrations and seeds sample data, so the API has something to work with on first run.

> Migrations run automatically only in Development. In production you'd apply them as a separate deploy step.

## HTTP and HTTPS

The API ships with two launch profiles (see [launchSettings.json](CoffeeMugAssignment/Properties/launchSettings.json)):

| Profile | URLs |
| --- | --- |
| `http`  | `http://localhost:5280` |
| `https` | `https://localhost:7299` and `http://localhost:5280` |

By default `dotnet run` uses the `http` profile. To run over HTTPS:

```bash
dotnet run --project CoffeeMugAssignment --launch-profile https
```

HTTPS uses the local ASP.NET dev certificate. If your browser or client doesn't trust it, trust it once with:

```bash
dotnet dev-certs https --trust
```

In Development the API also serves interactive OpenAPI docs via Scalar at `/scalar/v1`, and there's a ready-to-run request collection in [CoffeeMugAssignment.http](CoffeeMugAssignment/CoffeeMugAssignment.http).

## Tests

```bash
dotnet test
```

Integration tests run against a real PostgreSQL instance using [Testcontainers](https://testcontainers.com/), so **Docker must be running**. A container is started automatically, the real EF Core migrations are applied to it, and it's torn down afterwards — there's nothing to set up by hand. The pure unit tests (such as pricing) don't need Docker.

## Commit Conventions

This repo follows [Conventional Commits](https://www.conventionalcommits.org/). Messages are linted both locally (a Husky git hook) and in CI (a GitHub Actions check on every pull request), using [commitlint](https://commitlint.js.org/) with the `config-conventional` rules.

### One-time setup

The local hook is powered by a small Node toolchain. After cloning, install it once:

```bash
npm install
```

This pulls in `commitlint` and `husky` (dev dependencies only) and runs the `prepare` script, which installs the git hook into `.husky/`. From then on, every `git commit` is checked automatically.

> Node.js 18+ is required for the local hook. If you don't have Node installed, you can skip this — the CI check still enforces the convention on pull requests. The hook only affects commits, not the .NET build or runtime.

### Message format

```
<type>(<optional scope>): <short summary>
```

Common types: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`, `revert`. A breaking change adds a `!` (e.g. `feat!: ...`) or a `BREAKING CHANGE:` footer.

Examples:

```
feat(orders): add cancel endpoint
fix: return stock to products when an order is deleted
docs: document commit conventions
```

A commit template with the full cheat sheet lives in [.gitmessage](.gitmessage). To use it as your editor prefill:

```bash
git config commit.template .gitmessage
```

## Placing an Order

1. Call `GET /products` and copy a real product `id`.
2. Pick one of the seeded customer ids:
   - US: `11111111-1111-1111-1111-111111111111`
   - Europe: `22222222-2222-2222-2222-222222222222`
   - Asia: `33333333-3333-3333-3333-333333333333`
3. Call `POST /orders` with that `customerId` and your product lines.

There's a worked example in [CoffeeMugAssignment.http](CoffeeMugAssignment/CoffeeMugAssignment.http).

## Design Notes

### Assumptions

- Customers are seeded, since there are no customer endpoints.
- A customer's region lives on the customer entity; it isn't fetched from anywhere else.
- Discounts apply per order line, and only the highest one wins (they don't stack).
- Black Friday is the fourth Friday in November.

### Design decisions

- The app runs on PostgreSQL, and the integration tests run against a real Postgres via Testcontainers — so they hit the same engine and exercise the real migrations.
- Beyond the endpoints required by the task, products also support get-by-id, update, and delete, and orders support list, get-by-id, and delete.
- Sample products are seeded in Development (only when the table is empty), so the API isn't empty on first run.
- The discount date comes from an injectable clock, so tests can control "today".

### Trade-offs

- Orders have no edit (PUT) on purpose. Editing a placed order is messy because stock and prices are already locked in, so you cancel it and create a new one instead. Deleting an order returns its stock to the products.
- Migrations run on startup, but only in Development. In production you'd run them as a separate step.
- Products are seeded in code rather than with `HasData`, since they're regular data rather than fixed reference data.
- Integration tests share one Postgres container for speed, which is fine at this size. As the suite grows I'd add per-test isolation (Respawn or transaction rollback) and lean more on fast unit tests, since spinning up containers gets slower and depends on Docker being available in CI.
