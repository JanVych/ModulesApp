using ModulesApp.Interfaces;
using System.Diagnostics;

namespace ModulesApp.Models.BackgroundServices.Servicves;

public class DbGoodweBackgroundService : DbBackgroundService
{
    //private static int _port = 11000;
    //UdpClient udpListener = new UdpClient(_port);
    //IPEndPoint groupEndPoint = new IPEndPoint(IPAddress.Any, _port);

    public DbGoodweBackgroundService()
    {
        Type = BackgroundServiceType.Goodwe;
    }

    public override async Task ExecuteAsync(IServerContext serverContext)
    {
        await Task.Delay(1000, _cancellationToken.Token);
        Debug.WriteLine("Goodwe log action");
    }
}
