using ModulesApp.Helpers;
using ModulesApp.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace ModulesApp.Models.BackgroundServices.Servicves;

public class DbGoodweBackgroundService : DbBackgroundService
{
    public enum BatteryStatus
    {
        NoBattery = 0,
        Standby = 1,
        Discharging = 2,
        Charging = 3,
    }

    [NotMapped]
    private static readonly ModbusRtuUdp _modbusRtuUdp = new(0xF7, 8899, "192.168.0.240");

    public DbGoodweBackgroundService()
    {
        Type = BackgroundServiceType.Goodwe;
    }

    public override async Task ExecuteAsync(IServerContext serverContext)
    {
        await Task.Delay(1000, _cancellationToken.Token);

        ushort address = 0x0000;
        var regValue = _modbusRtuUdp.ReadRegister(address);
        Debug.WriteLine($"Read register {address} : {regValue}");
    }

    private BatteryStatus? GetBatteryStatus()
    {
        return (BatteryStatus) _modbusRtuUdp.ReadRegister(35184);
    }

}
