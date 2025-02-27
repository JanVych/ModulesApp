using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models;

[Table("Dashboard")]
public class DbDashboard
{
    [Key]
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string? IconString { get; set; }
    public ICollection<DbDashBoardCard> Cards { get; set; } = [];
}
