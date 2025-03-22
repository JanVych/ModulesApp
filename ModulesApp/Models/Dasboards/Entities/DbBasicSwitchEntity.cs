using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.Dasboards.Entities;

public class DbBasicSwitchEntity : DbDashboardEntity
{
    private bool _value = false;
    [NotMapped]
    public bool Value
    {
        get => _value;
        set
        {
            _value = value;
            Data["Value"] = value;
        }
    }

    public override void UpdateData(Dictionary<string, object> data)
    {
        var result = Data.TryGetValue("Value", out var v);
        if (result && v is bool bv)
        {
            Value = bv;
        }
    }

    public override void SaveData()
    {
    }
}
