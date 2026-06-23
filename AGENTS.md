# Repository Guidelines

## Project Structure & Module Organization

This is a .NET solution for the WiSave Incomes service. Production code lives under `src/`, with separate projects for API, console tooling, workers, contracts, domain/application layers, infrastructure, projections, and migrations. Tests live under `tests/` and mirror the project they validate, for example `tests/WiSave.Incomes.WebApi.Tests` and `tests/WiSave.Incomes.Core.Application.Tests`. Docker entrypoints are defined by `Dockerfile.WebApi`, `Dockerfile.Worker.Domain`, and `Dockerfile.Worker.Projections`; local service dependencies are in `docker-compose.yml`.

## Build, Test, and Development Commands

- `dotnet restore WiSave.Incomes.slnx` restores all solution packages.
- `dotnet build WiSave.Incomes.slnx` builds all projects with warnings treated as errors.
- `dotnet test WiSave.Incomes.slnx` runs the xUnit test suite.
- `dotnet run --project src/WiSave.Incomes.WebApi/WiSave.Incomes.WebApi.csproj` starts the web API locally.
- `dotnet run --project src/WiSave.Incomes.Console/WiSave.Incomes.Console.csproj -- <command>` runs console maintenance commands.
- `docker compose up --build` starts the API, workers, PostgreSQL databases, and RabbitMQ-backed wiring expected by the compose file.

## Coding Style & Naming Conventions

Projects target `net10.0` with nullable reference types, implicit usings, centralized package versions, and code style enforcement enabled in `Directory.Build.props` and `Directory.Packages.props`. Follow standard C# naming: PascalCase for types, methods, records, and public members; camelCase for locals and parameters; interfaces prefixed with `I`. Keep namespace and folder names aligned with the owning project area, such as `Authorization`, `Endpoints`, `Postgres`, or `Handlers`.

## Testing Guidelines

Tests use xUnit with `Microsoft.NET.Test.Sdk` and `coverlet.collector`. Place new tests in the matching project under `tests/`, and name test classes after the subject, e.g. `CategoryEndpointsTests` or `CreateIncomeCommandHandlerTests`. Use `[Fact]` for single scenarios and `[Theory]` for parameterized cases. Prefer focused tests around handlers, endpoints, repositories, authorization, and EF model configuration.

Treat most tests created only to drive TDD or to reproduce an implementation issue as temporary working artifacts. Do not persist low-value, implementation-coupled tests in the final change just because they helped during development. Before keeping agent-created tests in the final diff, ask the user whether they should be persisted; default to not persisting them unless the user explicitly approves. Keep tests when they protect meaningful business behavior, public contracts, regressions with real risk, authorization/security rules, persistence mappings, or integration boundaries; otherwise remove the temporary test before finishing.

## Commit & Pull Request Guidelines

Recent history uses concise Conventional Commit-style messages such as `feat(categories): handle category creation commands` and `ci(contracts): publish incomes contracts package`. Use a clear scope when helpful, and do not prefix commits, branches, or PR titles with `[codex]` or `codex/` unless explicitly requested. Pull requests should describe the change, list verification performed, link related issues when applicable, and include screenshots only for visible API documentation or UI-facing changes.

## Security & Configuration Tips

Do not commit secrets. `docker-compose.yml` uses development credentials and reads NuGet credentials from `${HOME}/.nuget/NuGet/NuGet.Config` as a Docker secret. Keep environment-specific overrides in local configuration files or environment variables.
