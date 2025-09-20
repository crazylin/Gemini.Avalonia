using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SCSA.Models;
using SCSA.Services;

namespace SCSA.Services;

/// <summary>
/// 设备管理器接口 - 统一管理设备连接、配置和参数
/// </summary>
public interface IDeviceManager : INotifyPropertyChanged, IDisposable
{
    #region 设备连接管理

    /// <summary>
    /// 当前连接的设备
    /// </summary>
    EnhancedDeviceConnection? CurrentDevice { get; }

    /// <summary>
    /// 是否有设备连接
    /// </summary>
    bool HasDevice { get; }

    /// <summary>
    /// 设备连接状态
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// 可用设备列表
    /// </summary>
    ObservableCollection<EnhancedDeviceConnection> AvailableDevices { get; }

    /// <summary>
    /// 连接到指定设备
    /// </summary>
    Task<bool> ConnectToDeviceAsync(EnhancedDeviceConnection device);

    /// <summary>
    /// 断开当前设备连接
    /// </summary>
    Task DisconnectAsync();

    /// <summary>
    /// 刷新可用设备列表
    /// </summary>
    Task RefreshDevicesAsync();

    #endregion

    #region 设备信息管理

    /// <summary>
    /// 设备信息
    /// </summary>
    DeviceInfoData? DeviceInfo { get; }

    /// <summary>
    /// 获取设备信息
    /// </summary>
    Task<(bool success, DeviceInfoData? info)> GetDeviceInfoAsync();

    #endregion

    #region 参数管理

    /// <summary>
    /// 设备参数列表
    /// </summary>
    ObservableCollection<DeviceParameter> Parameters { get; }

    /// <summary>
    /// 读取设备参数
    /// </summary>
    Task<bool> ReadParametersAsync();

    /// <summary>
    /// 写入设备参数
    /// </summary>
    Task<bool> WriteParametersAsync();

    /// <summary>
    /// 读取单个参数
    /// </summary>
    Task<(bool success, object? value)> ReadParameterAsync(ParameterType paramType);

    /// <summary>
    /// 写入单个参数
    /// </summary>
    Task<bool> WriteParameterAsync(ParameterType paramType, object value);

    #endregion

    #region 配置管理

    /// <summary>
    /// 保存参数配置到文件
    /// </summary>
    Task<bool> SaveParametersAsync(string? filePath = null);

    /// <summary>
    /// 从文件加载参数配置
    /// </summary>
    Task<bool> LoadParametersAsync(string filePath);

    /// <summary>
    /// 恢复默认参数
    /// </summary>
    void RestoreDefaultParameters();

    #endregion

    #region 事件

    /// <summary>
    /// 设备连接状态变化事件
    /// </summary>
    event EventHandler<DeviceConnectionChangedEventArgs>? DeviceConnectionChanged;

    /// <summary>
    /// 设备信息更新事件
    /// </summary>
    event EventHandler<DeviceInfoData>? DeviceInfoUpdated;

    /// <summary>
    /// 参数更新事件
    /// </summary>
    event EventHandler<ParameterUpdatedEventArgs>? ParameterUpdated;

    /// <summary>
    /// 状态消息事件
    /// </summary>
    event EventHandler<string>? StatusMessageChanged;

    #endregion
}

/// <summary>
/// 设备连接状态变化事件参数
/// </summary>
public class DeviceConnectionChangedEventArgs : EventArgs
{
    public EnhancedDeviceConnection? Device { get; }
    public bool IsConnected { get; }
    public string? Message { get; }

    public DeviceConnectionChangedEventArgs(EnhancedDeviceConnection? device, bool isConnected, string? message = null)
    {
        Device = device;
        IsConnected = isConnected;
        Message = message;
    }
}

/// <summary>
/// 参数更新事件参数
/// </summary>
public class ParameterUpdatedEventArgs : EventArgs
{
    public ParameterType ParameterType { get; }
    public object? OldValue { get; }
    public object? NewValue { get; }

    public ParameterUpdatedEventArgs(ParameterType parameterType, object? oldValue, object? newValue)
    {
        ParameterType = parameterType;
        OldValue = oldValue;
        NewValue = newValue;
    }
}
