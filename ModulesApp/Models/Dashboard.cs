using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models;

[Table("Dashboard")]
public class Dashboard
{
    [Key]
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string? IconString { get; set; }
    public ICollection<DashBoardCard> Cards { get; set; } = [];
}
