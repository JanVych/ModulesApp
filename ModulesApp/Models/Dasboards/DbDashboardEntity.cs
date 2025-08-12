using ModulesApp.Models.ServerTasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.Dasboards;

public enum DashboardEntityType
{
    KeyValue,
    Switch,
    Button,
    Frame,
    ValueSetter,
    DataList,
    LineChart,
    TemperatureList,
}

public enum EntityChartType
{
    MovingAverage24Hours,
}

[Table("DashBoardEntity")]
public abstract class DbDashboardEntity
{
    [Key]
    public long Id { get; private set; }

    public DashboardEntityType Type { get; protected set; }

    public string Name { get; set; } = $"Card_{Guid.NewGuid().ToString("N").Substring(0, 8)}";

    public Dictionary<string, object?> Data { get; set; } = [];

    public long DashboardId { get; set; }
    [ForeignKey("DashboardId")]
    public DbDashboard Dashboard { get; private set; } = default!;

    public long? ParentEntityId { get; set; }
    [ForeignKey("ParentEntityId")]
    public List<DbDashboardEntity> ChildEntities { get; set; } = [];

    public List<DbTask> ServerTasks { get; set; } = [];

    // Update the entity's state based on a key-value pair
    public virtual void UpdateState(string key, object? value, bool toDatabase = false){}

    // Update the entity's properties from the Data property
    public virtual void LoadState(){}

    // Used to save data back to the Data property
    public virtual void SaveToData(){}
    public override string ToString() => Name;


    public void ReplaceChildren(DbDashboardEntity entity)
    {
        var index = ChildEntities.FindIndex(x => x.Id == entity.Id);
        if (index >= 0)
        {
            ChildEntities[index] = entity;
        }
    }
}
