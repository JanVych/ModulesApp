# Conventions

## Naming

| Target | Convention | Example |
|--------|-----------|---------|
| DB entity classes | `Db` prefix + PascalCase | `DbModule`, `DbTask`, `DbDashboardEntity` |
| Repository classes | noun + `Repository` | `ModuleRepository`, `DashboardRepository` |
| Manager classes | noun + `Manager` | `BackgroundServiceManager` |
| Service/utility classes | noun + `Service` | `NotifyService`, `ContextService` |
| Async methods | `Async` suffix | `GetAllAsync()`, `AddAsync()`, `DeleteAsync()` |
| Private fields | `_camelCase` | `_dbContextFactory`, `_notifyService` |
| Public properties | `PascalCase` | `Id`, `Name`, `CreatedAt` |

## Dependency Injection

- Services are registered and injected as **concrete types**, not interfaces
- Blazor components use `@inject` (not constructor injection)
- `NotifyService` is `AddSingleton`; all repositories and managers are `AddScoped`

## Data Access

- All repository methods create their own EF Core context scope:
  ```csharp
  await using var context = await _dbContextFactory.CreateDbContextAsync();
  ```
- Use `.Include()` + `.AsSplitQuery()` when loading related entities
- After any write, call the relevant `_notifyService.Notify*Changed()` to trigger UI refresh

## Blazor Components

- Subscribe to `NotifyService` events in `OnInitializedAsync()`
- Always unsubscribe in `Dispose()` to avoid memory leaks
- Use MudBlazor components for all UI elements (dialogs, buttons, tables, forms)
- Open dialogs via `IDialogService`; prefer `ShowAndReturnAsync()` helper from the dialog service

## Models

- Polymorphic entity hierarchies use EF Core TPH (single table, discriminator column)
- Dynamic/flexible data stored in `Dictionary<string, object?>` (e.g., `ConfigurationData`, `MessageData`, `Data`)
- Enums define type discriminators (e.g., `BackgroundServiceType`, `DashboardEntityType`)

## Nullable

Nullable reference types are enabled project-wide — always handle potential nulls explicitly.

## Git Workflow

- Branch naming: `feature/short-description` (e.g. `feature/dashboard-charts`)
- Never commit directly to `master`
- Always push the branch to origin and open a PR targeting `master`
