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
    public ICollection<DbDashboardEntity> Entities { get; set; } = [];
}
