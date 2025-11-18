using ModulesApp.Helpers;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ModulesApp.Models.Dasboards.Entities;

public class DbToggleGroupEntity : DbDashboardEntity
{
    public class ToggleItem
    {
        public string Label { get; set; } = string.Empty;
        public int Value { get; set; }
        public MudBlazor.Color Color { get; set; } = MudBlazor.Color.Primary;
    }

    [NotMapped]
    public int Value { get; set; }

    [NotMapped]
    public MudBlazor.Color Color { get; set; } = MudBlazor.Color.Primary;

    [NotMapped]
    public List<ToggleItem> Items { get; set; } = new();

    public override void UpdateState(string key, object? value, bool toDatabse)
    {
        if(key == "Value")
        {
            ChangeValue(DataConvertor.ToInt32(value));
        }
        else
        {
            Data[key] = value;
        }
        
    }

    public override void LoadState()
    {
        if (Data.TryGetValue("Value", out var t))
        {
            Value = DataConvertor.ToInt32(t);
            LoadColor();
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
        Items.Add(new ToggleItem { Label = $"label{Items.Count + 1}", Value = Items.Count + 1 });
    }

    public void ChangeValue(int newValue)
    {
        if(Value != newValue)
        {
            Data["PrewValue"] = Value;
            Data["Value"] = newValue;
            Value = newValue;
            LoadColor();
        }
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
