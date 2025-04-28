using ModulesApp.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.Dasboards.Entities;

public class DbSwitchEntity : DbDashboardEntity
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
        Data = data;
        if( Data.TryGetValue("Value", out var v))
        {
            _value = DataConvertor.ToBool(v);
        }
    }

    public override void SaveData()
    {
    }
}
