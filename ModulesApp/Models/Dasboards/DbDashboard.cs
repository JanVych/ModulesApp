using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.Dasboards;

[Table("Dashboard")]
public class DbDashboard
{
    [Key]
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string? IconString { get; set; }
    public List<DbDashboardEntity> Entities { get; set; } = [];

    public override string ToString() => Name;

    public void ReplaceEntity(DbDashboardEntity entity)
    {
        var index = Entities.FindIndex(e => e.Id == entity.Id);
        if (index >= 0)
        {
            Entities[index] = entity;
        }
    }
}
