using ModulesApp.Services;
using Quartz;
using System.Text.Json;

namespace ModulesApp.Models.BackgroundServices.Servicves;

public class HttpBackgroundService : BackgroundService
{
    public HttpBackgroundService(ContextService contextService) : base(contextService){}

    public override async Task ExecuteAsync(IJobExecutionContext context)
    {
        var name = context.JobDetail.Key.Name;
        Console.WriteLine($"Http id: {name}, time: {DateTime.Now}");

        var url = ConfigurationData["Url"];

        try
        {
            using HttpClient client = new();
            string jsonString = await client.GetStringAsync(url.ToString());

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var list = JsonSerializer.Deserialize<List<Dictionary<string, object?>>>(jsonString, options);
            MessageData = list?[new Random().Next(0, list.Count)] ?? [];

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

    }
}
