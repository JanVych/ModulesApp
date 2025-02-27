using MudBlazor.Services;
using ModulesApp.Components;
using ModulesApp.Data;
using ModulesApp.Services;
using ModulesApp.Services.Data;

namespace ModulesApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add MudBlazor services
        builder.Services.AddMudServices();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddDbContextFactory<SQLiteDb>();

        builder.Services.AddControllers();

        builder.Services.AddSingleton<ModuleService>();
        builder.Services.AddSingleton<ModuleActionService>();
        builder.Services.AddSingleton<DashboardService>();
        builder.Services.AddSingleton<ServerTaskService>();

        builder.Services.AddSingleton<ServerContextService>();

        builder.Services.AddSingleton<BackgroundServiceManager>();

        builder.Services.AddSingleton<FirmwareService>();

        var app = builder.Build();

        app.MapControllers();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
