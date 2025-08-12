using ModulesApp.Helpers;

namespace ModulesApp.Models.Dasboards.Entities;

public class DbDataListEntity : DbDashboardEntity
{
    public class TableItem
    {
        public string? Column1 { get; set; } = string.Empty;
        public string? Column2 { get; set; } = string.Empty;
    }

    public List<TableItem> TableData = [];
    public string Title = string.Empty;
    public string Column2Suffix = string.Empty;

    public override void UpdateState(string key, object? value, bool toDatabse)
    {
        Data[key] = value;
        LoadState();
    }

    public override void LoadState()
    {
        if (Data.TryGetValue("Title", out var ti))
        {
            Title = DataConvertor.ToString(ti);
        }
        if (Data.TryGetValue("Column_2_Suffix", out var suffix))
        {
            Column2Suffix = DataConvertor.ToString(suffix);
        }
        if (Data.TryGetValue("Column_1", out var titles))
        {
            Data.TryGetValue("Column_2", out var values);
            var titlesList = DataConvertor.ToList<string>(titles);
            var valuesList = DataConvertor.ToList<string>(values);

            TableData = titlesList?
                .Select((title, index) => new TableItem
                {
                    Column1 = title,
                    Column2 = index < valuesList?.Count ? valuesList[index] : string.Empty
                })
                .ToList() ?? [];
        }
    }

    public override void SaveToData()
    {
        Data["Column_1"] = TableData.Select(i => i.Column1).ToList();
        Data["Column_2"] = TableData.Select(i => i.Column2).ToList();
        Data["Title"] = Title;
        Data["Column_2_Suffix"] = Column2Suffix;
    }
}
