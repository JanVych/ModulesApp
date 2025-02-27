using ModulesApp.Models.ServerTasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ModulesApp.Models;

[Table("Module")]
public class DbModule
{
    [Key]
    public long Id { get; set; }
    public string Key { get; set; } = default!;
    public string Name { get; set; } = default!;

    public string? ProgramName { get; set; } = default;
    public string? ProgramVersion { get; set; } = default;

    public string? Chip { get; set; } = default;
    public string? IDFVersion { get; set; } = default;
    public string? FirmwareVersion { get; set; } = default;
    public int? FlashSize { get; set; } = default;
    public int? FreeHeap { get; set; } = default;
    public string? WifiCurrent { get; set; } = default;

    public DateTime? LastResponse { get; set; }

    [JsonExtensionData]
    public Dictionary<string, object> JsonData { get; set; } = default!;
    public ICollection<DbModuleAction> ModuleActions { get; set; } = [];
    public ICollection<DbTask> ServerTasks { get; set; } = [];

    public override string ToString() => Name;
}