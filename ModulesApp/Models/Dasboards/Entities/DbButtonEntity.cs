namespace ModulesApp.Models.Dasboards.Entities;

public class DbButtonEntity : DbDashboardEntity
{
    public override void UpdateFromData(Dictionary<string, object?> data)
    {
        Data = data;
    }

    public override void SaveData()
    {
    }
}
