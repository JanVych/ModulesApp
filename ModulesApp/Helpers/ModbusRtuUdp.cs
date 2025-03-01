using System.Net.Sockets;
using System.Net;
namespace ModulesApp.Helpers;

public class ModbusRtuUdp
{
    public class CrcException : Exception
    {
        public CrcException(string message) : base(message) { }
        public CrcException(string message, Exception inner) : base(message, inner) { }
    }

    private byte DeviceAddress { get; set; }

    public int ServerPort { get; set; }

    public string ServerIp { get; set; }

    public int TimeoutMs { get; set; } = 5000;

    public ModbusRtuUdp(byte deviceAddress, int serverPort, string serverIp)
    {
        DeviceAddress = deviceAddress;
        ServerPort = serverPort;
        ServerIp = serverIp;
    }

    public async Task<ushort> ReadRegisterAsync(ushort address)
    {
        try
        {
            var frame = BuildFrame(0x03, address, 1);
            byte[] response = await SendAndReceiveAsync(frame);
            return (ushort)(response[3] << 8 | response[4]);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ModbusRtuUdp: {ex.Message}");
            return 0;
        }
    }

    public ushort ReadRegister(ushort address)
    {
        try
        {
            var frame = BuildFrame(0x03, address, 1);
            byte[] response = SendAndReceive(frame);
            return (ushort)(response[3] << 8 | response[4]);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ModbusRtuUdp: {ex.Message}");
            return 0;
        }
    }

    public async Task WriteRegisterAsync(ushort address, ushort value)
    {
        try
        {
            var frame = BuildFrame(0x06, address, value);
            byte[] response = await SendAndReceiveAsync(frame);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ModbusRtuUdp: {ex.Message}");
        }
    }

    public void WriteRegister(ushort address, ushort value)
    {
        try
        {
            var frame = BuildFrame(0x06, address, value);
            byte[] response = SendAndReceive(frame);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ModbusRtuUdp: {ex.Message}");
        }
    }


    private byte[] BuildFrame(byte functionCode, ushort address, ushort amountOrValue)
    {
        var frame = new byte[8];
        frame[0] = DeviceAddress;
        frame[1] = functionCode;
        frame[2] = (byte)(address >> 8);
        frame[3] = (byte)address;
        frame[4] = (byte)(amountOrValue >> 8);
        frame[5] = (byte)amountOrValue;
        ushort crc = CalculateCrc(frame, 6);
        frame[6] = (byte)crc;
        frame[7] = (byte)(crc >> 8);
        return frame;
    }

    private async Task<byte[]> SendAndReceiveAsync(byte[] data)
    {
        using UdpClient client = new();
        client.Client.ReceiveTimeout = TimeoutMs;
        client.Connect(ServerIp, ServerPort);
        await client.SendAsync(data, data.Length);

        UdpReceiveResult result = await client.ReceiveAsync();
        CheckCrc(result.Buffer);
        return result.Buffer;
    }

    private byte[] SendAndReceive(byte[] data)
    {
        using UdpClient client = new();
        client.Client.ReceiveTimeout = TimeoutMs;
        client.Connect(ServerIp, ServerPort);
        client.Send(data, data.Length);

        IPEndPoint remoteEndPoint = new(IPAddress.Any, ServerPort);
        var result = client.Receive(ref remoteEndPoint);
        CheckCrc(result);
        return result;
    }

    private static void CheckCrc(byte[] data)
    {
        ushort crc = CalculateCrc(data, data.Length - 2);
        ushort receivedCrc = (ushort)(data[^1] << 8 | data[^2]);
        if( !(crc == receivedCrc))
        {
            throw new CrcException("CRC check failed");
        }
    }

    public static ushort CalculateCrc(byte[] data, int length)
    {
        ushort crc = 0xFFFF;
        for (int i = 0; i < length; i++)
        {
            crc ^= data[i];
            for (int j = 0; j < 8; j++)
            {
                if ((crc & 1) != 0)
                {
                    crc >>= 1;
                    crc ^= 0xA001;
                }
                else
                {
                    crc >>= 1;
                }
            }
        }
        return crc;
    }
}
