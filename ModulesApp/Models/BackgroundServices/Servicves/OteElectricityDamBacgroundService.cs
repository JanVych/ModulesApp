using ModulesApp.Services;
using Quartz;
using System.Text.Json;

namespace ModulesApp.Models.BackgroundServices.Servicves;

public class OteElectricityDamBacgroundService : BackgroundService
{
    public OteElectricityDamBacgroundService(ContextService contextService) : base(contextService)
    {
    }

    public override async Task ExecuteAsync(IJobExecutionContext context)
    {
        var name = context.JobDetail.Key.Name;
        Console.WriteLine($"Ote electricity day-ahead market id: {name}, time: {DateTime.Now}");

        var url = "https://www.ote-cr.cz/cs/kratkodobe-trhy/elektrina/denni-trh/@@chart-data";
        var date = $"?date={DateTime.Now:yyyy-MM-dd}";
        try
        {
            using HttpClient client = new();
            string jsonString = await client.GetStringAsync(url + date);

            using JsonDocument doc = JsonDocument.Parse(jsonString);
            var root = doc.RootElement;

            MessageData["Date"] = root.GetProperty("graph").GetProperty("title").GetString();
            var jAmmount = root.GetProperty("data").GetProperty("dataLine")[0].GetProperty("point");
            var jPrices = root.GetProperty("data").GetProperty("dataLine")[1].GetProperty("point");

            List<double> ammounts = [];
            List<double> prices = [];

            for (int i = 0; i < jAmmount.GetArrayLength(); i++)
            {
                ammounts.Add(jAmmount[i].GetProperty("y").GetDouble());
                prices.Add(jPrices[i].GetProperty("y").GetDouble());
            }
            MessageData["TodayAmmounts"] = ammounts;
            MessageData["TodayPrices"] = prices;
            MessageData["CurrentPrice"] = prices[DateTime.Now.Hour - 1];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

    }
}
