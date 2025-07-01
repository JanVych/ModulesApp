using ModulesApp.Models.ServerTasks;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ModulesApp.Models;

public enum ProgramStatusType
{
    unknown,
    noProgram,
    running,
    stopped,
    error
}

[Table("Module")]
public class DbModule
{
    [Key]
    public long Id { get; set; }
    public string Key { get; set; } = default!;
    public string Name { get; set; } = default!;

    public string? ProgramName { get; set; } = default;
    public ProgramStatusType ProgramStatus { get; set; } = ProgramStatusType.unknown;
    public string? ProgramVersion { get; set; } = default;

    public string? Chip { get; set; } = default;
    public string? IDFVersion { get; set; } = default;
    public string? FirmwareVersion { get; set; } = default;
    public int? FlashSize { get; set; } = default;
    public int? FreeHeap { get; set; } = default;
    public string? WifiCurrent { get; set; } = default;

    public DateTime? LastResponse { get; set; }

    [JsonExtensionData]
    public Dictionary<string, object> Data { get; set; } = [];
    public ICollection<DbAction> Actions { get; set; } = [];
    public ICollection<DbTask> ServerTasks { get; set; } = [];

    public override string ToString() => Name;

    public string LastResponseText()
    {
        var now = DateTime.Now;
        var diff = now - LastResponse;

        return diff?.TotalSeconds switch
        {
            null => "---",
            < 1 => " now ",
            > 3600 => $"{(int)(diff.Value.TotalHours)} h ",
            > 60 => $"{(int)(diff.Value.TotalMinutes)} min ",
            _ => $"{(int)(diff.Value.TotalSeconds)} sec "
        };
    }

    public string FreeHeapText()
    {
        if (FreeHeap is null)
        {
            return "---";
        }
        var freeHeap = Math.Round((double)FreeHeap / 1000, 2);
        return $"{freeHeap} kB ";
    }

    public Color LastResponseColor()
    {
        var now = DateTime.Now;
        var diff = now - LastResponse;
        return diff?.TotalMinutes switch
        {
            > 4 => Color.Error,
            > 2 => Color.Warning,
            _ => Color.Success
        };
    }
}