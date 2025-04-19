using ModulesApp.Models.ServerTasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.Dasboards;

public enum DashboardEntityType
{
    BasicCard,
    DataListCard,
    BasicSwitch,
}

[Table("DashBoardEntity")]
public abstract class DbDashboardEntity
{
    [Key]
    public long Id { get; private set; }

    public DashboardEntityType Type { get; protected set; }

    public string Name { get; set; } = string.Empty;
    
    public Dictionary<string, object> Data { get; set; } = [];

    public long DashboardId { get; set; }
    [ForeignKey("DashboardId")]
    public DbDashboard Dashboard { get; private set; } = default!;

    public ICollection<DbTask> ServerTasks { get; set; } = [];

    public abstract void UpdateData(Dictionary<string, object> data);
    public void UpdateData() => UpdateData(Data);

    public abstract void SaveData();

    //public override string ToString() => $"{Name}, id: {Id}";
    public override string ToString() => Name;
}
