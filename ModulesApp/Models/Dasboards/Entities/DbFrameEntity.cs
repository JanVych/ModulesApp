using ModulesApp.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.Dasboards.Entities;

public class DbFrameEntity: DbDashboardEntity  
{
    [NotMapped]
    public bool IsCustomWidth { get; set; } = false;

    [NotMapped]
    public int MaxWidth { get; set; } = 800;

    public override void LoadState()
    {
        if (Data.TryGetValue("IsCustomWidth", out var obj))
        {
            IsCustomWidth = DataConvertor.ToBool(obj);
        }
        if (Data.TryGetValue("MaxWidth", out var maxWidthObj))
        {
            MaxWidth = DataConvertor.ToInt32(maxWidthObj);
        }
    }

    public override void SaveToData()
    {
        Data["IsCustomWidth"] = IsCustomWidth;
        Data["MaxWidth"] = MaxWidth;
    }
}
