using MudBlazor.Services;
using ModulesApp.Components;
using ModulesApp.Data;
using ModulesApp.Services;
using ModulesApp.Services.Data;
using Microsoft.EntityFrameworkCore;

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

        builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
        var connectionString = builder.Configuration.GetConnectionString("SQLiteDb");
        builder.Services.AddDbContextFactory<SQLiteDb>(options =>
        {
            options.UseSqlite(connectionString); 
        });

        builder.Services.AddControllers();

        builder.Services.AddScoped<ModuleService>();
        builder.Services.AddScoped<ActionService>();
        builder.Services.AddScoped<DashboardService>();
        builder.Services.AddScoped<ServerTaskService>();
        builder.Services.AddScoped<BackgroundServiceService>();
        builder.Services.AddScoped<ModuleProgramService>();

        builder.Services.AddScoped<ContextService>();

        builder.Services.AddScoped<ModuleProgramManager>();

        builder.Services.AddSingleton<BackgroundServiceManager>();
        builder.Services.AddSingleton<NotifyService>();

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

        app.Services.GetService<BackgroundServiceManager>();

        app.Run();
    }
}
