namespace ModulesApp.Models.Dasboards.Entities;

public class DbButtonEntity : DbDashboardEntity
{
    public override void UpdateData(Dictionary<string, object> data)
    {
        Data = data;
    }

    public override void SaveData()
    {
    }
}
