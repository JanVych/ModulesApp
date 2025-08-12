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

        //foreach (var a in Actions)
        //{
        //    Console.WriteLine($"Action: {a.Key}, Data: {a.Value}");
        //}

        var url = ConfigurationData["Url"];
        try
        {
            using HttpClient client = new();
            string jsonString = await client.GetStringAsync(url?.ToString());

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            try
            {
                var list = JsonSerializer.Deserialize<List<Dictionary<string, object?>>>(jsonString, options);
                MessageData = list?[0] ?? [];
            }
            catch (JsonException)
            {
                MessageData = JsonSerializer.Deserialize<Dictionary<string, object?>>(jsonString, options) ?? [];
            }
            

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

    }
}
