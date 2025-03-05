using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.Dasboards;

public enum DashboardEntityType
{
    BasicCard,
}

[Table("DashBoardEntity")]
public class DbDashboardEntity
{
    [Key]
    public long Id { get; set; }

    public DashboardEntityType Type { get; set; }

    public string Name { get; set; } = default!;

    public Dictionary<string, object> Data { get; set; } = [];

    public long DashboardId { get; set; }
    [ForeignKey("DashboardId")]
    public DbDashboard Dashboard { get; set; } = default!;
}
