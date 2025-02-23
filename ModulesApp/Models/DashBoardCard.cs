using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models;

[Table("DashBoardCard")]
public class DashBoardCard
{
    [Key]
    public long Id { get; set; }
    public string? Name { get; set; } = default;

    public string? Value { get; set; } = default;

    public long DashboardId { get; set; }
    [ForeignKey("DashboardId")]
    public Dashboard Dashboard { get; set; } = default!;
}