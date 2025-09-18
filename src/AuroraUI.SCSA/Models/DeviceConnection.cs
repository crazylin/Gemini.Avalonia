using System.Net;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SCSA.Models;

/// <summary>
/// 设备连接信息
/// </summary>
public class DeviceConnection : ReactiveObject
{
    /// <summary>
    /// 设备ID
    /// </summary>
    [Reactive]
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>
    /// 设备名称
    /// </summary>
    [Reactive]
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>
    /// IP端点
    /// </summary>
    [Reactive]
    public IPEndPoint? EndPoint { get; set; }

    /// <summary>
    /// 连接时间
    /// </summary>
    [Reactive]
    public DateTime ConnectTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 连接状态
    /// </summary>
    [Reactive]
    public bool IsConnected { get; set; }

    /// <summary>
    /// 固件版本
    /// </summary>
    [Reactive]
    public string FirmwareVersion { get; set; } = string.Empty;

    /// <summary>
    /// 设备类型
    /// </summary>
    [Reactive]
    public string DeviceType { get; set; } = string.Empty;
}

/// <summary>
/// 网络接口信息
/// </summary>
public class NetworkInterfaceInfo : ReactiveObject
{
    /// <summary>
    /// 接口名称
    /// </summary>
    [Reactive]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 接口描述
    /// </summary>
    [Reactive]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// IP地址
    /// </summary>
    [Reactive]
    public string IPAddress { get; set; } = string.Empty;

    /// <summary>
    /// 是否可用
    /// </summary>
    [Reactive]
    public bool IsAvailable { get; set; } = true;

    public override string ToString()
    {
        return $"{Name} ({IPAddress})";
    }
}
