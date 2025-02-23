using ModulesApp.Interfaces;
using ModulesApp.Models.BackgroundService;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;

namespace ModulesApp.Services.Background;

public class GoodweBackgroundService : DbBackgroundService
{
    //private static int _port = 11000;
    //UdpClient udpListener = new UdpClient(_port);
    //IPEndPoint groupEndPoint = new IPEndPoint(IPAddress.Any, _port);

    public GoodweBackgroundService()
    {
        Type = BackgroundServiceType.Goodwe;
    }

    public override async Task ExecuteAsync(IServerContext serverContext)
    {
        await Task.Delay(1000, _cancellationToken.Token);
        Debug.WriteLine("Goodwe log action");
    }
}
