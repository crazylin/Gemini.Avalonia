using System.Collections.Concurrent;
using System.Text;
using AuroraUI.Framework.Logging;
using AuroraUI.IO.Net.TCP;
using SCSA.Models;

namespace SCSA.Services;

/// <summary>
/// 设备控制API接口
/// </summary>
public interface IDeviceControlApi : IDisposable
{
    /// <summary>
    /// 读取参数
    /// </summary>
    Task<(bool success, List<Parameter> result)> ReadParameters(List<Parameter> parameters, CancellationToken cancellationToken);

    /// <summary>
    /// 设置参数
    /// </summary>
    Task<bool> SetParameters(List<Parameter> parameters, CancellationToken cancellationToken);

    /// <summary>
    /// 获取参数ID列表
    /// </summary>
    Task<(bool success, List<Parameter> result)> GetParameterIds(CancellationToken cancellationToken);

    /// <summary>
    /// 获取设备状态
    /// </summary>
    Task<(bool success, List<DeviceStatus> result)> GetDeviceStatus(List<DeviceStatusType> statusTypes, CancellationToken cancellationToken);

    /// <summary>
    /// 设置设备控制
    /// </summary>
    Task<bool> SetDeviceControl(Dictionary<string, object> controlData, CancellationToken cancellationToken);

    /// <summary>
    /// 开始数据采集
    /// </summary>
    Task<bool> StartDataCollection(DataChannelType channelType, TriggerType triggerType, double sampleRate, long dataLength, CancellationToken cancellationToken);

    /// <summary>
    /// 停止数据采集
    /// </summary>
    Task<bool> StopDataCollection(CancellationToken cancellationToken);

    /// <summary>
    /// 获取设备信息（Mac地址、SN码、设备类型、IP配置等）
    /// </summary>
    Task<(bool success, DeviceInfoData? result)> GetDeviceInfo(CancellationToken cancellationToken);
}

/// <summary>
/// Pipeline设备控制API实现
/// </summary>
public class PipelineDeviceControlApiAsync : IDeviceControlApi
{
    private static readonly ILogger Logger = LogManager.GetLogger("SCSA.DeviceControlApi");
    private readonly PipelineTcpClient<SCSADataPackage> _tcpClient;
    private readonly ConcurrentDictionary<short, TaskCompletionSource<SCSADataPackage>> _pendingCommands;
    private short _nextCmdId = 1;
    private readonly object _cmdIdLock = new();

    public PipelineDeviceControlApiAsync(PipelineTcpClient<SCSADataPackage> tcpClient)
    {
        _tcpClient = tcpClient;
        _pendingCommands = new ConcurrentDictionary<short, TaskCompletionSource<SCSADataPackage>>();
        
        // 订阅数据接收事件
        _tcpClient.DataReceived += OnDataReceived;
    }

    public PipelineDeviceControlApiAsync(System.Net.IPEndPoint endPoint)
    {
        var tcpClient = new System.Net.Sockets.TcpClient();
        tcpClient.Connect(endPoint);
        _tcpClient = new PipelineTcpClient<SCSADataPackage>(tcpClient);
        _pendingCommands = new ConcurrentDictionary<short, TaskCompletionSource<SCSADataPackage>>();
        
        // 订阅数据接收事件
        _tcpClient.DataReceived += OnDataReceived;
    }

    /// <summary>
    /// 生成下一个命令ID
    /// </summary>
    private short GetNextCmdId()
    {
        lock (_cmdIdLock)
        {
            return _nextCmdId++;
        }
    }

    /// <summary>
    /// 数据接收事件处理
    /// </summary>
    private void OnDataReceived(object? sender, SCSADataPackage package)
    {
        if (_pendingCommands.TryRemove(package.CmdId, out var tcs))
        {
            tcs.SetResult(package);
        }
        else
        {
            Logger.Debug($"收到未匹配的数据包: {package}");
        }
    }

    /// <summary>
    /// 发送命令并等待响应
    /// </summary>
    private async Task<SCSADataPackage?> SendCommandAsync(DeviceCommand command, byte[] data, CancellationToken cancellationToken)
    {
        var cmdId = GetNextCmdId();
        var package = new SCSADataPackage
        {
            Command = command,
            CmdId = cmdId,
            Data = data,
            DataLen = data.Length
        };

        var tcs = new TaskCompletionSource<SCSADataPackage>();
        _pendingCommands[cmdId] = tcs;

        try
        {
            if (!await _tcpClient.SendAsync(package))
            {
                _pendingCommands.TryRemove(cmdId, out _);
                Logger.Error($"发送命令失败: {command} (CmdId: {cmdId})");
                return null;
            }

            Logger.Debug($"发送命令: {command} (CmdId: {cmdId})");

            // 等待响应，默认超时5秒
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(5));

            try
            {
                return await tcs.Task.WaitAsync(timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                _pendingCommands.TryRemove(cmdId, out _);
                Logger.Warning($"命令超时: {command} (CmdId: {cmdId})");
                return null;
            }
        }
        catch (Exception ex)
        {
            _pendingCommands.TryRemove(cmdId, out _);
            Logger.Error($"发送命令异常: {command} (CmdId: {cmdId})", ex);
            return null;
        }
    }

    public async Task<(bool success, List<Parameter> result)> ReadParameters(List<Parameter> parameters, CancellationToken cancellationToken)
    {
        try
        {
            // 序列化参数列表
            var data = SerializeParameters(parameters);
            var response = await SendCommandAsync(DeviceCommand.RequestReadParameters, data, cancellationToken);
            
            if (response != null && response.Command == DeviceCommand.ReplyReadParameters)
            {
                var result = DeserializeParameters(response.Data);
                return (true, result);
            }
            
            return (false, new List<Parameter>());
        }
        catch (Exception ex)
        {
            Logger.Error("读取参数失败", ex);
            return (false, new List<Parameter>());
        }
    }

    public async Task<bool> SetParameters(List<Parameter> parameters, CancellationToken cancellationToken)
    {
        try
        {
            var data = SerializeParameters(parameters);
            var response = await SendCommandAsync(DeviceCommand.RequestSetParameters, data, cancellationToken);
            
            return response != null && response.Command == DeviceCommand.ReplySetParameters;
        }
        catch (Exception ex)
        {
            Logger.Error("设置参数失败", ex);
            return false;
        }
    }

    public async Task<(bool success, List<Parameter> result)> GetParameterIds(CancellationToken cancellationToken)
    {
        try
        {
            var response = await SendCommandAsync(DeviceCommand.RequestGetParameterIds, Array.Empty<byte>(), cancellationToken);
            
            if (response != null && response.Command == DeviceCommand.ReplyGetParameterIds)
            {
                var result = DeserializeParameterIds(response.Data);
                return (true, result);
            }
            
            return (false, new List<Parameter>());
        }
        catch (Exception ex)
        {
            Logger.Error("获取参数ID列表失败", ex);
            return (false, new List<Parameter>());
        }
    }

    public async Task<(bool success, List<DeviceStatus> result)> GetDeviceStatus(List<DeviceStatusType> statusTypes, CancellationToken cancellationToken)
    {
        try
        {
            var data = SerializeStatusTypes(statusTypes);
            var response = await SendCommandAsync(DeviceCommand.RequestGetDeviceStatus, data, cancellationToken);
            
            if (response != null && response.Command == DeviceCommand.ReplyGetDeviceStatus)
            {
                var result = DeserializeDeviceStatuses(response.Data);
                return (true, result);
            }
            
            return (false, new List<DeviceStatus>());
        }
        catch (Exception ex)
        {
            Logger.Error("获取设备状态失败", ex);
            return (false, new List<DeviceStatus>());
        }
    }

    public async Task<bool> SetDeviceControl(Dictionary<string, object> controlData, CancellationToken cancellationToken)
    {
        try
        {
            var data = SerializeControlData(controlData);
            var response = await SendCommandAsync(DeviceCommand.RequestSetDeviceControl, data, cancellationToken);
            
            return response != null && response.Command == DeviceCommand.ReplySetDeviceControl;
        }
        catch (Exception ex)
        {
            Logger.Error("设置设备控制失败", ex);
            return false;
        }
    }

    public async Task<bool> StartDataCollection(DataChannelType channelType, TriggerType triggerType, double sampleRate, long dataLength, CancellationToken cancellationToken)
    {
        try
        {
            var data = SerializeCollectionConfig(channelType, triggerType, sampleRate, dataLength);
            var response = await SendCommandAsync(DeviceCommand.RequestStartCollection, data, cancellationToken);
            
            return response != null;
        }
        catch (Exception ex)
        {
            Logger.Error("开始数据采集失败", ex);
            return false;
        }
    }

    public async Task<bool> StopDataCollection(CancellationToken cancellationToken)
    {
        try
        {
            var response = await SendCommandAsync(DeviceCommand.RequestStopCollection, Array.Empty<byte>(), cancellationToken);
            return response != null;
        }
        catch (Exception ex)
        {
            Logger.Error("停止数据采集失败", ex);
            return false;
        }
    }

    public async Task<(bool success, DeviceInfoData? result)> GetDeviceInfo(CancellationToken cancellationToken)
    {
        try
        {
            // 构造读取参数请求，参数ID为0x0004（设备信息）
            var parameter = new Parameter
            {
                Address = ParameterType.DeviceInfo,
                Value = 0,
                Length = 0
            };

            var parameters = new List<Parameter> { parameter };
            var data = SerializeParameters(parameters);
            var response = await SendCommandAsync(DeviceCommand.RequestReadParameters, data, cancellationToken);
            
            if (response != null && response.Command == DeviceCommand.ReplyReadParameters)
            {
                // 解析返回的设备信息数据
                if (response.Data != null && response.Data.Length >= 37) // 至少37字节
                {
                    var deviceInfo = DeviceInfoData.ParseFromBytes(response.Data);
                    return (true, deviceInfo);
                }
            }
            
            return (false, null);
        }
        catch (Exception ex)
        {
            Logger.Error("获取设备信息失败", ex);
            return (false, null);
        }
    }

    #region 序列化/反序列化方法

    private static byte[] SerializeParameters(List<Parameter> parameters)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        
        writer.Write(parameters.Count);
        foreach (var param in parameters)
        {
            writer.Write((int)param.Address);
            writer.Write(param.Length);
            
            // 根据参数类型使用正确的序列化方式
            var parameterData = GetParameterData(param);
            writer.Write(parameterData);
        }
        
        return ms.ToArray();
    }
    
    /// <summary>
    /// 获取参数数据（按照原项目的GetParameterData实现）
    /// </summary>
    private static byte[] GetParameterData(Parameter parameter)
    {
        switch (parameter.Address)
        {
            case ParameterType.SamplingRate:
            case ParameterType.UploadDataType:
                return [(byte)parameter.Value];
            case ParameterType.LaserPowerIndicatorLevel:
                return [Convert.ToByte(parameter.Value)];
            case ParameterType.SignalStrength:
            case ParameterType.LowPassFilter:
            case ParameterType.HighPassFilter:
            case ParameterType.VelocityRange:
            case ParameterType.DisplacementRange:
            case ParameterType.AccelerationRange:
            case ParameterType.DigitalRange:
            case ParameterType.AnalogOutputType1:
            case ParameterType.AnalogOutputSwitch1:
            case ParameterType.AnalogOutputType2:
            case ParameterType.AnalogOutputSwitch2:
            case ParameterType.FrontendFilter:
            case ParameterType.FrontendFilterType:
            case ParameterType.FrontendFilterSwitch:
            case ParameterType.FrontendDcRemovalSwitch:
            case ParameterType.OrthogonalityCorrectionSwitch:
            case ParameterType.OrthogonalityCorrectionMode:
                return [(byte)parameter.Value];
            case ParameterType.DataSegmentLength:
                return BitConverter.GetBytes(Convert.ToInt32(parameter.Value));
            case ParameterType.VelocityLowPassFilterSwitch:
            case ParameterType.DisplacementLowPassFilterSwitch:
            case ParameterType.AccelerationLowPassFilterSwitch:
                return [(byte)parameter.Value];
            case ParameterType.VelocityAmpCorrection:
            case ParameterType.DisplacementAmpCorrection:
            case ParameterType.AccelerationAmpCorrection:
                // 注意：parameter.Value可能是放大1000倍的整数，需要转换回float
                return BitConverter.GetBytes(Convert.ToSingle(parameter.Value / 1000.0));
            case ParameterType.TriggerSampleType:
            case ParameterType.TriggerSampleMode:
                return [(byte)parameter.Value];
            case ParameterType.TriggerSampleLevel:
                // 触发电平也是float类型
                return BitConverter.GetBytes(Convert.ToSingle(parameter.Value / 1000.0));
            case ParameterType.TriggerSampleChannel:
                return [(byte)parameter.Value];
            case ParameterType.TriggerSampleLength:
            case ParameterType.TriggerSampleDelay:
                return BitConverter.GetBytes(Convert.ToInt32(parameter.Value));
            case ParameterType.LaserDriveCurrent:
            case ParameterType.TECTargetTemperature:
            case ParameterType.OrthogonalityCorrectionValue:
                // 这些也是float类型
                return BitConverter.GetBytes(Convert.ToSingle(parameter.Value / 1000.0));
            default:
                return [0];
        }
    }

    private static List<Parameter> DeserializeParameters(byte[] data)
    {
        var parameters = new List<Parameter>();
        using var ms = new MemoryStream(data);
        using var reader = new BinaryReader(ms);
        
        var count = reader.ReadInt16();
        for (int i = 0; i < count; i++)
        {
            parameters.Add(new Parameter
            {
                Address = (ParameterType)reader.ReadUInt16(),
                Value = reader.ReadInt32(),
                Length = reader.ReadByte()
            });
        }
        
        return parameters;
    }

    private static List<Parameter> DeserializeParameterIds(byte[] data)
    {
        var parameters = new List<Parameter>();
        using var ms = new MemoryStream(data);
        using var reader = new BinaryReader(ms);
        
        var count = reader.ReadInt16();
        for (int i = 0; i < count; i++)
        {
            parameters.Add(new Parameter
            {
                Address = (ParameterType)reader.ReadUInt16(),
                Length = reader.ReadByte()
            });
        }
        
        return parameters;
    }

    private static byte[] SerializeStatusTypes(List<DeviceStatusType> statusTypes)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        
        writer.Write((short)statusTypes.Count);
        foreach (var statusType in statusTypes)
        {
            writer.Write((byte)statusType);
        }
        
        return ms.ToArray();
    }

    private static List<DeviceStatus> DeserializeDeviceStatuses(byte[] data)
    {
        var statuses = new List<DeviceStatus>();
        using var ms = new MemoryStream(data);
        using var reader = new BinaryReader(ms);
        
        var count = reader.ReadInt16();
        for (int i = 0; i < count; i++)
        {
            statuses.Add(new DeviceStatus
            {
                StatusType = (DeviceStatusType)reader.ReadByte(),
                Value = reader.ReadDouble(),
                UpdateTime = DateTime.Now
            });
        }
        
        return statuses;
    }

    private static byte[] SerializeControlData(Dictionary<string, object> controlData)
    {
        // 简化实现：将控制数据序列化为JSON字符串
        var json = System.Text.Json.JsonSerializer.Serialize(controlData);
        return Encoding.UTF8.GetBytes(json);
    }

    private static byte[] SerializeCollectionConfig(DataChannelType channelType, TriggerType triggerType, double sampleRate, long dataLength)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        
        writer.Write((byte)channelType);
        writer.Write((byte)triggerType);
        writer.Write(sampleRate);
        writer.Write(dataLength);
        
        return ms.ToArray();
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // 取消订阅事件
            _tcpClient.DataReceived -= OnDataReceived;
            
            // 取消所有待处理的命令
            foreach (var tcs in _pendingCommands.Values)
            {
                tcs.TrySetCanceled();
            }
            _pendingCommands.Clear();
            
            // 释放TCP客户端
            _tcpClient?.Dispose();
        }
    }

    #endregion
}
