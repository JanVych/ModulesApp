# Architecture

## Directory Layout

```
ModulesApp/
├── Components/
│   ├── Pages/              # Blazor page components (Modules, Dashboards, Tasks, etc.)
│   ├── ServerTasks/        # Node/link/port components for the visual task editor
│   ├── DashBoardEntities/  # Dashboard widget components
│   ├── BackgroundServices/ # Service configuration UI
│   ├── Dialog/             # MudBlazor dialog components
│   ├── Account/            # Login / Register pages
│   └── Layout/             # MainLayout, NavMenu
├── Models/
│   ├── ServerTasks/        # DbTask, DbTaskNode (abstract), DbTaskLink, DbTaskPort
│   ├── Dasboards/          # DbDashboard, DbDashboardEntity (abstract), Entities/
│   ├── BackgroundServices/ # DbBackgroundService, service implementations
│   └── ModulesPrograms/    # DbModuleProgram, firmware models
├── Services/
│   ├── Data/               # Repositories (one per aggregate)
│   ├── BackgroundServiceManager.cs
│   ├── ModuleProgramManager.cs
│   ├── ContextService.cs
│   └── NotifyService.cs
├── Data/
│   └── AppDbContext.cs     # EF Core context + migrations
├── Controllers/            # REST: FirmwareController, ModulesController
├── Helpers/                # DataConvertor, ModbusRtuUdp
├── Interfaces/             # IDbNode
└── Types/
```

## Service Layer

### Repositories (`Services/Data/`)
Each repository is scoped (`AddScoped`) and follows the same structure:
- Constructor takes `IDbContextFactory<AppDbContext>` and `NotifyService`
- Each method opens its own short-lived context: `await using var context = await _dbContextFactory.CreateDbContextAsync()`
- Eager loading uses `.Include()` + `.AsSplitQuery()` to avoid Cartesian product
- Write methods call `_notifyService.Notify*Changed()` after saving

Repositories: `ModuleRepository`, `DashboardRepository`, `ServerTaskRepository`, `BackgroundServiceRepository`, `ActionRepository`, `ModuleProgramRepository`

### Managers
- `BackgroundServiceManager` — orchestrates Quartz job lifecycle (launch, pause, resume, delete)
- `ModuleProgramManager` — handles firmware compilation and deployment

### Singletons
- `NotifyService` — registered as `AddSingleton`; exposes C# events that Blazor components subscribe to for real-time UI updates

### Context
- `ContextService` — scoped orchestrator; entry point for user-triggered dashboard interactions:
  ```csharp
  await DashboardRepository.UpdateAsync(entity, key, value);
  await ServerTaskRepository.ExecuteTasksAsync(this, entity);
  ```

## Blazor Component Pattern

Components inject concrete services (not interfaces) via `@inject`:
```csharp
@inject ModuleRepository _moduleService
@inject NotifyService _notifyService
@inject IDialogService _dialogService
```

Lifecycle:
- `OnInitializedAsync()` — load data, subscribe to `NotifyService` events
- `Dispose()` — unsubscribe from events

## Domain Areas

### ServerTasks
Node-graph execution engine. A `DbTask` owns a graph of `DbTaskNode` (abstract, polymorphic) connected by `DbTaskLink`. Execution is async, supports cycle detection. Triggered by modules, background services, or dashboard entity interactions.

### Dashboards
Runtime UI system. `DbDashboard` owns a list of `DbDashboardEntity` (abstract, polymorphic). Entities store runtime state in `Dictionary<string, object?> Data`. Entity types: `KeyValue`, `Switch`, `Button`, `Frame`, `ValueSetter`, `DataList`, `LineChart24h`, `AccumulationTank`, `ToggleGroup`, `RoomsTemp`.

### BackgroundServices
Quartz-scheduled jobs. Types: `Cron`, `Http`, `Goodwe` (solar inverter), `OteElectricityDam`. Config stored in `Dictionary<string, object?> ConfigurationData`, last data in `MessageData`.
