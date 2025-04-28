using Microsoft.EntityFrameworkCore;
using ModulesApp.Models;
using ModulesApp.Models.BackgroundServices;
using ModulesApp.Models.BackgroundServices.Servicves;
using ModulesApp.Models.Dasboards;
using ModulesApp.Models.Dasboards.Entities;
using ModulesApp.Models.ModulesPrograms;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Models.ServerTasks.Nodes;
using System.Text.Json;

namespace ModulesApp.Data;

public class SQLiteDb : DbContext
{
    protected readonly IConfiguration _configuration;

    public DbSet<DbModule> Modules { get; set; }

    public DbSet<DbDashboard> Dashboards { get; set; }
    public DbSet<DbDashboardEntity> DashboardEntities { get; set; }

    public DbSet<DbTaskNode> TaskNodes { get; set; }
    public DbSet<DbTaskLink> TaskLinks { get; set; }
    public DbSet<DbTask> Tasks { get; set; }

    public DbSet<DbBackgroundService> BackgroundServices { get; set; }

    public DbSet<DbAction> Actions { get; set; }

    public DbSet<DbModuleIDF> IDFs { get; set; }
    public DbSet<DbModuleFirmware> Firmwares { get; set; }
    public DbSet<DbModuleProgram> Programs { get; set; }
    public DbSet<DbModuleProgramFile> ProgramsFiles { get; set; }

    public SQLiteDb(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            //.UseLazyLoadingProxies()
            .UseSqlite(NormalizePath(_configuration.GetConnectionString("SQLiteDb")));
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        var SerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        builder.Entity<DbTaskLink>()
            .HasOne(link => link.Source)
            .WithMany(node => node.SourceLinks)
            .HasForeignKey(link => link.SourceNodeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<DbTaskLink>()
            .HasOne(link => link.Target)
            .WithMany(node => node.TargetLinks)
            .HasForeignKey(link => link.TargetNodeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<DbTaskNode>()
            .HasDiscriminator<string>("NodeType")
            .HasValue<DbConditionNode>("Condition")
            .HasValue<DbDataDisplayNode>("DataDisplay")
            .HasValue<DbFromMessageNode>("FromMessage")
            .HasValue<DbValueNode>("StaticData")
            .HasValue<DbSendMessageNode>("SendMessage")
            .HasValue<DbArrayOperationNode>("ArrayOperation")
            .HasValue<DbArithmeticOperationNode>("ArithmeticOperation");


        builder.Entity<DbAction>()
            .Property(p => p.Value)
            .HasConversion(
                v => JsonSerializer.Serialize(v, SerializerOptions),
                v => JsonSerializer.Deserialize<object>(v, SerializerOptions) ?? string.Empty);

        builder.Entity<DbModule>()
            .Property(p => p.Data)
            .HasConversion(
                v => JsonSerializer.Serialize(v, SerializerOptions),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, SerializerOptions) ?? new Dictionary<string, object>());


        builder.Entity<DbBackgroundService>()
            .HasDiscriminator<BackgroundServiceType>(nameof(DbBackgroundService.Type))
            .HasValue<DbGoodweBackgroundService>(BackgroundServiceType.Goodwe)
            .HasValue<DbTestBackgroundService>(BackgroundServiceType.Test);

        builder.Entity<DbBackgroundService>()
            .Property(p => p.Data)
            .HasConversion(
                v => JsonSerializer.Serialize(v, SerializerOptions),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, SerializerOptions) ?? new Dictionary<string, object>());


        builder.Entity<DbDashboardEntity>()
            .Property(p => p.Data)
            .HasConversion(
                v => JsonSerializer.Serialize(v, SerializerOptions),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, SerializerOptions) ?? new Dictionary<string, object>());

        builder.Entity<DbDashboardEntity>()
            .HasDiscriminator<DashboardEntityType>(nameof(DbDashboardEntity.Type))
            .HasValue<DbBasicCardEntity>(DashboardEntityType.BasicCard)
            .HasValue<DbDataListEntity>(DashboardEntityType.DataList)
            .HasValue<DbSwitchEntity>(DashboardEntityType.Switch)
            .HasValue<DbTemperaturesListEntity>(DashboardEntityType.TemperatureList)
            .HasValue<DbButtonEntity>(DashboardEntityType.Button)
            .HasValue<DbValueSetterEntity>(DashboardEntityType.ValueSetter);

        base.OnModelCreating(builder);
    }

    static string? NormalizePath(string? connectionString)
    {
        return connectionString?.Replace("\\", Path.DirectorySeparatorChar.ToString());
    }
}