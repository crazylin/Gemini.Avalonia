using System.Buffers;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Net;
using System.Text;
using AuroraUI.IO.Net.TCP;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SCSA.Models;

/// <summary>
/// 设备命令枚举
/// </summary>
public enum DeviceCommand : byte
{
    // 数据采集命令
    RequestStartCollection = 0x01,
    RequestStopCollection = 0x02,
    ReplyUploadData = 0x05,
    
    // 参数管理命令
    RequestSetParameters = 0x03,
    RequestReadParameters = 0x04,
    RequestGetParameterIds = 0x06,
    ReplySetParameters = 0x13,
    ReplyReadParameters = 0x14,
    ReplyGetParameterIds = 0x16,
    
    // 设备状态命令
    RequestGetDeviceStatus = 0x07,
    ReplyGetDeviceStatus = 0x17,
    
    // 设备控制命令
    RequestSetDeviceControl = 0x08,
    ReplySetDeviceControl = 0x18
}

/// <summary>
/// 数据通道类型
/// </summary>
public enum DataChannelType : byte
{
    Velocity = 0,           // 速度
    Displacement = 1,       // 位移  
    Acceleration = 2,       // 加速度
    ISignalAndQSignal = 3   // I/Q信号
}

/// <summary>
/// 触发类型
/// </summary>
public enum TriggerType : byte
{
    Manual = 0,       // 手动触发
    Auto = 1,         // 自动触发
    External = 2      // 外部触发
}

/// <summary>
/// 设备状态类型
/// </summary>
public enum DeviceStatusType : byte
{
    RunningState = 0,      // 运行状态
    TEC_NTC = 1,          // TEC NTC
    BoardTemperature = 2,  // 板温
    PdCurrent = 3,        // PD电流
    SignalStrengthI = 4,   // I信号强度
    SignalStrengthQ = 5    // Q信号强度
}

/// <summary>
/// 参数类型
/// </summary>
public enum ParameterType : uint
{
    SamplingRate = 0x00000000,                    // 采样率
    UploadDataType = 0x00000001,                  // 上传数据类型
    LaserPowerIndicatorLevel = 0x00000002,        // 激光功率指示器电平
    SignalStrength = 0x00000003,                  // 信号强度
    DeviceInfo = 0x00000004,                      // 设备信息（Mac地址、SN码、设备类型、IP配置等）
    LowPassFilter = 0x00000005,                   // 低通滤波器
    HighPassFilter = 0x00000006,                  // 高通滤波器
    VelocityRange = 0x00000007,                   // 速度量程
    DisplacementRange = 0x00000008,               // 位移量程
    AccelerationRange = 0x00000009,               // 加速度量程
    AnalogOutputType1 = 0x0000000A,               // 模拟输出类型1
    AnalogOutputSwitch1 = 0x0000000B,             // 模拟输出开关1
    AnalogOutputType2 = 0x0000000C,               // 模拟输出类型2
    AnalogOutputSwitch2 = 0x0000000D,             // 模拟输出开关2

    // 触发采样相关
    TriggerSampleType = 0x0000000E,               // 触发采样类型
    TriggerSampleMode = 0x0000000F,               // 触发采样模式-软（上升沿、下降沿）
    TriggerSampleLevel = 0x00000010,              // 触发采样模式-硬（触发电平）
    TriggerSampleChannel = 0x00000011,            // 触发通道
    TriggerSampleLength = 0x00000012,             // 触发采样长度
    TriggerSampleDelay = 0x00000013,              // 触发前置点数

    // 硬件相关参数
    LaserDriveCurrent = 0x00000014,               // 激光器电流 (mA)
    TECTargetTemperature = 0x00000015,            // TEC目标温度 (℃)
    DigitalRange = 0x00000016,                    // 数字口量程

    // 前端算法参数
    FrontendFilter = 0x10000000,                  // 前端滤波器
    FrontendFilterType = 0x10000001,              // 前端滤波器类型
    FrontendFilterSwitch = 0x10000002,            // 前端滤波器开关
    FrontendDcRemovalSwitch = 0x10000003,         // 前端直流去除开关
    OrthogonalityCorrectionSwitch = 0x10000004,   // 正交性校正开关
    DataSegmentLength = 0x10000005,               // 数据段长度
    VelocityLowPassFilterSwitch = 0x10000006,     // 速度低通滤波器开关
    DisplacementLowPassFilterSwitch = 0x10000007, // 位移低通滤波器开关
    AccelerationLowPassFilterSwitch = 0x10000008, // 加速度低通滤波器开关
    VelocityAmpCorrection = 0x10000009,           // 速度幅值校正
    DisplacementAmpCorrection = 0x1000000A,       // 位移幅值校正
    AccelerationAmpCorrection = 0x1000000B,       // 加速度幅值校正
    OrthogonalityCorrectionMode = 0x1000000C,     // 正交性校正模式
    OrthogonalityCorrectionValue = 0x1000000D     // 正交性校正值
}

/// <summary>
/// SCSA网络数据包
/// </summary>
public class SCSADataPackage : IPipelineDataPackage<SCSADataPackage>, IPacketWritable
{
    private const uint EXPECTED_MAGIC = 'S' | ('C' << 8) | ('Z' << 16) | ('N' << 24); // SCZN Magic

    public byte Version { get; set; } = 0x01;
    public DeviceCommand Command { get; set; }
    public short CmdId { get; set; }
    public int DataLen { get; set; }
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public uint Crc { get; set; }

    public byte[] GetBytes()
    {
        var bodyLen = Data?.Length ?? 0;
        var totalLen = 4 // Magic
                       + 1 // Version
                       + 1 // Command
                       + 2 // CmdId
                       + 4 // BodyLength
                       + bodyLen
                       + 4; // CRC32

        var buffer = new byte[totalLen];
        var idx = 0;

        // Magic "SCZN"
        buffer[idx++] = (byte)(EXPECTED_MAGIC & 0xFF);
        buffer[idx++] = (byte)((EXPECTED_MAGIC >> 8) & 0xFF);
        buffer[idx++] = (byte)((EXPECTED_MAGIC >> 16) & 0xFF);
        buffer[idx++] = (byte)((EXPECTED_MAGIC >> 24) & 0xFF);

        // Version
        buffer[idx++] = Version;

        // Command
        buffer[idx++] = (byte)Command;

        // CmdId (Little Endian)
        buffer[idx++] = (byte)(CmdId & 0xFF);
        buffer[idx++] = (byte)((CmdId >> 8) & 0xFF);

        // BodyLength (Little Endian)
        buffer[idx++] = (byte)(bodyLen & 0xFF);
        buffer[idx++] = (byte)((bodyLen >> 8) & 0xFF);
        buffer[idx++] = (byte)((bodyLen >> 16) & 0xFF);
        buffer[idx++] = (byte)((bodyLen >> 24) & 0xFF);

        // Body
        if (bodyLen > 0 && Data != null)
        {
            Array.Copy(Data, 0, buffer, idx, bodyLen);
            idx += bodyLen;
        }

        // CRC32 - 使用简单的校验和替代
        Crc = CalculateCrc32(buffer.Take(idx).ToArray());
        buffer[idx++] = (byte)(Crc & 0xFF);
        buffer[idx++] = (byte)((Crc >> 8) & 0xFF);
        buffer[idx++] = (byte)((Crc >> 16) & 0xFF);
        buffer[idx++] = (byte)((Crc >> 24) & 0xFF);

        return buffer;
    }

    public bool TryParse(ReadOnlySequence<byte> buffer, out SCSADataPackage packet, out SequencePosition frameEnd)
    {
        packet = null!;
        frameEnd = buffer.Start;

        // 最小包长度检查
        if (buffer.Length < 16) // Magic(4) + Ver(1) + Cmd(1) + CmdId(2) + BodyLen(4) + CRC(4) = 16
            return false;

        var reader = new SequenceReader<byte>(buffer);

        // 检查Magic
        if (!reader.TryReadLittleEndian(out int magicInBuf) || magicInBuf != EXPECTED_MAGIC)
        {
            // 尝试找到下一个可能的Magic位置
            var nextPosition = buffer.GetPosition(1, buffer.Start);
            if (nextPosition.GetInteger() < buffer.End.GetInteger())
            {
                return TryParse(buffer.Slice(nextPosition), out packet, out frameEnd);
            }
            return false;
        }

        // 读取协议头
        if (!reader.TryRead(out var version)) return false;
        if (!reader.TryRead(out var command)) return false;
        if (!reader.TryReadLittleEndian(out short cmdId)) return false;
        if (!reader.TryReadLittleEndian(out int bodyLen)) return false;

        // 数据长度合理性检查
        if (bodyLen < 0 || bodyLen > 16 * 1024 * 1024) // 最大16MB
        {
            var nextPosition = buffer.GetPosition(1, buffer.Start);
            if (nextPosition.GetInteger() < buffer.End.GetInteger())
            {
                return TryParse(buffer.Slice(nextPosition), out packet, out frameEnd);
            }
            return false;
        }

        var totalFrameLen = 12L + bodyLen + 4L; // 12 = 头部长度
        if (buffer.Length < totalFrameLen)
            return false;

        // 读取数据部分
        var payloadStart = buffer.GetPosition(12, buffer.Start);
        var payloadSeq = buffer.Slice(payloadStart, bodyLen);

        // 读取CRC
        var crcStart = buffer.GetPosition(12 + bodyLen, buffer.Start);
        Span<byte> crcSpan = stackalloc byte[4];
        buffer.Slice(crcStart, 4).CopyTo(crcSpan);
        var receivedCrc = (uint)(
            crcSpan[0]
            | (crcSpan[1] << 8)
            | (crcSpan[2] << 16)
            | (crcSpan[3] << 24)
        );

        // CRC校验（可选）
        var computedCrc = CalculateCrc32(buffer.Slice(0, 12 + bodyLen).ToArray());
        if (computedCrc != receivedCrc)
        {
            // CRC校验失败，尝试下一个位置
            var nextPosition = buffer.GetPosition(1, buffer.Start);
            if (nextPosition.GetInteger() < buffer.End.GetInteger())
            {
                return TryParse(buffer.Slice(nextPosition), out packet, out frameEnd);
            }
            return false;
        }

        packet = new SCSADataPackage
        {
            Version = version,
            Command = (DeviceCommand)command,
            CmdId = cmdId,
            DataLen = bodyLen,
            Data = payloadSeq.ToArray(),
            Crc = receivedCrc
        };

        frameEnd = buffer.GetPosition(totalFrameLen, buffer.Start);
        return true;
    }

    public override string ToString()
    {
        return $"CMD: {Command}, CmdId: {CmdId}, DataLen: {DataLen}, Crc: {Crc:X8}";
    }

    /// <summary>
    /// 计算CRC32校验和（简化实现）
    /// </summary>
    private static uint CalculateCrc32(byte[] data)
    {
        using var crc = SHA256.Create();
        var hash = crc.ComputeHash(data);
        return BitConverter.ToUInt32(hash, 0);
    }
}

/// <summary>
/// 参数信息
/// </summary>
public class Parameter : ReactiveObject
{
    [Reactive] public ParameterType Address { get; set; }
    [Reactive] public int Value { get; set; }
    [Reactive] public byte Length { get; set; }
}

/// <summary>
/// 设备参数基类
/// </summary>
public abstract class DeviceParameter : ReactiveObject
{
    [Reactive] public int Address { get; set; }
    [Reactive] public string Name { get; set; } = string.Empty;
    [Reactive] public string Category { get; set; } = string.Empty;
    [Reactive] public string Unit { get; set; } = string.Empty;
    [Reactive] public string Desc { get; set; } = string.Empty;
    [Reactive] public byte DataLength { get; set; }
    [Reactive] public int Value { get; set; }
    
    /// <summary>
    /// 获取最小值（虚拟方法，子类可重写）
    /// </summary>
    public virtual object? GetMinValue() => null;
    
    /// <summary>
    /// 获取最大值（虚拟方法，子类可重写）
    /// </summary>
    public virtual object? GetMaxValue() => null;
}

/// <summary>
/// 枚举选项
/// </summary>
public class EnumOption : ReactiveObject
{
    /// <summary>
    /// 枚举值（发送给设备的字节值）
    /// </summary>
    [Reactive] public int EnumValue { get; set; }
    
    /// <summary>
    /// 实际数值（显示给用户的数值）
    /// </summary>
    [Reactive] public double ActualValue { get; set; }
    
    /// <summary>
    /// 显示文本（包含数值和单位）
    /// </summary>
    [Reactive] public string DisplayText { get; set; } = string.Empty;
    
    /// <summary>
    /// 单位
    /// </summary>
    [Reactive] public string Unit { get; set; } = string.Empty;
}

/// <summary>
/// 枚举类型参数（下拉框选择）
/// </summary>
public class EnumParameter : DeviceParameter
{
    /// <summary>
    /// 枚举选项列表
    /// </summary>
    [Reactive] public ObservableCollection<EnumOption> EnumOptions { get; set; } = new();
    
    /// <summary>
    /// 当前选中的枚举选项
    /// </summary>
    [Reactive] public EnumOption? SelectedEnumOption { get; set; }
    
    public EnumParameter()
    {
        // 当选中的枚举选项改变时，自动更新Value
        this.WhenAnyValue(x => x.SelectedEnumOption)
            .WhereNotNull()
            .Subscribe(option => Value = option.EnumValue);
    }
}

/// <summary>
/// 布尔类型参数（开关控件）
/// </summary>
public class BoolParameter : DeviceParameter
{
    /// <summary>
    /// 布尔值
    /// </summary>
    [Reactive] public bool BoolValue { get; set; }
    
    /// <summary>
    /// 开关选项列表
    /// </summary>
    [Reactive] public ObservableCollection<EnumOption> SwitchOptions { get; set; } = new();
    
    /// <summary>
    /// 当前选中的开关选项
    /// </summary>
    [Reactive] public EnumOption? SelectedSwitchOption { get; set; }
    
    public BoolParameter()
    {
        // 初始化开关选项
        SwitchOptions.Add(new EnumOption 
        { 
            EnumValue = 0, 
            ActualValue = 0, 
            DisplayText = "关" 
        });
        SwitchOptions.Add(new EnumOption 
        { 
            EnumValue = 1, 
            ActualValue = 1, 
            DisplayText = "开" 
        });
        
        // 当布尔值改变时，自动更新Value和SelectedSwitchOption
        this.WhenAnyValue(x => x.BoolValue)
            .Subscribe(boolVal => 
            {
                Value = boolVal ? 1 : 0;
                SelectedSwitchOption = SwitchOptions.FirstOrDefault(o => o.EnumValue == (boolVal ? 1 : 0));
            });
            
        // 当选中的开关选项改变时，自动更新BoolValue
        this.WhenAnyValue(x => x.SelectedSwitchOption)
            .Where(option => option != null)
            .Subscribe(option => 
            {
                BoolValue = option!.EnumValue == 1;
            });
    }
}

/// <summary>
/// 整数类型参数（无小数点）
/// </summary>
public class IntegerParameter : DeviceParameter
{
    [Reactive] public int MinValue { get; set; }
    [Reactive] public int MaxValue { get; set; }
    
    public override object? GetMinValue() => MinValue;
    public override object? GetMaxValue() => MaxValue;
}

/// <summary>
/// 浮点数类型参数（支持小数点）
/// </summary>
public class FloatParameter : DeviceParameter
{
    [Reactive] public double MinValue { get; set; }
    [Reactive] public double MaxValue { get; set; }
    [Reactive] public double FloatValue { get; set; }
    
    public override object? GetMinValue() => MinValue;
    public override object? GetMaxValue() => MaxValue;
    
    public FloatParameter()
    {
        // 当浮点值改变时，自动更新Value（转换为整数存储）
        this.WhenAnyValue(x => x.FloatValue)
            .Subscribe(floatVal => Value = (int)(floatVal * 1000)); // 示例：保持3位小数精度
    }
}

/// <summary>
/// 通用数值类型参数
/// </summary>
public class NumberParameter : DeviceParameter
{
    [Reactive] public int MinValue { get; set; }
    [Reactive] public int MaxValue { get; set; }
    
    public override object? GetMinValue() => MinValue;
    public override object? GetMaxValue() => MaxValue;
}

/// <summary>
/// 参数分类Tab页
/// </summary>
public class ParameterCategoryTab : ReactiveObject
{
    /// <summary>
    /// 分类名称
    /// </summary>
    [Reactive] public string CategoryName { get; set; } = string.Empty;
    
    /// <summary>
    /// 该分类下的参数列表
    /// </summary>
    [Reactive] public ObservableCollection<DeviceParameter> Parameters { get; set; } = new();
    
    /// <summary>
    /// 参数数量
    /// </summary>
    public int Count => Parameters.Count;
    
    /// <summary>
    /// Tab标题（包含参数数量）
    /// </summary>
    public string TabHeader => $"{CategoryName} ({Count})";
}

/// <summary>
/// 设备状态
/// </summary>
public class DeviceStatus : ReactiveObject
{
    [Reactive] public DeviceStatusType StatusType { get; set; }
    [Reactive] public double Value { get; set; }
    [Reactive] public string Unit { get; set; } = string.Empty;
    [Reactive] public DateTime UpdateTime { get; set; } = DateTime.Now;
}

/// <summary>
/// 增强的设备连接信息
/// </summary>
public class EnhancedDeviceConnection : DeviceConnection
{
    /// <summary>
    /// TCP客户端
    /// </summary>
    public PipelineTcpClient<SCSADataPackage>? TcpClient { get; set; }

    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName => $"{DeviceName} ({EndPoint})";

    /// <summary>
    /// IP端点（兼容性属性）
    /// </summary>
    public IPEndPoint? IpEndPoint => EndPoint;

    /// <summary>
    /// 设备参数列表
    /// </summary>
    [Reactive] public List<DeviceParameter> DeviceParameters { get; set; } = new();

    /// <summary>
    /// 支持的参数类型
    /// </summary>
    [Reactive] public List<ParameterType> SupportParameterTypes { get; set; } = new();

    /// <summary>
    /// 设备状态列表
    /// </summary>
    [Reactive] public List<DeviceStatus> DeviceStatuses { get; set; } = new();

    /// <summary>
    /// 最后通信时间
    /// </summary>
    [Reactive] public DateTime LastCommunicationTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 通信状态
    /// </summary>
    [Reactive] public string CommunicationStatus { get; set; } = "正常";
}

/// <summary>
/// 设备信息数据结构（用于参数ID 0x0004）
/// </summary>
public class DeviceInfoData : ReactiveObject
{
    /// <summary>
    /// MAC地址 (6字节)
    /// </summary>
    [Reactive] public byte[] MacAddress { get; set; } = new byte[6];

    /// <summary>
    /// 设备SN码 (10字节)
    /// </summary>
    [Reactive] public byte[] SerialNumber { get; set; } = new byte[10];

    /// <summary>
    /// 设备类型 (1字节)
    /// </summary>
    [Reactive] public byte DeviceType { get; set; }

    /// <summary>
    /// IP地址 (4字节)
    /// </summary>
    [Reactive] public byte[] IpAddress { get; set; } = new byte[4];

    /// <summary>
    /// 子网掩码 (4字节)
    /// </summary>
    [Reactive] public byte[] SubnetMask { get; set; } = new byte[4];

    /// <summary>
    /// 网关 (4字节)
    /// </summary>
    [Reactive] public byte[] Gateway { get; set; } = new byte[4];

    /// <summary>
    /// DNS1 (4字节)
    /// </summary>
    [Reactive] public byte[] Dns1 { get; set; } = new byte[4];

    /// <summary>
    /// DNS2 (4字节)
    /// </summary>
    [Reactive] public byte[] Dns2 { get; set; } = new byte[4];

    /// <summary>
    /// MAC地址字符串表示
    /// </summary>
    public string MacAddressString => 
        string.Join(":", MacAddress.Select(b => b.ToString("X2")));

    /// <summary>
    /// SN码字符串表示
    /// </summary>
    public string SerialNumberString => 
        System.Text.Encoding.ASCII.GetString(SerialNumber).TrimEnd('\0');

    /// <summary>
    /// IP地址字符串表示
    /// </summary>
    public string IpAddressString => 
        string.Join(".", IpAddress.Select(b => b.ToString()));

    /// <summary>
    /// 子网掩码字符串表示
    /// </summary>
    public string SubnetMaskString => 
        string.Join(".", SubnetMask.Select(b => b.ToString()));

    /// <summary>
    /// 网关字符串表示
    /// </summary>
    public string GatewayString => 
        string.Join(".", Gateway.Select(b => b.ToString()));

    /// <summary>
    /// DNS1字符串表示
    /// </summary>
    public string Dns1String => 
        string.Join(".", Dns1.Select(b => b.ToString()));

    /// <summary>
    /// DNS2字符串表示
    /// </summary>
    public string Dns2String => 
        string.Join(".", Dns2.Select(b => b.ToString()));

    /// <summary>
    /// 从字节数组解析设备信息
    /// </summary>
    /// <param name="data">字节数组数据</param>
    /// <returns>解析后的设备信息</returns>
    public static DeviceInfoData ParseFromBytes(byte[] data)
    {
        if (data.Length < 37) // 6+10+1+4+4+4+4+4 = 37字节
            throw new ArgumentException("设备信息数据长度不足");

        var deviceInfo = new DeviceInfoData();
        int offset = 0;

        // MAC地址 (6字节)
        Array.Copy(data, offset, deviceInfo.MacAddress, 0, 6);
        offset += 6;

        // SN码 (10字节)
        Array.Copy(data, offset, deviceInfo.SerialNumber, 0, 10);
        offset += 10;

        // 设备类型 (1字节)
        deviceInfo.DeviceType = data[offset];
        offset += 1;

        // IP地址 (4字节)
        Array.Copy(data, offset, deviceInfo.IpAddress, 0, 4);
        offset += 4;

        // 子网掩码 (4字节)
        Array.Copy(data, offset, deviceInfo.SubnetMask, 0, 4);
        offset += 4;

        // 网关 (4字节)
        Array.Copy(data, offset, deviceInfo.Gateway, 0, 4);
        offset += 4;

        // DNS1 (4字节)
        Array.Copy(data, offset, deviceInfo.Dns1, 0, 4);
        offset += 4;

        // DNS2 (4字节)
        if (data.Length >= offset + 4)
        {
            Array.Copy(data, offset, deviceInfo.Dns2, 0, 4);
        }

        return deviceInfo;
    }

    /// <summary>
    /// 转换为字节数组
    /// </summary>
    /// <returns>字节数组</returns>
    public byte[] ToBytes()
    {
        var data = new byte[37]; // 总共37字节
        int offset = 0;

        Array.Copy(MacAddress, 0, data, offset, 6);
        offset += 6;

        Array.Copy(SerialNumber, 0, data, offset, 10);
        offset += 10;

        data[offset] = DeviceType;
        offset += 1;

        Array.Copy(IpAddress, 0, data, offset, 4);
        offset += 4;

        Array.Copy(SubnetMask, 0, data, offset, 4);
        offset += 4;

        Array.Copy(Gateway, 0, data, offset, 4);
        offset += 4;

        Array.Copy(Dns1, 0, data, offset, 4);
        offset += 4;

        Array.Copy(Dns2, 0, data, offset, 4);

        return data;
    }
}
