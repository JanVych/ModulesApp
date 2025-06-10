using ModulesApp.Services;
using Quartz;

namespace ModulesApp.Models.BackgroundServices.Servicves;

public class TestBackgroundService : BackgroundService
{
    public TestBackgroundService(ContextService contextService) : base(contextService){}

    public override async Task ExecuteAsync(IJobExecutionContext context)
    {
        Console.WriteLine($"Test Service executed at {DateTime.Now}");
        foreach (var action in Actions)
        {
            Console.WriteLine($"key: {action.Key}, value: {action.Value}");
        }

        Data["TestData"] = "TestValue";
        await Task.Delay(6000);
        Data["Test2"] = 123456789;
        Data["Test3"] = true;
        Data["Test4"] = 123.456;
        Data["Test5"] = new List<string> { "Test1", "Test2", "Test3" };
    }
}
