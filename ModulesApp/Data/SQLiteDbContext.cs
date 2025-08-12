using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModulesApp.Interfaces;
using ModulesApp.Models;
using ModulesApp.Models.BackgroundServices;
using ModulesApp.Models.Dasboards;
using ModulesApp.Models.Dasboards.Entities;
using ModulesApp.Models.ModulesPrograms;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Models.ServerTasks.Nodes;
using System.Reflection.Emit;
using System.Text.Json;

namespace ModulesApp.Data;

public class SQLiteDbContext(DbContextOptions options) : IdentityDbContext(options)
{
    public DbSet<DbModule> Modules { get; set; }

    public DbSet<DbDashboard> Dashboards { get; set; }
    public DbSet<DbDashboardEntity> DashboardEntities { get; set; }

    public DbSet<DbTaskNode> TaskNodes { get; set; }
    public DbSet<DbTaskLink> TaskLinks { get; set; }
    public DbSet<DbTask> Tasks { get; set; }

    public DbSet<DbBackgroundService> BackgroundServices { get; set; }

    public DbSet<DbAction> Actions { get; set; }

    //public DbSet<DbModuleIDF> IDFs { get; set; }
    public DbSet<DbModuleFirmware> Firmwares { get; set; }
    public DbSet<DbModuleProgram> Programs { get; set; }
    public DbSet<DbModuleProgramFile> ProgramsFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var SerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        builder.Entity<DbTask>()
            .HasOne(t => t.Module)
            .WithMany(m => m.ServerTasks)
            .HasForeignKey(t => t.ModuleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<DbTask>()
            .HasOne(t => t.BackgroundService)
            .WithMany(m => m.ServerTasks)
            .HasForeignKey(t => t.BackgroundServiceId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<DbTask>()
            .HasOne(t => t.DashboardEntity)
            .WithMany(m => m.ServerTasks)
            .HasForeignKey(t => t.DashboardEntityId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<DbAction>()
            .HasOne(a => a.Module)
            .WithMany(m => m.Actions)
            .HasForeignKey(a => a.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<DbAction>()
            .HasOne(a => a.BackgroundService)
            .WithMany(b => b.Actions)
            .HasForeignKey(a => a.BackgroundServiceId)
            .OnDelete(DeleteBehavior.Cascade);

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
            .HasDiscriminator<NodeType>(nameof(DbTaskNode.Type))
            .HasValue<DbConditionNode>(NodeType.Condition)
            .HasValue<DbDataDisplayNode>(NodeType.DataDisplay)
            .HasValue<DbFromMessageNode>(NodeType.FromMessage)
            .HasValue<DbValueNode>(NodeType.Value)
            .HasValue<DbSendMessageNode>(NodeType.SendMessage)
            .HasValue<DbArrayOperationNode>(NodeType.ArrayOperation)
            .HasValue<DbArithmeticOperationNode>(NodeType.ArithmeticOperation)
            .HasValue<DbConvertToNode>(NodeType.ConvertTo)
            .HasValue<DbDateTimeNode>(NodeType.DateTime)
            .HasValue<DbFromAnyNode>(NodeType.FromAny)
            .HasValue<DbBooleanOperationNode>(NodeType.BooleanOperation)
            .HasValue<DbArithmeticSaturationNode>(NodeType.ArithmeticSaturation)
            .HasValue<DbBranchNode>(NodeType.Branch);

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
            .Property(p => p.MessageData)
            .HasConversion(
                v => JsonSerializer.Serialize(v, SerializerOptions),
                v => JsonSerializer.Deserialize<Dictionary<string, object?>>(v, SerializerOptions) ?? new Dictionary<string, object?>());
        builder.Entity<DbBackgroundService>()
            .Property(p => p.ConfigurationData)
            .HasConversion(
                v => JsonSerializer.Serialize(v, SerializerOptions),
                v => JsonSerializer.Deserialize<Dictionary<string, object?>>(v, SerializerOptions) ?? new Dictionary<string, object?>());


        builder.Entity<DbDashboardEntity>()
            .Property(p => p.Data)
            .HasConversion(
                v => JsonSerializer.Serialize(v, SerializerOptions),
                v => JsonSerializer.Deserialize<Dictionary<string, object?>>(v, SerializerOptions) ?? new Dictionary<string, object?>());

        builder.Entity<DbDashboardEntity>()
            .HasDiscriminator<DashboardEntityType>(nameof(DbDashboardEntity.Type))
            .HasValue<DbDataListEntity>(DashboardEntityType.DataList)
            .HasValue<DbSwitchEntity>(DashboardEntityType.Switch)
            .HasValue<DbTemperaturesListEntity>(DashboardEntityType.TemperatureList)
            .HasValue<DbButtonEntity>(DashboardEntityType.Button)
            .HasValue<DbValueSetterEntity>(DashboardEntityType.ValueSetter)
            .HasValue<DbLineChartEntity>(DashboardEntityType.LineChart)
            .HasValue<DbKeyValueEntity>(DashboardEntityType.KeyValue)
            .HasValue<DbFrameEntity>(DashboardEntityType.Frame);

        base.OnModelCreating(builder);
    }
}