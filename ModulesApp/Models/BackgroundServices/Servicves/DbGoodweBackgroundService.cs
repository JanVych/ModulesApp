using ModulesApp.Helpers;
using ModulesApp.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

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

    public enum Days
    {
        Sunday = 0x1,
        Monday = 0x2,
        Tuesday = 0x4,
        Wednesday = 0x8,
        Thursday = 0x10,
        Friday = 0x20,
        Saturday = 0x40,
    }

    [NotMapped]
    private readonly ModbusRtuUdp _modbusRtuUdp = new(0xF7, 8899, "192.168.0.240", 2);

    public DbGoodweBackgroundService(){}

    public override async Task ExecuteAsync(IServerContext serverContext)
    {
        if (_modbusRtuUdp.Open())
        {
            //SetBatteryDays(6);
            //SetBatteryDischarge(5);
            AddToMessage("PV1 Power", GetPV1Power());
            AddToMessage("Inverter Power", GetInverterPower());
            AddToMessage("Backup Power", GetBackupPower());
            AddToMessage("Load Power", GetLoadPower());
            AddToMessage("Battery Power", GetBatteryPower());
            AddToMessage("Inverter Temperature", GetInverterTemperature());
            AddToMessage("Battery Temperature", GetBatteryTemperature());
            AddToMessage("Battery SOC", GetBatterySOC());
            AddToMessage("Battery Status", GetBatteryStatus());
        }
        _modbusRtuUdp.Close();
    }

    public uint? GetPV1Power() => _modbusRtuUdp.ReadU32Register(35105);
    public uint? GetInverterPower() => _modbusRtuUdp.ReadU32Register(35137);
    public uint? GetBackupPower() => _modbusRtuUdp.ReadU32Register(35169);
    public uint? GetLoadPower() => _modbusRtuUdp.ReadU32Register(35171);
    public uint? GetBatteryPower() => _modbusRtuUdp.ReadU32Register(35182);
    public float? GetInverterTemperature() => _modbusRtuUdp.ReadFLoatFromS16Register(35174) / 10;
    public float? GetBatteryTemperature() => _modbusRtuUdp.ReadFLoatFromS16Register(37003) / 10;
    public ushort? GetBatterySOC() => _modbusRtuUdp.ReadU16Register(37007);
    public BatteryStatus? GetBatteryStatus()
    {
        var value = _modbusRtuUdp.ReadU16Register(35184);
        return value == null ? null : (BatteryStatus)value;
    }

    public void SetBatteryDays(byte days)
    {
        ushort value = (ushort)(0xFF00 + days);
        _modbusRtuUdp.WriteU16Register(37001, value);
    }

    public void SetBatteryStartTime(byte hour, byte minute)
    {
        ushort value = (ushort)((hour << 8) + minute);
        _modbusRtuUdp.WriteU16Register(47515, value);
    }

    public void SetBatteryStopTime(byte hour, byte minute)
    {
        ushort value = (ushort)((hour << 8) + minute);
        _modbusRtuUdp.WriteU16Register(47516, value);
    }

    public void SetBatteryCharge(short power) => SetBattery(power);
    public void SetBatteryDischarge(short power) => SetBattery((short) -power);
    private void SetBattery(short power)
    {
        _modbusRtuUdp.WriteU16Register(47515, 0x0000);
        _modbusRtuUdp.WriteU16Register(47516, 0x173B);
        _modbusRtuUdp.WriteU16Register(47517, (ushort)power);
        _modbusRtuUdp.WriteU16Register(47518, 0xFFFF);
    }
}
