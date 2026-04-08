# Project Summary

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core 9.0, Blazor Server |
| UI | MudBlazor 8.11.0 (Material Design) |
| Database | PostgreSQL via EF Core 9.0 + Npgsql 9.0.4 |
| Scheduling | Quartz 3.15.0 |
| Diagrams | Z.Blazor.Diagrams 3.0.3 |
| Charts | Blazor-ApexCharts 6.1.0 |
| Code editor | BlazorMonaco 3.3.0 |
| Auth | ASP.NET Identity |

**Version:** 1.2.0  
**Nullable:** enabled (strict null checks throughout)

## Key Features

- **Modules** — manage and control IoT devices; receive data via actions
- **Dashboards** — customizable runtime UI with interactive entity widgets (charts, buttons, switches, value setters, etc.)
- **ServerTasks** — visual node-graph automation engine; tasks execute as directed acyclic graphs triggered by modules, services, or dashboard interactions
- **BackgroundServices** — scheduled jobs (Quartz) that poll external sources: HTTP endpoints, Goodwe solar inverter, OTE electricity pricing, cron-only triggers
- **ModulePrograms** — firmware and IDF file management for IoT devices

## Deployment

- Target: Linux ARM64 (e.g. Raspberry Pi)
- Published as self-contained binary
- Managed as a `systemctl` service
- CI/CD via GitHub Actions (triggers on `v*.*.*` tags)
