using ModulesApp.Helpers;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ModulesApp.Models.Dasboards.Entities;

public class DbToggleGroupEntity : DbDashboardEntity
{
    public class ToggleItem
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public MudBlazor.Color Color { get; set; } = MudBlazor.Color.Primary;
    }

    [NotMapped]
    public string Value { get; set; } = string.Empty;
    [NotMapped]
    public MudBlazor.Color Color { get; set; } = MudBlazor.Color.Primary;

    [NotMapped]
    public List<ToggleItem> Items { get; set; } = new();



    public override void UpdateState(string key, object? value, bool toDatabse)
    {
        Data[key] = value;
        if(!toDatabse && key == "Value")
        {
            Value = DataConvertor.ToString(value);
            LoadColor();
        }
    }

    public override void LoadState()
    {
        if (Data.TryGetValue("Value", out var t))
        {
            Value = DataConvertor.ToString(t);
        }
        if (Data.TryGetValue("Items", out var i))
        {
            if(i is List<ToggleItem> itemsList)
            {
                Items = itemsList;
                LoadColor();
            }
            else if (i is JsonElement jsonItemsList)
            {
                var list = jsonItemsList.Deserialize<List<ToggleItem>>();
                if (list != null)
                {
                    Items = list;
                    LoadColor();
                }
            }
        }
    }

    public override void SaveToData()
    {
        Data["Value"] = Value;
        Data["Items"] = Items;
    }

    public void AddNewItem()
    {
        Items.Add(new ToggleItem { Label = $"label{Items.Count + 1}", Value = $"value{Items.Count + 1}" });
    }

    public void LoadColor()
    {
        var currentItem = Items.FirstOrDefault(x => x.Value == Value);
        if (currentItem != null)
        {
            Color = currentItem.Color;
        }
    }
}
