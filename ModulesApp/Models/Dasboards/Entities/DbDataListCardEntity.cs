using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ModulesApp.Models.Dasboards.Entities;

public class DbDataListCardEntity : DbDashboardEntity
{

    [NotMapped]
    public List<(string, string)> TableData { get; set; } = [];

    public override void UpdateData(Dictionary<string, object> data)
    {
        Data = data;
        if (Data.TryGetValue("Titles", out var titles) && Data.TryGetValue("Values", out var values))
        {
            var titlesList = ToStringList(titles);
            var valuesList = ToStringList(values);
            var tableData = titlesList?
            .Select((title, index) => (title, index < valuesList?.Count ? valuesList[index] : string.Empty))
            .ToList();

            if (tableData != null)
            {
                TableData = tableData;
            }
        }
    }

    private static List<string>? ToStringList(object value)
    {
        if (value is List<string> eValue)
        {
            return eValue;
        }
        if (value is JsonElement jValue && jValue.ValueKind == JsonValueKind.Array)
        {
            return jValue
                .EnumerateArray()
                .Select(e => JsonSerializer.Deserialize<string>(e.GetRawText()) ?? string.Empty)
                .ToList();
        }
        return null;
    }
}
