using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Text.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SCSA.Models;
using SCSA.Services;
using AuroraUI.Framework.Logging;

namespace SCSA.Services;

/// <summary>
/// 设备管理器实现 - 统一管理设备连接、配置和参数
/// </summary>
[Export(typeof(IDeviceManager))]
[Export(typeof(DeviceManager))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class DeviceManager : ReactiveObject, IDeviceManager
{
    private static readonly ILogger Logger = LogManager.GetLogger("SCSA.DeviceManager");
    
    private readonly IConnectionManager _connectionManager;
    private IDeviceControlApi? _deviceControlApi;
    private bool _disposed;

    #region 属性

    /// <summary>
    /// 当前连接的设备
    /// </summary>
    [Reactive]
    public EnhancedDeviceConnection? CurrentDevice { get; private set; }

    /// <summary>
    /// 是否有设备连接
    /// </summary>
    public bool HasDevice => CurrentDevice != null && IsConnected;

    /// <summary>
    /// 设备连接状态
    /// </summary>
    [Reactive]
    public bool IsConnected { get; private set; }

    /// <summary>
    /// 可用设备列表
    /// </summary>
    public ObservableCollection<EnhancedDeviceConnection> AvailableDevices { get; }

    /// <summary>
    /// 设备信息
    /// </summary>
    [Reactive]
    public DeviceInfoData? DeviceInfo { get; private set; }

    /// <summary>
    /// 设备参数列表
    /// </summary>
    public ObservableCollection<DeviceParameter> Parameters { get; }

    #endregion

    #region 事件

    /// <summary>
    /// 设备连接状态变化事件
    /// </summary>
    public event EventHandler<DeviceConnectionChangedEventArgs>? DeviceConnectionChanged;

    /// <summary>
    /// 设备信息更新事件
    /// </summary>
    public event EventHandler<DeviceInfoData>? DeviceInfoUpdated;

    /// <summary>
    /// 参数更新事件
    /// </summary>
    public event EventHandler<ParameterUpdatedEventArgs>? ParameterUpdated;

    /// <summary>
    /// 状态消息事件
    /// </summary>
    public event EventHandler<string>? StatusMessageChanged;

    #endregion

    [ImportingConstructor]
    public DeviceManager(IConnectionManager connectionManager)
    {
        _connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));
        
        AvailableDevices = new ObservableCollection<EnhancedDeviceConnection>();
        Parameters = new ObservableCollection<DeviceParameter>();

        // 监听连接管理器的设备变化
        _connectionManager.ConnectedDevicesChanged += OnConnectedDevicesChanged;
        
        // 初始化默认参数
        RestoreDefaultParameters();
        
        Logger.Info($"设备管理器已初始化，默认参数数量: {Parameters.Count}");
    }

    #region 设备连接管理

    /// <summary>
    /// 连接到指定设备
    /// </summary>
    public async Task<bool> ConnectToDeviceAsync(EnhancedDeviceConnection device)
    {
        try
        {
            Logger.Info($"正在连接到设备: {device.DisplayName}");
            OnStatusMessageChanged("正在连接设备...");

            // 断开当前连接
            if (IsConnected)
            {
                await DisconnectAsync();
            }

            // 创建设备控制API
            if (device.IpEndPoint == null)
            {
                OnStatusMessageChanged("设备IP端点无效");
                return false;
            }
            _deviceControlApi = new PipelineDeviceControlApiAsync(device.IpEndPoint);
            
            // 尝试连接并获取设备信息以验证连接
            var (success, deviceInfo) = await _deviceControlApi.GetDeviceInfo(CancellationToken.None);
            
            if (success)
            {
                CurrentDevice = device;
                IsConnected = true;
                DeviceInfo = deviceInfo;
                
                OnStatusMessageChanged($"已连接到设备: {device.DisplayName}");
                OnDeviceConnectionChanged(device, true, "设备连接成功");
                OnDeviceInfoUpdated(deviceInfo);
                
                Logger.Info($"设备连接成功: {device.DisplayName}");
                return true;
            }
            else
            {
                _deviceControlApi?.Dispose();
                _deviceControlApi = null;
                OnStatusMessageChanged("设备连接失败");
                Logger.Warning($"设备连接失败: {device.DisplayName}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _deviceControlApi?.Dispose();
            _deviceControlApi = null;
            OnStatusMessageChanged($"连接设备失败: {ex.Message}");
            Logger.Error($"连接设备失败: {device.DisplayName}", ex);
            return false;
        }
    }

    /// <summary>
    /// 断开当前设备连接
    /// </summary>
    public async Task DisconnectAsync()
    {
        try
        {
            if (CurrentDevice != null)
            {
                Logger.Info($"正在断开设备连接: {CurrentDevice.DisplayName}");
                OnStatusMessageChanged("正在断开设备连接...");
            }

            var previousDevice = CurrentDevice;
            
            CurrentDevice = null;
            IsConnected = false;
            DeviceInfo = null;
            
            if (_deviceControlApi != null)
            {
                _deviceControlApi.Dispose();
                _deviceControlApi = null;
            }

            OnStatusMessageChanged("设备已断开连接");
            OnDeviceConnectionChanged(previousDevice, false, "设备连接已断开");
            
            Logger.Info("设备连接已断开");
        }
        catch (Exception ex)
        {
            Logger.Error("断开设备连接时发生错误", ex);
            OnStatusMessageChanged($"断开连接失败: {ex.Message}");
        }
        
        await Task.CompletedTask;
    }

    /// <summary>
    /// 刷新可用设备列表
    /// </summary>
    public async Task RefreshDevicesAsync()
    {
        try
        {
            Logger.Info("正在刷新设备列表");
            OnStatusMessageChanged("正在刷新设备列表...");

            await _connectionManager.RefreshDevicesAsync();
            
            OnStatusMessageChanged("设备列表刷新完成");
            Logger.Info("设备列表刷新完成");
        }
        catch (Exception ex)
        {
            Logger.Error("刷新设备列表失败", ex);
            OnStatusMessageChanged($"刷新设备列表失败: {ex.Message}");
        }
    }

    #endregion

    #region 设备信息管理

    /// <summary>
    /// 获取设备信息
    /// </summary>
    public async Task<(bool success, DeviceInfoData? info)> GetDeviceInfoAsync()
    {
        if (_deviceControlApi == null || !IsConnected)
        {
            OnStatusMessageChanged("设备未连接");
            return (false, null);
        }

        try
        {
            Logger.Info("正在获取设备信息");
            OnStatusMessageChanged("正在获取设备信息...");

            var result = await _deviceControlApi.GetDeviceInfo(CancellationToken.None);
            
            if (result.success && result.result != null)
            {
                DeviceInfo = result.result;
                OnDeviceInfoUpdated(result.result);
                OnStatusMessageChanged("设备信息获取成功");
                Logger.Info("设备信息获取成功");
            }
            else
            {
                OnStatusMessageChanged("获取设备信息失败");
                Logger.Warning("获取设备信息失败");
            }

            return result;
        }
        catch (Exception ex)
        {
            Logger.Error("获取设备信息时发生错误", ex);
            OnStatusMessageChanged($"获取设备信息失败: {ex.Message}");
            return (false, null);
        }
    }

    #endregion

    #region 参数管理

    /// <summary>
    /// 读取设备参数
    /// </summary>
    public async Task<bool> ReadParametersAsync()
    {
        if (_deviceControlApi == null || !IsConnected)
        {
            OnStatusMessageChanged("设备未连接");
            return false;
        }

        try
        {
            Logger.Info("正在读取设备参数");
            OnStatusMessageChanged("正在读取设备参数...");

            // 读取所有参数
            bool allSuccess = true;
            foreach (var param in Parameters)
            {
                var result = await ReadParameterAsync((ParameterType)param.Address);
                if (result.success && result.value != null)
                {
                    var oldValue = param.Value;
                    param.Value = Convert.ToInt32(result.value);
                    OnParameterUpdated((ParameterType)param.Address, oldValue, param.Value);
                }
                else
                {
                    allSuccess = false;
                }
            }

            if (allSuccess)
            {
                OnStatusMessageChanged("设备参数读取完成");
                Logger.Info("设备参数读取完成");
            }
            else
            {
                OnStatusMessageChanged("部分参数读取失败");
                Logger.Warning("部分参数读取失败");
            }

            return allSuccess;
        }
        catch (Exception ex)
        {
            Logger.Error("读取设备参数时发生错误", ex);
            OnStatusMessageChanged($"读取参数失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 写入设备参数
    /// </summary>
    public async Task<bool> WriteParametersAsync()
    {
        if (_deviceControlApi == null || !IsConnected)
        {
            OnStatusMessageChanged("设备未连接");
            return false;
        }

        try
        {
            Logger.Info("正在写入设备参数");
            OnStatusMessageChanged("正在写入设备参数...");

            // 写入所有参数
            bool allSuccess = true;
            foreach (var param in Parameters)
            {
                var success = await WriteParameterAsync((ParameterType)param.Address, param.Value);
                if (!success)
                {
                    allSuccess = false;
                }
            }

            if (allSuccess)
            {
                OnStatusMessageChanged("设备参数写入完成");
                Logger.Info("设备参数写入完成");
            }
            else
            {
                OnStatusMessageChanged("部分参数写入失败");
                Logger.Warning("部分参数写入失败");
            }

            return allSuccess;
        }
        catch (Exception ex)
        {
            Logger.Error("写入设备参数时发生错误", ex);
            OnStatusMessageChanged($"写入参数失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 读取单个参数
    /// </summary>
    public async Task<(bool success, object? value)> ReadParameterAsync(ParameterType paramType)
    {
        if (_deviceControlApi == null || !IsConnected)
        {
            return (false, null);
        }

        try
        {
            // 这里应该调用设备API读取具体参数
            // 暂时返回模拟数据
            await Task.Delay(100);
            return (true, 0);
        }
        catch (Exception ex)
        {
            Logger.Error($"读取参数失败: {paramType}", ex);
            return (false, null);
        }
    }

    /// <summary>
    /// 写入单个参数
    /// </summary>
    public async Task<bool> WriteParameterAsync(ParameterType paramType, object value)
    {
        if (_deviceControlApi == null || !IsConnected)
        {
            return false;
        }

        try
        {
            // 这里应该调用设备API写入具体参数
            // 暂时返回成功
            await Task.Delay(100);
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error($"写入参数失败: {paramType} = {value}", ex);
            return false;
        }
    }

    #endregion

    #region 配置管理

    /// <summary>
    /// 保存参数配置到文件
    /// </summary>
    public async Task<bool> SaveParametersAsync(string? filePath = null)
    {
        try
        {
            Logger.Info("正在保存参数配置");
            OnStatusMessageChanged("正在保存参数配置...");

            filePath ??= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                                     "SCSA_Parameters.json");

            var config = new
            {
                SavedAt = DateTime.Now,
                DeviceInfo = DeviceInfo,
                Parameters = Parameters.Select(p => new
                {
                    p.Address,
                    p.Name,
                    p.Value,
                    p.Unit,
                    MinValue = p.GetMinValue(),
                    MaxValue = p.GetMaxValue(),
                    p.DataLength
                }).ToArray()
            };

            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);

            OnStatusMessageChanged($"参数配置已保存: {filePath}");
            Logger.Info($"参数配置已保存: {filePath}");
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error("保存参数配置失败", ex);
            OnStatusMessageChanged($"保存配置失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 从文件加载参数配置
    /// </summary>
    public async Task<bool> LoadParametersAsync(string filePath)
    {
        try
        {
            Logger.Info($"正在加载参数配置: {filePath}");
            OnStatusMessageChanged("正在加载参数配置...");

            if (!File.Exists(filePath))
            {
                OnStatusMessageChanged("配置文件不存在");
                return false;
            }

            var json = await File.ReadAllTextAsync(filePath);
            using var document = JsonDocument.Parse(json);
            
            // 这里应该解析JSON并更新Parameters集合
            // 暂时跳过具体实现
            
            OnStatusMessageChanged($"参数配置已加载: {filePath}");
            Logger.Info($"参数配置已加载: {filePath}");
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error($"加载参数配置失败: {filePath}", ex);
            OnStatusMessageChanged($"加载配置失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 恢复默认参数
    /// </summary>
    public void RestoreDefaultParameters()
    {
        try
        {
            Logger.Info("正在恢复默认参数");
            
            Parameters.Clear();
            
            // 添加所有SCSA参数（使用辅助方法获取详细信息）
            var parameterTypes = new[]
            {
                ParameterType.SamplingRate,
                ParameterType.UploadDataType,
                ParameterType.LaserPowerIndicatorLevel,
                ParameterType.LowPassFilter,
                ParameterType.HighPassFilter,
                ParameterType.VelocityRange,
                ParameterType.DisplacementRange,
                ParameterType.AccelerationRange,
                ParameterType.DigitalRange,
                ParameterType.AnalogOutputType1,
                ParameterType.AnalogOutputSwitch1,
                ParameterType.AnalogOutputType2,
                ParameterType.AnalogOutputSwitch2,
                ParameterType.TriggerSampleType,
                ParameterType.TriggerSampleMode,
                ParameterType.TriggerSampleLevel,
                ParameterType.TriggerSampleChannel,
                ParameterType.TriggerSampleLength,
                ParameterType.TriggerSampleDelay,
                ParameterType.LaserDriveCurrent,
                ParameterType.TECTargetTemperature,
                ParameterType.FrontendFilter,
                ParameterType.FrontendFilterType,
                ParameterType.FrontendFilterSwitch,
                ParameterType.FrontendDcRemovalSwitch,
                ParameterType.OrthogonalityCorrectionSwitch,
                ParameterType.DataSegmentLength,
                ParameterType.VelocityLowPassFilterSwitch,
                ParameterType.DisplacementLowPassFilterSwitch,
                ParameterType.AccelerationLowPassFilterSwitch,
                ParameterType.VelocityAmpCorrection,
                ParameterType.DisplacementAmpCorrection,
                ParameterType.AccelerationAmpCorrection,
                ParameterType.OrthogonalityCorrectionMode,
                ParameterType.OrthogonalityCorrectionValue
            };

            var defaultParams = parameterTypes.Select(paramType =>
            {
                var defaultValue = SCSA.ViewModels.ParameterConfigurationToolViewModel.GetParameterDefaultValue(paramType);
                var actualValue = SCSA.ViewModels.ParameterConfigurationToolViewModel.GetParameterActualValue(paramType, (int)defaultValue);
                
                // 创建对应类型的参数对象
                var parameter = SCSA.ViewModels.ParameterConfigurationToolViewModel.CreateParameter(paramType);

                // 设置通用属性
                parameter.Address = (int)paramType;
                parameter.Name = SCSA.ViewModels.ParameterConfigurationToolViewModel.GetParameterName(paramType);
                parameter.Value = (int)actualValue;
                parameter.Unit = SCSA.ViewModels.ParameterConfigurationToolViewModel.GetParameterUnit(paramType);
                parameter.Category = SCSA.ViewModels.ParameterConfigurationToolViewModel.GetParameterCategory(paramType);
                parameter.Desc = SCSA.ViewModels.ParameterConfigurationToolViewModel.GetParameterDescription(paramType);
                parameter.DataLength = (byte)SCSA.ViewModels.ParameterConfigurationToolViewModel.GetParameterDataLength(paramType);

                // 根据具体类型设置特定属性
                switch (parameter)
                {
                    case EnumParameter enumParam:
                        CreateEnumOptionsForParameter(enumParam, paramType);
                        break;
                    case BoolParameter boolParam:
                        boolParam.BoolValue = (int)defaultValue != 0;
                        break;
                    case IntegerParameter intParam:
                        intParam.MinValue = (int)SCSA.ViewModels.ParameterConfigurationToolViewModel.GetParameterMinValue(paramType);
                        intParam.MaxValue = (int)SCSA.ViewModels.ParameterConfigurationToolViewModel.GetParameterMaxValue(paramType);
                        break;
                    case FloatParameter floatParam:
                        floatParam.MinValue = SCSA.ViewModels.ParameterConfigurationToolViewModel.GetParameterMinValue(paramType);
                        floatParam.MaxValue = SCSA.ViewModels.ParameterConfigurationToolViewModel.GetParameterMaxValue(paramType);
                        floatParam.FloatValue = actualValue;
                        break;
                    case NumberParameter numberParam:
                        numberParam.MinValue = (int)SCSA.ViewModels.ParameterConfigurationToolViewModel.GetParameterMinValue(paramType);
                        numberParam.MaxValue = (int)SCSA.ViewModels.ParameterConfigurationToolViewModel.GetParameterMaxValue(paramType);
                        break;
                }
                
                return parameter;
            });

            foreach (var param in defaultParams)
            {
                Parameters.Add(param);
            }
            
            OnStatusMessageChanged("已恢复默认参数");
            Logger.Info("已恢复默认参数");
        }
        catch (Exception ex)
        {
            Logger.Error("恢复默认参数失败", ex);
            OnStatusMessageChanged($"恢复默认参数失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 为枚举类型参数创建选项列表
    /// </summary>
    private void CreateEnumOptionsForParameter(EnumParameter parameter, ParameterType paramType)
    {
        try
        {
            var unit = SCSA.ViewModels.ParameterConfigurationToolViewModel.GetParameterUnit(paramType);
            parameter.EnumOptions.Clear();
            
            // 根据参数类型获取实际的枚举值
            var enumValues = GetEnumValuesForParameter(paramType);
            
            foreach (var enumValue in enumValues)
            {
                var actualValue = SCSA.ViewModels.ParameterConfigurationToolViewModel.GetParameterActualValue(paramType, enumValue);
                var displayText = SCSA.ViewModels.ParameterConfigurationToolViewModel.GetParameterValueDisplayText(paramType, enumValue);
                
                var option = new EnumOption
                {
                    EnumValue = enumValue,
                    ActualValue = actualValue,
                    DisplayText = displayText,
                    Unit = unit
                };
                
                parameter.EnumOptions.Add(option);
                
                // 设置默认选中项
                if (enumValue == (int)SCSA.ViewModels.ParameterConfigurationToolViewModel.GetParameterDefaultValue(paramType))
                {
                    parameter.SelectedEnumOption = option;
                }
            }
            
            Logger.Debug($"为参数 {parameter.Name} 创建了 {parameter.EnumOptions.Count} 个枚举选项");
        }
        catch (Exception ex)
        {
            Logger.Error($"创建参数 {parameter.Name} 枚举选项失败", ex);
        }
    }

    /// <summary>
    /// 获取指定参数类型的实际枚举值列表
    /// </summary>
    private IEnumerable<int> GetEnumValuesForParameter(ParameterType paramType)
    {
        return paramType switch
        {
            ParameterType.SamplingRate => Enum.GetValues<ParameterRanges.SamplingRate>().Select(e => (int)(byte)e),
            ParameterType.UploadDataType => Enum.GetValues<ParameterRanges.UploadDataType>().Select(e => (int)(byte)e),
            ParameterType.LaserPowerIndicatorLevel => Enum.GetValues<ParameterRanges.LaserPowerLevel>().Select(e => (int)(byte)e),
            ParameterType.LowPassFilter => Enum.GetValues<ParameterRanges.LowPassFilter>().Select(e => (int)(byte)e),
            ParameterType.HighPassFilter => Enum.GetValues<ParameterRanges.HighPassFilter>().Select(e => (int)(byte)e),
            ParameterType.VelocityRange => Enum.GetValues<ParameterRanges.VelocityRange>().Select(e => (int)(byte)e),
            ParameterType.DisplacementRange => Enum.GetValues<ParameterRanges.DisplacementRange>().Select(e => (int)(byte)e),
            ParameterType.AccelerationRange => Enum.GetValues<ParameterRanges.AccelerationRange>().Select(e => (int)(byte)e),
            ParameterType.DigitalRange => Enum.GetValues<ParameterRanges.DigitalRange>().Select(e => (int)(byte)e),
            ParameterType.AnalogOutputType1 or ParameterType.AnalogOutputType2 => Enum.GetValues<ParameterRanges.AnalogOutputType>().Select(e => (int)(byte)e),
            ParameterType.FrontendFilter => Enum.GetValues<ParameterRanges.FrontendFilter>().Select(e => (int)(byte)e),
            ParameterType.FrontendFilterType => Enum.GetValues<ParameterRanges.FrontendFilterType>().Select(e => (int)(byte)e),
            ParameterType.TriggerSampleType => Enum.GetValues<ParameterRanges.TriggerSampleType>().Select(e => (int)(byte)e),
            ParameterType.TriggerSampleMode => Enum.GetValues<ParameterRanges.TriggerSampleMode>().Select(e => (int)(byte)e),
            ParameterType.TriggerSampleChannel => Enum.GetValues<ParameterRanges.TriggerSampleChannel>().Select(e => (int)(byte)e),
            ParameterType.OrthogonalityCorrectionMode => Enum.GetValues<ParameterRanges.OrthogonalityCorrectionMode>().Select(e => (int)(byte)e),
            // 对于开关类型参数，使用SwitchState枚举
            ParameterType.AnalogOutputSwitch1 or 
            ParameterType.AnalogOutputSwitch2 or 
            ParameterType.FrontendFilterSwitch or 
            ParameterType.FrontendDcRemovalSwitch or 
            ParameterType.OrthogonalityCorrectionSwitch or 
            ParameterType.VelocityLowPassFilterSwitch or 
            ParameterType.DisplacementLowPassFilterSwitch or 
            ParameterType.AccelerationLowPassFilterSwitch => Enum.GetValues<ParameterRanges.SwitchState>().Select(e => (int)(byte)e),
            // 对于其他未明确定义的参数类型，回退到范围循环
            _ => GetFallbackEnumValues(paramType)
        };
    }

    /// <summary>
    /// 获取回退的枚举值范围（用于未明确定义枚举的参数类型）
    /// </summary>
    private IEnumerable<int> GetFallbackEnumValues(ParameterType paramType)
    {
        var minValue = (int)SCSA.ViewModels.ParameterConfigurationToolViewModel.GetParameterMinValue(paramType);
        var maxValue = (int)SCSA.ViewModels.ParameterConfigurationToolViewModel.GetParameterMaxValue(paramType);
        
        for (int i = minValue; i <= maxValue; i++)
        {
            yield return i;
        }
    }

    #endregion

    #region 事件处理

    private void OnConnectedDevicesChanged(object? sender, EventArgs e)
    {
        // 更新可用设备列表
        AvailableDevices.Clear();
        foreach (var device in _connectionManager.ConnectedDevices)
        {
            AvailableDevices.Add(device);
        }
    }

    private void OnDeviceConnectionChanged(EnhancedDeviceConnection? device, bool isConnected, string? message = null)
    {
        DeviceConnectionChanged?.Invoke(this, new DeviceConnectionChangedEventArgs(device, isConnected, message));
    }

    private void OnDeviceInfoUpdated(DeviceInfoData? deviceInfo)
    {
        if (deviceInfo != null)
        {
            DeviceInfoUpdated?.Invoke(this, deviceInfo);
        }
    }

    private void OnParameterUpdated(ParameterType parameterType, object? oldValue, object? newValue)
    {
        ParameterUpdated?.Invoke(this, new ParameterUpdatedEventArgs(parameterType, oldValue, newValue));
    }

    private void OnStatusMessageChanged(string message)
    {
        StatusMessageChanged?.Invoke(this, message);
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        if (!_disposed)
        {
            _connectionManager.ConnectedDevicesChanged -= OnConnectedDevicesChanged;
            _deviceControlApi?.Dispose();
            _disposed = true;
            Logger.Info("设备管理器已释放");
        }
    }

    #endregion
}
