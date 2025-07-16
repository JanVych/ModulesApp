using ModulesApp.Helpers;
using ModulesApp.Services;
using Quartz;

namespace ModulesApp.Models.BackgroundServices.Servicves;

public class GoodweBackgroundService : BackgroundService
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
        All = 0x7F,
        None = 0x00,
    }

    //private readonly ModbusRtuUdp _modbusRtuUdp = new(0xF7, 8899, "192.168.0.240", 2);
    private ModbusRtuUdp? _modbusRtuUdp;

    public GoodweBackgroundService(ContextService contextService) : base(contextService) 
    {
    }

    public override async Task ExecuteAsync(IJobExecutionContext context)
    {
        if (!ConfigurationData.ContainsKey("Port") || !ConfigurationData.ContainsKey("Ip"))
        {
            throw new ArgumentException("Configuration data must contain 'Port' and 'Ip'");
        }

        var ip = DataConvertor.ToString(ConfigurationData["Ip"]);
        var port = DataConvertor.ToInt32(ConfigurationData["Port"]);
        _modbusRtuUdp = new ModbusRtuUdp(0xF7, port, ip, 2);

        if (_modbusRtuUdp.Open())
        {
            foreach (var action in Actions)
            {
                if (action.Key == "SetBatteryPower")
                {
                    var value = DataConvertor.ToDouble(action.Value);
                    SetBatteryPower((short)value);
                }
                else if (action.Key == "SetBatteryCharge")
                {
                    var value = DataConvertor.ToDouble(action.Value);
                    SetBatteryCharge((short)value);
                }
                else if (action.Key == "SetBatteryDischarge")
                {
                    var value = DataConvertor.ToDouble(action.Value);
                    SetBatteryDischarge((short)value);
                }
            }
            MessageData["PV total Power [W]"] = GetPvTotalPower();
            MessageData["Grid Power [W]"] = GetGridPower();
            MessageData["Backup Power [W]"] = GetBackupPower();
            MessageData["Load Power [W]"] = GetLoadPower();
            MessageData["Battery Power [W]"] = GetBatteryPower();
            MessageData["Inverter Temperature [℃]"] = GetInverterTemperature();
            MessageData["Battery Temperature [℃]"] = GetBatteryTemperature();
            MessageData["Battery SOC [%]"] = GetBatterySOC();
            MessageData["Battery Status"] = GetBatteryStatus()?.ToString() ?? "Unknown";

        }
        _modbusRtuUdp.Close();
    }

    public uint? GetPvTotalPower() => _modbusRtuUdp?.ReadU32Register(35301);

    /// <summary>
    ///  Get Grid Power in wats
    /// </summary>
    /// <returns>negative value = consuming, positive value = suplying</returns>
    public int? GetGridPower() => (int?)_modbusRtuUdp?.ReadU32Register(35139);
    public uint? GetBackupPower() => _modbusRtuUdp?.ReadU32Register(35169);
    public uint? GetLoadPower() => _modbusRtuUdp?.ReadU32Register(35171);
    public uint? GetBatteryPower() => _modbusRtuUdp?.ReadU32Register(35182);
    public float? GetInverterTemperature() => _modbusRtuUdp?.ReadFLoatFromS16Register(35174) / 10;
    public float? GetBatteryTemperature() => _modbusRtuUdp?.ReadFLoatFromS16Register(37003) / 10;
    public ushort? GetBatterySOC() => _modbusRtuUdp?.ReadU16Register(37007);
    public BatteryStatus? GetBatteryStatus()
    {
        var value = _modbusRtuUdp?.ReadU16Register(35184);
        return value == null ? null : (BatteryStatus)value;
    }

    public void SetBatteryDays(byte days)
    {
        ushort value = (ushort)(0xFF00 + days);
        _modbusRtuUdp?.WriteU16Register(37001, value);
    }

    public void SetBatteryStartTime(byte hour, byte minute)
    {
        ushort value = (ushort)((hour << 8) + minute);
        _modbusRtuUdp?.WriteU16Register(47515, value);
    }

    public void SetBatteryStopTime(byte hour, byte minute)
    {
        ushort value = (ushort)((hour << 8) + minute);
        _modbusRtuUdp?.WriteU16Register(47516, value);
    }

    public void SetBatteryCharge(short power) => SetBatteryPower((short) -power);
    public void SetBatteryDischarge(short power) => SetBatteryPower(power);

    // 1 percent == 100W
    // 10 kW Max Battery Power

    /// <summary>
    /// Set battery power, in Watts
    /// </summary>
    /// <param name="power"></param>
    private void SetBatteryPower(short power)
    {
        var percentPower = (short)(power / 100);
        if (percentPower > 100 || percentPower < -100)
        {
            return;
        }
        _modbusRtuUdp?.WriteU16Register(47515, 0x0000);
        _modbusRtuUdp?.WriteU16Register(47516, 0x173B);
        _modbusRtuUdp?.WriteS16Register(47517, percentPower);

        if (power == 0)
        {
            _modbusRtuUdp?.WriteU16Register(47518, 0x007F);
        }
        else
        {
            _modbusRtuUdp?.WriteU16Register(47518, 0xFF7F);
        }
       
    }

    // set interval (interval_number, start_time(hodina+minuta), end_time(...), power(wats), day_of_week)

    //interval_numbe - offset for thsi 4 reg. 47515  47516 47517 47518, * 4 * interval_number
    //(hodina+minuta) hour = 1 higher byte,minute = 1 lower byte
    // day_of_week 0x00 -higher deactivated, 0xFF higher- activated, lower = day of week
}
