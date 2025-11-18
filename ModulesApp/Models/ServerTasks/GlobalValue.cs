using ModulesApp.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.ServerTasks;

[Table("GlobalValue")]
public class DbGlobalValue
{
    [Key]
    public long Id { get; set; }
    public string Group { get; set; } = string.Empty;
    public string Key { get; set; } = default!;
    public object? Value { get; set; } = default!;

    public NodeValueType Type { get; set; } = NodeValueType.String;

}
