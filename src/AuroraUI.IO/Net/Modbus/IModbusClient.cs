namespace AuroraUI.IO.Net.Modbus;

/// <summary>
/// Modbus客户端接口
/// </summary>
public interface IModbusClient
{
    /// <summary>
    /// 串口名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 波特率，默认9600
    /// </summary>
    int BaudRate { get; }

    /// <summary>
    /// 连接状态
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="message">Modbus消息</param>
    void Send(ModbusMessage message);

    /// <summary>
    /// 数据接收事件
    /// </summary>
    event EventHandler<ModbusMessage> OnDataReceived;

    /// <summary>
    /// 端口断开事件
    /// </summary>
    event EventHandler OnSessionClosed;

    /// <summary>
    /// 开启端口
    /// </summary>
    /// <param name="name">端口名称</param>
    /// <param name="baudRate">波特率</param>
    void Start(string name, int baudRate);

    /// <summary>
    /// 停止端口
    /// </summary>
    void Stop();
}

/// <summary>
/// Modbus消息类
/// </summary>
public class ModbusMessage : EventArgs
{
    /// <summary>
    /// 设备地址
    /// </summary>
    public byte Address { set; get; }

    /// <summary>
    /// 功能码
    /// </summary>
    public byte Command { set; get; }

    /// <summary>
    /// 起始地址
    /// </summary>
    public short StartAddress { set; get; }

    /// <summary>
    /// 数量
    /// </summary>
    public short Count { set; get; }

    /// <summary>
    /// 数据
    /// </summary>
    public byte[] Data { set; get; } = Array.Empty<byte>();

    /// <summary>
    /// 错误码
    /// </summary>
    public byte ErrorCode { set; get; }

    /// <summary>
    /// CRC校验
    /// </summary>
    public short Crc { set; get; }

    /// <summary>
    /// 扩展标志
    /// </summary>
    public bool Extend { set; get; } = false;

    /// <summary>
    /// 转换为消息字节数组
    /// </summary>
    /// <returns>字节数组</returns>
    public byte[] ToMessage()
    {
        var data = new List<byte>();
        data.Add(Address);
        data.Add(Command);
        data.AddRange(BitConverter.GetBytes(StartAddress));
        data.AddRange(BitConverter.GetBytes(Count).Reverse());
        //data.AddRange(Data);
        var crc = (short)Crc_Count(data.ToArray());
        data.AddRange(BitConverter.GetBytes(crc));

        return data.ToArray();
    }

    /// <summary>
    /// 转换为字符串表示
    /// </summary>
    /// <returns>十六进制字符串</returns>
    public override string ToString()
    {
        return ToMessage().Select(b => b.ToString("x2")).Aggregate((p, n) => p + " " + n);
    }

    /// <summary>
    /// CRC校验计算
    /// </summary>
    /// <param name="pbuf">待校验的字节数组</param>
    /// <returns>CRC值</returns>
    private int Crc_Count(byte[] pbuf)
    {
        var crcbuf = pbuf;
        //计算并填写CRC校验码
        var crc = 0xffff;
        var len = crcbuf.Length;
        for (var n = 0; n < len; n++)
        {
            byte i;
            crc = crc ^ crcbuf[n];
            for (i = 0; i < 8; i++)
            {
                int TT;
                TT = crc & 1;
                crc = crc >> 1;
                crc = crc & 0x7fff;
                if (TT == 1) crc = crc ^ 0xa001;

                crc = crc & 0xffff;
            }
        }

        return crc;
    }
}
