using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ModulesApp.Models.BackgroundServices;

namespace ModulesApp.Models;

[Table("Action")]
public class DbAction
{
    [Key]
    public long Id { get; set; }

    public string Key { get; set; } = default!;
    public object Value { get; set; } = default!;

    public long ModuleId { get; set; }
    [ForeignKey("ModuleId")]
    public DbModule? Module { get; set; }

    public long BackgroundServiceId { get; set; }
    [ForeignKey("BackgroundServiceId")]
    public DbBackgroundService? BackgroundService { get; set; }
}
