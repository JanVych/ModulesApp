using MudBlazor.Services;
using ModulesApp.Components;
using ModulesApp.Data;
using ModulesApp.Services;
using ModulesApp.Services.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

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

        builder.Services.AddSingleton<BackgroundServiceManager>();
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

        //app.Services.GetService<BackgroundServiceManager>();

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

        app.Run();
    }

    static string? NormalizePath(string? connectionString)
    {
        return connectionString?.Replace("\\", Path.DirectorySeparatorChar.ToString());
    }
}
