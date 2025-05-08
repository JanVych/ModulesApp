using ModulesApp.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.Dasboards.Entities
{
    public class DbTemperaturesListEntity : DbDashboardEntity
    {
        public class TableItem
        {
            public string? Name { get; set; } = string.Empty;
            public double CurrentTemp { get; set; }
            public double TargetTemp { get; set; }
        }

        [NotMapped]
        public List<TableItem> TableData = [];

        [NotMapped]
        public List<string?> Headers = [string.Empty, string.Empty, string.Empty];

        public override void SaveData()
        {
            Data["Column1"] = TableData.Select(i => i.Name).ToList();
            Data["Value"] = TableData.Select(i => i.CurrentTemp).ToList();
            Data["Column3"] = TableData.Select(i => i.TargetTemp).ToList();

            Data["Headers"] = Headers;
        }

        public override void UpdateFromData(Dictionary<string, object?> data)
        {
            foreach (var (key, value) in data)
            {
                Data[key] = value;
            }
            if(Data.TryGetValue("Headers", out var headersObject))
            {
                var headers = DataConvertor.ToList<string>(headersObject);
                if (headers != null)
                {
                    Headers = headers;
                }
                else
                {
                    Headers = [string.Empty, string.Empty, string.Empty];
                }
            }
            if (Data.TryGetValue("Column1", out var names))
            {
                Data.TryGetValue("Value", out var currentTemps);
                Data.TryGetValue("Column3", out var targetTemps);

                var namesList = DataConvertor.ToList<string>(names);
                var currentTempsList = DataConvertor.ToList<double>(currentTemps);
                var targetTempsList = DataConvertor.ToList<double>(targetTemps);

                TableData = namesList?
                    .Select((title, index) => new TableItem
                    {
                        Name = title,
                        CurrentTemp = index < currentTempsList?.Count ? currentTempsList[index] : 0,
                        TargetTemp = index < targetTempsList?.Count ? targetTempsList[index] : 0
                    })
                    .ToList() ?? [];
            }
        }
    }
}
