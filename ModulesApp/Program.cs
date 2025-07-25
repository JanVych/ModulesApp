using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModulesApp.Components;
using ModulesApp.Data;
using ModulesApp.Models.BackgroundServices;
using ModulesApp.Services;
using ModulesApp.Services.Data;
using MudBlazor.Services;
using Quartz;
using System.Security.Claims;
using static Quartz.Logging.OperationName;


namespace ModulesApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddMudServices();

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        // database
        var connectionString = NormalizePath(builder.Configuration.GetConnectionString("SQLiteDb"));
        builder.Services.AddDbContextFactory<SQLiteDbContext>(options =>
        {
            options.UseSqlite(connectionString); 
        });

        builder.Services.AddScoped(provider =>
            provider.GetRequiredService<IDbContextFactory<SQLiteDbContext>>().CreateDbContext());

        // quartz, background services
        builder.Services.AddQuartz(q =>
        {
            q.Properties["quartz.jobStore.misfireThreshold"] = "1000";
            //q.Properties["quartz.threadPool.threadCount"] = "5";
        });
        builder.Services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        builder.Services.AddControllers();

        // identity
        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 8;
            options.SignIn.RequireConfirmedAccount = false;
        }).AddEntityFrameworkStores<SQLiteDbContext>();

        //data services
        builder.Services.AddScoped<ModuleService>();
        builder.Services.AddScoped<ActionService>();
        builder.Services.AddScoped<DashboardService>();
        builder.Services.AddScoped<ServerTaskService>();
        builder.Services.AddScoped<BackgroundServiceService>();
        builder.Services.AddScoped<ModuleProgramService>();

        builder.Services.AddScoped<ContextService>();

        builder.Services.AddScoped<ModuleProgramManager>();
        //builder.Services.AddScoped<ModuleProgramManager>(s => new ModuleProgramManager(s));

        builder.Services.AddScoped<BackgroundServiceManager>();

        //builder.Services.AddSingleton<BackgroundServiceManager>();
        builder.Services.AddSingleton<NotifyService>();

        var app = builder.Build();

        app.MapControllers();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        // Map the POST /Account/Logout endpoint
        var accountGroup = app.MapGroup("/Account");
        accountGroup.MapPost("/Logout", async (
            ClaimsPrincipal user,
            [FromServices] SignInManager<IdentityUser> signInManager,
            [FromForm] string returnUrl) =>
        {
            await signInManager.SignOutAsync();
            return TypedResults.LocalRedirect($"~/{returnUrl}");
        });

        using (var scope = app.Services.CreateScope())
        {
            var scopedService = scope.ServiceProvider.GetRequiredService<BackgroundServiceManager>();
            scopedService.LaunchAsync().GetAwaiter().GetResult();
        }

        app.Run();
    }

    static string? NormalizePath(string? connectionString)
    {
        return connectionString?.Replace("\\", Path.DirectorySeparatorChar.ToString());
    }
}
