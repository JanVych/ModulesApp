using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ModulesApp.Models;

[Table("ModuleAction")]
public class ModuleAction
{
    [Key]
    public long Id { get; set; }
    //public Dictionary<string, object> JsonData { get; set; } = default!;

    public string Key { get; set; } = default!;
    public object Value { get; set; } = default!;

    public long ModuleId { get; set; }
    [ForeignKey("ModuleId")]
    public Module Module { get; set; } = default!;

}
