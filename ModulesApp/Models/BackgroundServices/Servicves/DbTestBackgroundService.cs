using ModulesApp.Interfaces;

namespace ModulesApp.Models.BackgroundServices.Servicves;

public class DbTestBackgroundService: DbBackgroundService
{
    public DbTestBackgroundService() { }
    public override async Task ExecuteAsync()
    {
        Console.WriteLine("Test Service Test, Actions:");
        foreach (var action in Actions)
        {
            Console.WriteLine($"key: {action.Key}, value: {action.Value}");
        }

        Console.WriteLine("Test Service Test waiting 4s");
        await Task.Delay(4000, CancellationToken.Token);
        AddMessage("Test1", "Test");
        Console.WriteLine("Test Service Test waiting 6s, without cancellation token");
        await Task.Delay(6000);
        AddMessage("Test2", 123456789);
        AddMessage("Test3", true);
        AddMessage("Test4", 123.456);
        AddMessage("Test5", new List<string> { "Test1", "Test2", "Test3" });
    }
}
