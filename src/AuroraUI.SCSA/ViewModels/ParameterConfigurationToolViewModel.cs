using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Reactive;
using System.Reactive.Linq;
using System.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SCSA.Models;
using SCSA.Services;
using static SCSA.Models.ParameterRanges;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using AuroraUI.Framework.Logging;
using AuroraUI.Framework.Services;
using AuroraUI.Framework;

namespace SCSA.ViewModels;

/// <summary>
/// 参数配置工具ViewModel - 使用统一的设备管理器
/// </summary>
[Export(typeof(ITool))]
[Export(typeof(ParameterConfigurationToolViewModel))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class ParameterConfigurationToolViewModel : AuroraUI.Framework.Tool, IDisposable
{
    private static readonly ILogger Logger = LogManager.GetLogger("SCSA.ParameterConfiguration");
    private readonly IDeviceManager _deviceManager;

    /// <summary>
    /// 选中的参数
    /// </summary>
    [Reactive]
    public DeviceParameter? SelectedParameter { get; set; }

    /// <summary>
    /// 状态消息
    /// </summary>
    [Reactive]
    public string StatusMessage { get; set; } = "就绪";

    /// <summary>
    /// 是否正在操作
    /// </summary>
    [Reactive]
    public bool IsOperating { get; set; }

    /// <summary>
    /// 设备信息文本
    /// </summary>
    [Reactive]
    public string DeviceInfoText { get; set; } = "";

    // 通过设备管理器访问的属性
    public bool HasDevice => _deviceManager.HasDevice;
    public bool IsConnected => _deviceManager.IsConnected;
    public ObservableCollection<DeviceParameter> Parameters => _deviceManager.Parameters;
    public DeviceInfoData? DeviceInfo => _deviceManager.DeviceInfo;
    
    /// <summary>
    /// 可选的参数分类列表（包含"所有参数"选项）
    /// </summary>
    [Reactive] public ObservableCollection<string> AvailableCategories { get; private set; } = new();
    
    /// <summary>
    /// 当前选中的分类
    /// </summary>
    [Reactive] public string? SelectedCategory { get; set; }
    
    /// <summary>
    /// 过滤后显示的参数列表
    /// </summary>
    [Reactive] public ObservableCollection<DeviceParameter> FilteredParameters { get; private set; } = new();
    

    /// <summary>
    /// 读取参数命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> ReadParametersCommand { get; }

    /// <summary>
    /// 写入参数命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> WriteParametersCommand { get; }

    /// <summary>
    /// 保存参数命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> SaveParametersCommand { get; }

    /// <summary>
    /// 另存为参数命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> SaveAsParametersCommand { get; }

    /// <summary>
    /// 加载参数命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> LoadParametersCommand { get; }

    /// <summary>
    /// 刷新参数命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> RefreshParametersCommand { get; }

    /// <summary>
    /// 恢复默认值命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> RestoreDefaultsCommand { get; }

    /// <summary>
    /// 获取设备信息命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> GetDeviceInfoCommand { get; }

    /// <summary>
    /// 首选位置 - 右侧dock
    /// </summary>
    public override PaneLocation PreferredLocation => PaneLocation.Right;
    
    /// <summary>
    /// 首选宽度
    /// </summary>
    public override double PreferredWidth => 450.0;
    
    /// <summary>
    /// 首选高度
    /// </summary>
    public override double PreferredHeight => 600.0;

    [ImportingConstructor]
    public ParameterConfigurationToolViewModel(IDeviceManager deviceManager) : base()
    {
        _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
        
        // 设置工具基本信息
        DisplayName = "参数配置";
        ToolTip = "设备参数配置和管理工具";
        
        // 定义命令执行条件
        var hasDeviceObs = this.WhenAnyValue(x => x.HasDevice);
        var notOperatingObs = this.WhenAnyValue(x => x.IsOperating).Select(x => !x);
        var canOperate = hasDeviceObs.CombineLatest(notOperatingObs, (hasDevice, notOperating) => hasDevice && notOperating);
        var hasParameters = Observable.Return(true); // 暂时移除有问题的绑定

        // 初始化命令
        ReadParametersCommand = ReactiveCommand.CreateFromTask(ReadParametersAsync, canOperate);
        WriteParametersCommand = ReactiveCommand.CreateFromTask(WriteParametersAsync, canOperate);
        SaveParametersCommand = ReactiveCommand.CreateFromTask(SaveParametersAsync, hasParameters);
        SaveAsParametersCommand = ReactiveCommand.CreateFromTask(SaveAsParametersAsync, hasParameters);
        LoadParametersCommand = ReactiveCommand.CreateFromTask(LoadParametersAsync, notOperatingObs);
        RefreshParametersCommand = ReactiveCommand.CreateFromTask(RefreshParametersAsync, canOperate);
        RestoreDefaultsCommand = ReactiveCommand.Create(RestoreDefaults, hasParameters);
        GetDeviceInfoCommand = ReactiveCommand.CreateFromTask(GetDeviceInfoAsync, canOperate);

        // 订阅设备管理器事件
        _deviceManager.StatusMessageChanged += OnStatusMessageChanged;
        _deviceManager.DeviceInfoUpdated += OnDeviceInfoUpdated;
        
        // 初始化分类选择
        InitializeCategorySelection();
        
        Logger.Info($"参数配置工具ViewModel已初始化，首选位置: {PreferredLocation}");
    }

    #region 分类管理
    
    private const string ALL_PARAMETERS = "所有参数";
    
    /// <summary>
    /// 初始化分类选择
    /// </summary>
    private void InitializeCategorySelection()
    {
        // 监听参数集合变化，重新更新分类列表
        Parameters.CollectionChanged += (sender, e) => UpdateAvailableCategories();
        
        // 监听分类选择变化，更新过滤的参数列表
        this.WhenAnyValue(x => x.SelectedCategory)
            .Subscribe(_ => UpdateFilteredParameters());
        
        // 初始更新
        UpdateAvailableCategories();
        
        // 默认选择"所有参数"
        SelectedCategory = ALL_PARAMETERS;
    }
    
    /// <summary>
    /// 更新可用分类列表
    /// </summary>
    private void UpdateAvailableCategories()
    {
        var currentSelection = SelectedCategory;
        
        AvailableCategories.Clear();
        
        // 添加"所有参数"选项
        AvailableCategories.Add(ALL_PARAMETERS);
        
        // 添加实际的分类，按指定顺序排序
        var actualCategories = Parameters
            .Select(p => p.Category)
            .Distinct()
            .ToList();
            
        // 定义分类显示顺序
        var categoryOrder = new List<string>
        {
            "基础配置",      
            "量程配置",
            "模拟口配置",
            "信号处理配置", 
            "触发采样配置",
            "算法参数配置",
            "硬件参数配置"
        };
        
        // 按指定顺序添加分类
        foreach (var category in categoryOrder)
        {
            if (actualCategories.Contains(category))
            {
                AvailableCategories.Add(category);
            }
        }
        
        // 添加任何不在预定义顺序中的分类
        foreach (var category in actualCategories.Where(c => !categoryOrder.Contains(c)).OrderBy(c => c))
        {
            AvailableCategories.Add(category);
        }
        
        // 恢复之前的选择，如果不存在则选择"所有参数"
        if (!string.IsNullOrEmpty(currentSelection) && AvailableCategories.Contains(currentSelection))
        {
            SelectedCategory = currentSelection;
        }
        else
        {
            SelectedCategory = ALL_PARAMETERS;
        }
    }
    
    /// <summary>
    /// 更新过滤后的参数列表
    /// </summary>
    private void UpdateFilteredParameters()
    {
        FilteredParameters.Clear();
        
        if (string.IsNullOrEmpty(SelectedCategory))
            return;
            
        IEnumerable<DeviceParameter> parametersToShow;
        
        if (SelectedCategory == ALL_PARAMETERS)
        {
            // 显示所有参数，按分类和地址排序
            parametersToShow = Parameters.OrderBy(p => p.Category).ThenBy(p => p.Address);
        }
        else
        {
            // 显示选中分类的参数
            parametersToShow = Parameters
                .Where(p => p.Category == SelectedCategory)
                .OrderBy(p => p.Address);
        }
        
        foreach (var param in parametersToShow)
        {
            FilteredParameters.Add(param);
        }
    }
    
    #endregion
    
    #region 命令实现 - 委托给设备管理器

    /// <summary>
    /// 读取参数
    /// </summary>
    private async Task ReadParametersAsync()
    {
        try
        {
            IsOperating = true;
            await _deviceManager.ReadParametersAsync();
        }
        finally
        {
            IsOperating = false;
        }
    }

    /// <summary>
    /// 写入参数
    /// </summary>
    private async Task WriteParametersAsync()
    {
        try
        {
            IsOperating = true;
            await _deviceManager.WriteParametersAsync();
        }
        finally
        {
            IsOperating = false;
        }
    }

    /// <summary>
    /// 保存参数到文件
    /// </summary>
    private async Task SaveParametersAsync()
    {
        await _deviceManager.SaveParametersAsync();
    }

    /// <summary>
    /// 参数另存为
    /// </summary>
    private async Task SaveAsParametersAsync()
    {
        // 这里可以打开文件对话框让用户选择保存路径
        await _deviceManager.SaveParametersAsync();
    }

    /// <summary>
    /// 从文件加载参数
    /// </summary>
    private async Task LoadParametersAsync()
    {
        // 这里可以打开文件对话框让用户选择文件
        // 暂时使用默认路径
        var defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                                      "SCSA_Parameters.json");
        await _deviceManager.LoadParametersAsync(defaultPath);
    }

    /// <summary>
    /// 刷新参数列表
    /// </summary>
    private async Task RefreshParametersAsync()
    {
        try
        {
            IsOperating = true;
            await _deviceManager.ReadParametersAsync();
        }
        finally
        {
            IsOperating = false;
        }
    }

    /// <summary>
    /// 恢复默认参数
    /// </summary>
    private void RestoreDefaults()
    {
        _deviceManager.RestoreDefaultParameters();
    }

    /// <summary>
    /// 获取设备信息
    /// </summary>
    private async Task GetDeviceInfoAsync()
    {
        try
        {
            IsOperating = true;
            await _deviceManager.GetDeviceInfoAsync();
        }
        finally
        {
            IsOperating = false;
        }
    }

    #endregion

    #region 参数辅助方法

    /// <summary>
    /// 获取参数名称
    /// </summary>
    public static string GetParameterName(ParameterType paramType)
    {
        return paramType switch
        {
            ParameterType.DeviceInfo => "设备信息",
            ParameterType.SamplingRate => "采样率",
            ParameterType.UploadDataType => "上传数据类型",
            ParameterType.LaserPowerIndicatorLevel => "激光功率指示器电平",
            ParameterType.LowPassFilter => "低通滤波器",
            ParameterType.HighPassFilter => "高通滤波器",
            ParameterType.VelocityRange => "速度量程",
            ParameterType.DisplacementRange => "位移量程",
            ParameterType.AccelerationRange => "加速度量程",
            ParameterType.DigitalRange => "数字量程",
            ParameterType.AnalogOutputType1 => "模拟输出类型1",
            ParameterType.AnalogOutputSwitch1 => "模拟输出开关1",
            ParameterType.AnalogOutputType2 => "模拟输出类型2",
            ParameterType.AnalogOutputSwitch2 => "模拟输出开关2",
            ParameterType.TriggerSampleType => "触发采样类型",
            ParameterType.TriggerSampleMode => "触发采样模式",
            ParameterType.TriggerSampleLevel => "触发采样电平",
            ParameterType.TriggerSampleChannel => "触发采样通道",
            ParameterType.TriggerSampleLength => "触发采样长度",
            ParameterType.TriggerSampleDelay => "触发采样延迟",
            ParameterType.LaserDriveCurrent => "激光器驱动电流",
            ParameterType.TECTargetTemperature => "TEC目标温度",
            ParameterType.FrontendFilter => "前端滤波器",
            ParameterType.FrontendFilterType => "前端滤波器类型",
            ParameterType.FrontendFilterSwitch => "前端滤波器开关",
            ParameterType.FrontendDcRemovalSwitch => "前端直流去除开关",
            ParameterType.OrthogonalityCorrectionSwitch => "正交性校正开关",
            ParameterType.DataSegmentLength => "数据分段长度",
            ParameterType.VelocityLowPassFilterSwitch => "速度低通滤波器开关",
            ParameterType.DisplacementLowPassFilterSwitch => "位移低通滤波器开关",
            ParameterType.AccelerationLowPassFilterSwitch => "加速度低通滤波器开关",
            ParameterType.VelocityAmpCorrection => "速度幅度校正",
            ParameterType.DisplacementAmpCorrection => "位移幅度校正",
            ParameterType.AccelerationAmpCorrection => "加速度幅度校正",
            ParameterType.OrthogonalityCorrectionMode => "正交性校正模式",
            ParameterType.OrthogonalityCorrectionValue => "正交性校正值",
            _ => paramType.ToString()
        };
    }

    /// <summary>
    /// 获取参数单位
    /// </summary>
    public static string GetParameterUnit(ParameterType paramType)
    {
        return paramType switch
        {
            ParameterType.SamplingRate => "Hz",
            ParameterType.LowPassFilter => "Hz",
            ParameterType.FrontendFilter => "Hz",
            ParameterType.VelocityRange => "μm/s",
            ParameterType.DisplacementRange => "μm",
            ParameterType.LaserDriveCurrent => "mA",
            ParameterType.TECTargetTemperature => "℃",
            ParameterType.DataSegmentLength => "点",
            ParameterType.TriggerSampleLength => "点",
            ParameterType.TriggerSampleDelay => "点",
            ParameterType.TriggerSampleLevel => "V",
            ParameterType.VelocityAmpCorrection => "",
            ParameterType.DisplacementAmpCorrection => "",
            ParameterType.AccelerationAmpCorrection => "",
            _ => ""
        };
    }

    /// <summary>
    /// 获取参数最小值
    /// </summary>
    public static double GetParameterMinValue(ParameterType paramType)
    {
        return paramType switch
        {
            ParameterType.SamplingRate => 0,
            ParameterType.UploadDataType => 0,
            ParameterType.LaserPowerIndicatorLevel => 0,
            ParameterType.LowPassFilter => 0,
            ParameterType.HighPassFilter => 0,
            ParameterType.VelocityRange => 0,
            ParameterType.DisplacementRange => 0,
            ParameterType.AccelerationRange => 0,
            ParameterType.DigitalRange => 0,
            ParameterType.FrontendFilter => 0,
            ParameterType.LaserDriveCurrent => 0,
            ParameterType.TECTargetTemperature => 10,
            ParameterType.DataSegmentLength => 64,
            ParameterType.TriggerSampleLength => 1,
            ParameterType.TriggerSampleDelay => 0,
            ParameterType.TriggerSampleLevel => -10,
            ParameterType.VelocityAmpCorrection => 0,
            ParameterType.DisplacementAmpCorrection => 0,
            ParameterType.AccelerationAmpCorrection => 0,
            _ => 0
        };
    }

    /// <summary>
    /// 获取参数最大值
    /// </summary>
    public static double GetParameterMaxValue(ParameterType paramType)
    {
        return paramType switch
        {
            // 使用枚举定义的参数范围
            ParameterType.SamplingRate => GetMaxValue<SamplingRate>(),
            ParameterType.UploadDataType => GetMaxValue<UploadDataType>(),
            ParameterType.LaserPowerIndicatorLevel => GetMaxValue<LaserPowerLevel>(),
            ParameterType.LowPassFilter => GetMaxValue<LowPassFilter>(),
            ParameterType.HighPassFilter => GetMaxValue<HighPassFilter>(),
            ParameterType.VelocityRange => GetMaxValue<VelocityRange>(),
            ParameterType.DisplacementRange => GetMaxValue<DisplacementRange>(),
            ParameterType.AccelerationRange => GetMaxValue<AccelerationRange>(),
            ParameterType.DigitalRange => GetMaxValue<DigitalRange>(),
            ParameterType.FrontendFilter => GetMaxValue<FrontendFilter>(),
            ParameterType.AnalogOutputType1 => GetMaxValue<AnalogOutputType>(),
            ParameterType.AnalogOutputType2 => GetMaxValue<AnalogOutputType>(),
            ParameterType.FrontendFilterType => GetMaxValue<FrontendFilterType>(),
            ParameterType.TriggerSampleType => GetMaxValue<TriggerSampleType>(),
            ParameterType.TriggerSampleChannel => GetMaxValue<TriggerSampleChannel>(),
            ParameterType.OrthogonalityCorrectionMode => GetMaxValue<OrthogonalityCorrectionMode>(),
            // 开关类型参数
            ParameterType.AnalogOutputSwitch1 => GetMaxValue<SwitchState>(),
            ParameterType.AnalogOutputSwitch2 => GetMaxValue<SwitchState>(),
            ParameterType.FrontendFilterSwitch => GetMaxValue<SwitchState>(),
            ParameterType.FrontendDcRemovalSwitch => GetMaxValue<SwitchState>(),
            ParameterType.OrthogonalityCorrectionSwitch => GetMaxValue<SwitchState>(),
            ParameterType.VelocityLowPassFilterSwitch => GetMaxValue<SwitchState>(),
            ParameterType.DisplacementLowPassFilterSwitch => GetMaxValue<SwitchState>(),
            ParameterType.AccelerationLowPassFilterSwitch => GetMaxValue<SwitchState>(),
            // 数值范围参数保持原有设置
            ParameterType.LaserDriveCurrent => 200,
            ParameterType.TECTargetTemperature => 40,
            ParameterType.DataSegmentLength => 65536,
            ParameterType.TriggerSampleLength => 65536,
            ParameterType.TriggerSampleDelay => 65536,
            ParameterType.TriggerSampleLevel => 10,
            ParameterType.VelocityAmpCorrection => 10,
            ParameterType.DisplacementAmpCorrection => 10,
            ParameterType.AccelerationAmpCorrection => 10,
            // 其他参数保持默认值
            ParameterType.TriggerSampleMode => 100,
            _ => 100
        };
    }

    /// <summary>
    /// 获取参数默认值
    /// </summary>
    public static double GetParameterDefaultValue(ParameterType paramType)
    {
        return paramType switch
        {
            ParameterType.SamplingRate => 0x0C, // 5MHz
            ParameterType.UploadDataType => 0,
            ParameterType.LaserPowerIndicatorLevel => 10,
            ParameterType.LowPassFilter => 0x00, // 100Hz
            ParameterType.HighPassFilter => 0,
            ParameterType.VelocityRange => 0x00, // 24.5 μm/s
            ParameterType.DisplacementRange => 0x00, // 2.45 μm
            ParameterType.AccelerationRange => 0,
            ParameterType.DigitalRange => 0,
            ParameterType.FrontendFilter => 0x00, // 100kHz
            ParameterType.LaserDriveCurrent => 45,
            ParameterType.TECTargetTemperature => 25,
            ParameterType.DataSegmentLength => 1024,
            ParameterType.TriggerSampleLength => 1024,
            ParameterType.TriggerSampleDelay => 0,
            ParameterType.TriggerSampleLevel => 0,
            ParameterType.VelocityAmpCorrection => 1,
            ParameterType.DisplacementAmpCorrection => 1,
            ParameterType.AccelerationAmpCorrection => 1,
            _ => 0
        };
    }

    /// <summary>
    /// 获取参数数据长度
    /// </summary>
    public static int GetParameterDataLength(ParameterType paramType)
    {
        return paramType switch
        {
            ParameterType.DeviceInfo => 41, // 设备信息特殊长度
            ParameterType.DataSegmentLength => 4,
            ParameterType.TriggerSampleLength => 4,
            ParameterType.TriggerSampleDelay => 4,
            ParameterType.LaserDriveCurrent => 4,
            ParameterType.TECTargetTemperature => 4,
            ParameterType.TriggerSampleLevel => 4,
            ParameterType.VelocityAmpCorrection => 4,
            ParameterType.DisplacementAmpCorrection => 4,
            ParameterType.AccelerationAmpCorrection => 4,
            ParameterType.OrthogonalityCorrectionValue => 4,
            _ => 1 // 大部分枚举参数是1字节
        };
    }

    /// <summary>
    /// 获取枚举值对应的实际数值（用于显示）
    /// </summary>
    public static double GetParameterActualValue(ParameterType paramType, int enumValue)
    {
        return paramType switch
        {
            ParameterType.SamplingRate => enumValue switch
            {
                0x00 => 2000, 0x01 => 5000, 0x02 => 10000, 0x03 => 20000, 0x04 => 50000,
                0x05 => 100000, 0x06 => 200000, 0x07 => 400000, 0x08 => 800000, 0x09 => 1000000,
                0x0A => 2000000, 0x0B => 4000000, 0x0C => 5000000, 0x0D => 10000000, 0x0E => 20000000,
                _ => enumValue
            },
            ParameterType.LowPassFilter => enumValue switch
            {
                0x00 => 100, 0x01 => 500, 0x02 => 1000, 0x03 => 2000, 0x04 => 5000,
                0x05 => 10000, 0x06 => 20000, 0x07 => 40000, 0x08 => 80000, 0x09 => 100000,
                0x0A => 160000, 0x0B => 320000, 0x0C => 500000, 0x0D => 1000000, 0x0E => 3000000,
                _ => enumValue
            },
            ParameterType.VelocityRange => enumValue switch
            {
                0x00 => 24.5, 0x01 => 122.5, 0x02 => 245, 0x03 => 1225, 
                0x04 => 2450, 0x05 => 4900, 0x06 => 12250, 0x07 => 24500,
                0x08 => 49000, 0x09 => 122500, 0x0A => 245000, 0x0B => 490000,
                0x0C => 1225000, 0x0D => 2450000, 0x0E => 4900000,
                _ => enumValue
            },
            ParameterType.DisplacementRange => enumValue switch
            {
                0x00 => 2.45, 0x01 => 4.9, 0x02 => 12.25, 0x03 => 24.5, 
                0x04 => 49, 0x05 => 122.5, 0x06 => 245, 0x07 => 490,
                0x08 => 1225, 0x09 => 2450, 0x0A => 4900, 0x0B => 12250,
                0x0C => 24500, 0x0D => 49000, 0x0E => 122500,
                _ => enumValue
            },
            ParameterType.FrontendFilter => enumValue switch
            {
                0x00 => 100000, 0x01 => 300000, 0x02 => 500000, 0x03 => 1000000, 0x04 => 5000000, 0x05 => 8000000,
                _ => enumValue
            },
            ParameterType.AccelerationRange => enumValue + 1, // 档位1-15 (0x00-0x0E 映射到 1-15)
            ParameterType.DigitalRange => enumValue, // 中=0, 低=1, 高=2
            ParameterType.HighPassFilter => enumValue + 1, // 档位1-10 (0x00-0x09 映射到 1-10)
            // 对于纯枚举类型参数（不需要数值转换），直接返回枚举值
            ParameterType.UploadDataType => enumValue, // 速度=0, 位移=1, 加速度=2, I/Q=3
            ParameterType.LaserPowerIndicatorLevel => enumValue, // 0-10
            ParameterType.AnalogOutputType1 => enumValue switch { 0 => 0, 1 => 1, 2 => 2, _ => enumValue }, // 速度=0, 位移=1, 加速度=2
            ParameterType.AnalogOutputType2 => enumValue switch { 0 => 0, 1 => 1, 2 => 2, _ => enumValue }, // 速度=0, 位移=1, 加速度=2
            ParameterType.FrontendFilterType => enumValue switch { 0 => 0, 1 => 1, 2 => 2, _ => enumValue }, // hamming=0, hann=1, kaiser=2
            ParameterType.TriggerSampleType => enumValue,
            ParameterType.TriggerSampleMode => enumValue,
            ParameterType.TriggerSampleChannel => enumValue,
            ParameterType.OrthogonalityCorrectionMode => enumValue, // 自动=0, 手动=1
            // 默认返回枚举值本身
            _ => enumValue
        };
    }

    /// <summary>
    /// 将实际数值转换为枚举值（用于下发到设备）
    /// </summary>
    public static int GetParameterEnumValue(ParameterType paramType, double actualValue)
    {
        return paramType switch
        {
            ParameterType.SamplingRate => actualValue switch
            {
                2000 => 0x00, 5000 => 0x01, 10000 => 0x02, 20000 => 0x03, 50000 => 0x04,
                100000 => 0x05, 200000 => 0x06, 400000 => 0x07, 800000 => 0x08, 1000000 => 0x09,
                2000000 => 0x0A, 4000000 => 0x0B, 5000000 => 0x0C, 10000000 => 0x0D, 20000000 => 0x0E,
                _ => (int)actualValue
            },
            ParameterType.LowPassFilter => actualValue switch
            {
                100 => 0x00, 500 => 0x01, 1000 => 0x02, 2000 => 0x03, 5000 => 0x04,
                10000 => 0x05, 20000 => 0x06, 40000 => 0x07, 80000 => 0x08, 100000 => 0x09,
                160000 => 0x0A, 320000 => 0x0B, 500000 => 0x0C, 1000000 => 0x0D, 3000000 => 0x0E,
                _ => (int)actualValue
            },
            ParameterType.VelocityRange => actualValue switch
            {
                24.5 => 0x00, 122.5 => 0x01, 245 => 0x02, 1225 => 0x03, 
                2450 => 0x04, 4900 => 0x05, 12250 => 0x06, 24500 => 0x07,
                49000 => 0x08, 122500 => 0x09, 245000 => 0x0A, 490000 => 0x0B,
                1225000 => 0x0C, 2450000 => 0x0D, 4900000 => 0x0E,
                _ => (int)actualValue
            },
            ParameterType.DisplacementRange => actualValue switch
            {
                2.45 => 0x00, 4.9 => 0x01, 12.25 => 0x02, 24.5 => 0x03, 
                49 => 0x04, 122.5 => 0x05, 245 => 0x06, 490 => 0x07,
                1225 => 0x08, 2450 => 0x09, 4900 => 0x0A, 12250 => 0x0B,
                24500 => 0x0C, 49000 => 0x0D, 122500 => 0x0E,
                _ => (int)actualValue
            },
            ParameterType.FrontendFilter => actualValue switch
            {
                100000 => 0x00, 300000 => 0x01, 500000 => 0x02, 1000000 => 0x03, 5000000 => 0x04, 8000000 => 0x05,
                _ => (int)actualValue
            },
            // 对于非数值枚举类型，直接返回数值
            _ => (int)actualValue
        };
    }

    /// <summary>
    /// 获取频率显示文本，超过kHz的用MHz表示
    /// </summary>
    private static string? GetFrequencyDisplayText<T>(ParameterType paramType, int value) where T : Enum
    {
        if (value < 0 || value > GetMaxValue<T>())
            return null;

        var enumValue = (T)Enum.ToObject(typeof(T), (byte)value);
        
        return paramType switch
        {
            ParameterType.SamplingRate when enumValue is SamplingRate sr => sr switch
            {
                SamplingRate.KHz_2 => "2kHz",
                SamplingRate.KHz_5 => "5kHz", 
                SamplingRate.KHz_10 => "10kHz",
                SamplingRate.KHz_20 => "20kHz",
                SamplingRate.KHz_50 => "50kHz",
                SamplingRate.KHz_100 => "100kHz",
                SamplingRate.KHz_200 => "200kHz",
                SamplingRate.KHz_400 => "400kHz",
                SamplingRate.KHz_800 => "800kHz",
                SamplingRate.MHz_1 => "1MHz",
                SamplingRate.MHz_2 => "2MHz",
                SamplingRate.MHz_4 => "4MHz",
                SamplingRate.MHz_5 => "5MHz",
                SamplingRate.MHz_10 => "10MHz",
                SamplingRate.MHz_20 => "20MHz",
                _ => enumValue.ToString()
            },
            ParameterType.LowPassFilter when enumValue is LowPassFilter lpf => lpf switch
            {
                LowPassFilter.Hz_5 => "5Hz",
                LowPassFilter.Hz_10 => "10Hz",
                LowPassFilter.Hz_25 => "25Hz",
                LowPassFilter.Hz_50 => "50Hz",
                LowPassFilter.Hz_100 => "100Hz",
                LowPassFilter.Hz_250 => "250Hz",
                LowPassFilter.Hz_500 => "500Hz",
                LowPassFilter.KHz_1 => "1kHz",
                LowPassFilter.KHz_2_5 => "2.5kHz",
                LowPassFilter.KHz_5 => "5kHz",
                LowPassFilter.KHz_10 => "10kHz",
                LowPassFilter.KHz_25 => "25kHz",
                LowPassFilter.KHz_50 => "50kHz",
                LowPassFilter.KHz_100 => "100kHz",
                LowPassFilter.KHz_250 => "250kHz",
                _ => enumValue.ToString()
            },
            ParameterType.HighPassFilter when enumValue is HighPassFilter hpf => hpf switch
            {
                HighPassFilter.Hz_0_1 => "0.1Hz",
                HighPassFilter.Hz_0_2 => "0.2Hz",
                HighPassFilter.Hz_0_5 => "0.5Hz",
                HighPassFilter.Hz_1 => "1Hz",
                HighPassFilter.Hz_2 => "2Hz",
                HighPassFilter.Hz_5 => "5Hz",
                HighPassFilter.Hz_10 => "10Hz",
                HighPassFilter.Hz_20 => "20Hz",
                HighPassFilter.Hz_50 => "50Hz",
                HighPassFilter.Hz_100 => "100Hz",
                _ => enumValue.ToString()
            },
            ParameterType.FrontendFilter when enumValue is FrontendFilter ff => ff switch
            {
                FrontendFilter.MHz_0_5 => "0.5MHz",
                FrontendFilter.MHz_1 => "1MHz",
                FrontendFilter.MHz_2 => "2MHz",
                FrontendFilter.MHz_5 => "5MHz",
                FrontendFilter.MHz_10 => "10MHz",
                FrontendFilter.MHz_20 => "20MHz",
                _ => enumValue.ToString()
            },
            _ => null
        };
    }

    /// <summary>
    /// 获取枚举值的显示文本
    /// </summary>
    private static string? GetEnumDisplayText<T>(ParameterType paramType, int value) where T : Enum
    {
        if (!Enum.IsDefined(typeof(T), (byte)value))
            return null;

        var enumValue = (T)Enum.ToObject(typeof(T), (byte)value);
        return paramType switch
        {
            ParameterType.AnalogOutputType1 or ParameterType.AnalogOutputType2 when enumValue is AnalogOutputType aot => aot switch
            {
                AnalogOutputType.Velocity => "速度",
                AnalogOutputType.Displacement => "位移",
                AnalogOutputType.Acceleration => "加速度",
                _ => enumValue.ToString()
            },
            ParameterType.FrontendFilterType when enumValue is FrontendFilterType fft => fft switch
            {
                FrontendFilterType.Hamming => "hamming",
                FrontendFilterType.Hann => "hann",
                FrontendFilterType.Kaiser => "kaiser",
                _ => enumValue.ToString()
            },
            ParameterType.UploadDataType when enumValue is UploadDataType udt => udt switch
            {
                UploadDataType.Velocity => "速度",
                UploadDataType.Displacement => "位移",
                UploadDataType.Acceleration => "加速度",
                UploadDataType.IQSignal => "I、Q路信号",
                _ => enumValue.ToString()
            },
            ParameterType.DigitalRange when enumValue is DigitalRange dr => dr switch
            {
                DigitalRange.Medium => "中",
                DigitalRange.Low => "低",
                DigitalRange.High => "高",
                _ => enumValue.ToString()
            },
            ParameterType.OrthogonalityCorrectionMode when enumValue is OrthogonalityCorrectionMode ocm => ocm switch
            {
                OrthogonalityCorrectionMode.Auto => "自动",
                OrthogonalityCorrectionMode.Manual => "手动",
                _ => enumValue.ToString()
            },
            ParameterType.TriggerSampleType when enumValue is TriggerSampleType tst => tst switch
            {
                TriggerSampleType.FreeTrigger => "自由触发",
                TriggerSampleType.SoftwareTrigger => "软件触发",
                TriggerSampleType.HardwareTrigger => "硬件触发",
                TriggerSampleType.DebugTrigger => "调试触发",
                _ => enumValue.ToString()
            },
            ParameterType.TriggerSampleChannel when enumValue is TriggerSampleChannel tsc => tsc switch
            {
                TriggerSampleChannel.Channel1 => "通道1",
                TriggerSampleChannel.Channel2 => "通道2",
                _ => enumValue.ToString()
            },
            ParameterType.TriggerSampleMode when enumValue is TriggerSampleMode tsm => tsm switch
            {
                TriggerSampleMode.RisingEdge => "上升沿（高电平）",
                TriggerSampleMode.FallingEdge => "下降沿（低电平）",
                _ => enumValue.ToString()
            },
            // 开关状态参数
            ParameterType.AnalogOutputSwitch1 or 
            ParameterType.AnalogOutputSwitch2 or 
            ParameterType.FrontendFilterSwitch or 
            ParameterType.FrontendDcRemovalSwitch or 
            ParameterType.OrthogonalityCorrectionSwitch or 
            ParameterType.VelocityLowPassFilterSwitch or 
            ParameterType.DisplacementLowPassFilterSwitch or 
            ParameterType.AccelerationLowPassFilterSwitch 
                when enumValue is SwitchState ss => ss switch
            {
                SwitchState.Off => "关",
                SwitchState.On => "开",
                _ => enumValue.ToString()
            },
            _ => null
        };
    }

    /// <summary>
    /// 获取参数值的显示文本（包含实际值和单位）
    /// </summary>
    public static string GetParameterValueDisplayText(ParameterType paramType, int value)
    {
        // 尝试获取枚举显示文本
        var textWithName = paramType switch
        {
            ParameterType.AnalogOutputType1 or ParameterType.AnalogOutputType2 => GetEnumDisplayText<AnalogOutputType>(paramType, value),
            ParameterType.FrontendFilterType => GetEnumDisplayText<FrontendFilterType>(paramType, value),
            ParameterType.UploadDataType => GetEnumDisplayText<UploadDataType>(paramType, value),
            ParameterType.DigitalRange => GetEnumDisplayText<DigitalRange>(paramType, value),
            ParameterType.OrthogonalityCorrectionMode => GetEnumDisplayText<OrthogonalityCorrectionMode>(paramType, value),
            ParameterType.TriggerSampleChannel => GetEnumDisplayText<TriggerSampleChannel>(paramType, value),
            ParameterType.TriggerSampleMode => GetEnumDisplayText<TriggerSampleMode>(paramType, value),
            ParameterType.TriggerSampleType => GetEnumDisplayText<TriggerSampleType>(paramType, value),
            // 频率相关参数（采样率、滤波器）- 使用MHz表示超过kHz的频率
            ParameterType.SamplingRate => GetFrequencyDisplayText<SamplingRate>(paramType, value),
            ParameterType.LowPassFilter => GetFrequencyDisplayText<LowPassFilter>(paramType, value),
            ParameterType.HighPassFilter => GetFrequencyDisplayText<HighPassFilter>(paramType, value),
            ParameterType.FrontendFilter => GetFrequencyDisplayText<FrontendFilter>(paramType, value),
            // 开关状态参数
            ParameterType.AnalogOutputSwitch1 or 
            ParameterType.AnalogOutputSwitch2 or 
            ParameterType.FrontendFilterSwitch or 
            ParameterType.FrontendDcRemovalSwitch or 
            ParameterType.OrthogonalityCorrectionSwitch or 
            ParameterType.VelocityLowPassFilterSwitch or 
            ParameterType.DisplacementLowPassFilterSwitch or 
            ParameterType.AccelerationLowPassFilterSwitch => GetEnumDisplayText<SwitchState>(paramType, value),
            _ => null
        };
        
        if (textWithName != null)
        {
            return textWithName;
        }
        
        var actualValue = GetParameterActualValue(paramType, value);
        var unit = GetParameterUnit(paramType);
        
        // 对于枚举类型，显示实际值
        var parameter = CreateParameter(paramType);
        if (parameter is EnumParameter)
        {
            return string.IsNullOrEmpty(unit) ? actualValue.ToString() : $"{actualValue} {unit}";
        }
        
        // 对于普通数值类型，直接显示值
        return string.IsNullOrEmpty(unit) ? value.ToString() : $"{value} {unit}";
    }

    /// <summary>
    /// 创建对应类型的参数对象
    /// </summary>
    public static DeviceParameter CreateParameter(ParameterType paramType)
    {
        return paramType switch
        {
            // 枚举类型
            ParameterType.SamplingRate => new EnumParameter(),
            ParameterType.UploadDataType => new EnumParameter(),
            ParameterType.LaserPowerIndicatorLevel => new EnumParameter(),
            ParameterType.LowPassFilter => new EnumParameter(),
            ParameterType.HighPassFilter => new EnumParameter(),
            ParameterType.VelocityRange => new EnumParameter(),
            ParameterType.DisplacementRange => new EnumParameter(),
            ParameterType.AccelerationRange => new EnumParameter(),
            ParameterType.DigitalRange => new EnumParameter(),
            ParameterType.AnalogOutputType1 => new EnumParameter(),
            ParameterType.AnalogOutputType2 => new EnumParameter(),
            ParameterType.FrontendFilter => new EnumParameter(),
            ParameterType.FrontendFilterType => new EnumParameter(),
            ParameterType.TriggerSampleType => new EnumParameter(),
            ParameterType.TriggerSampleMode => new EnumParameter(),
            ParameterType.TriggerSampleChannel => new EnumParameter(),
            ParameterType.OrthogonalityCorrectionMode => new EnumParameter(),
            
            // 布尔类型
            ParameterType.AnalogOutputSwitch1 => new BoolParameter(),
            ParameterType.AnalogOutputSwitch2 => new BoolParameter(),
            ParameterType.FrontendFilterSwitch => new BoolParameter(),
            ParameterType.FrontendDcRemovalSwitch => new BoolParameter(),
            ParameterType.OrthogonalityCorrectionSwitch => new BoolParameter(),
            ParameterType.VelocityLowPassFilterSwitch => new BoolParameter(),
            ParameterType.DisplacementLowPassFilterSwitch => new BoolParameter(),
            ParameterType.AccelerationLowPassFilterSwitch => new BoolParameter(),
            
            // 整数类型
            ParameterType.DataSegmentLength => new IntegerParameter(),
            ParameterType.TriggerSampleLength => new IntegerParameter(),
            ParameterType.TriggerSampleDelay => new IntegerParameter(),
            
            // 浮点数类型
            ParameterType.LaserDriveCurrent => new FloatParameter(),
            ParameterType.TECTargetTemperature => new FloatParameter(),
            ParameterType.TriggerSampleLevel => new FloatParameter(),
            ParameterType.VelocityAmpCorrection => new FloatParameter(),
            ParameterType.DisplacementAmpCorrection => new FloatParameter(),
            ParameterType.AccelerationAmpCorrection => new FloatParameter(),
            ParameterType.OrthogonalityCorrectionValue => new FloatParameter(),
            
            // 默认数值类型
            _ => new NumberParameter()
        };
    }

    /// <summary>
    /// 获取参数描述
    /// </summary>
    public static string GetParameterDescription(ParameterType paramType)
    {
        return paramType switch
        {
            ParameterType.SamplingRate => "设置设备的数据采样频率，影响数据采集的时间分辨率",
            ParameterType.UploadDataType => "选择上传到主机的数据类型，如原始数据、速度、位移等",
            ParameterType.LaserDriveCurrent => "激光器驱动电流设置，影响激光功率和测量精度",
            ParameterType.TECTargetTemperature => "温度控制器的目标温度，保持激光器工作稳定",
            ParameterType.DataSegmentLength => "单次数据采集的长度，影响数据处理的实时性",
            ParameterType.LaserPowerIndicatorLevel => "激光功率指示等级，用于功率监控和安全",
            ParameterType.LowPassFilter => "低通滤波器设置，去除高频噪声",
            ParameterType.HighPassFilter => "高通滤波器设置，去除低频漂移",
            ParameterType.VelocityRange => "速度测量量程设置，影响速度测量的精度和范围",
            ParameterType.DisplacementRange => "位移测量量程设置，影响位移测量的精度和范围",
            ParameterType.AccelerationRange => "加速度测量量程设置，影响加速度测量的精度和范围",
            ParameterType.DigitalRange => "数字量程设置，影响数字信号的处理范围",
            ParameterType.AnalogOutputSwitch1 => "模拟输出通道1开关，控制模拟输出是否启用",
            ParameterType.AnalogOutputSwitch2 => "模拟输出通道2开关，控制模拟输出是否启用",
            ParameterType.AnalogOutputType1 => "模拟输出通道1类型设置，如电压、电流等",
            ParameterType.AnalogOutputType2 => "模拟输出通道2类型设置，如电压、电流等",
            ParameterType.FrontendFilter => "前端滤波器设置，对输入信号进行预处理",
            ParameterType.FrontendFilterSwitch => "前端滤波器开关，控制是否启用前端滤波",
            ParameterType.FrontendFilterType => "前端滤波器类型选择，如带通、低通等",
            ParameterType.FrontendDcRemovalSwitch => "前端直流分量去除开关，消除信号的直流偏移",
            ParameterType.TriggerSampleType => "触发采样类型设置，如上升沿、下降沿等",
            ParameterType.TriggerSampleMode => "触发采样模式设置，如单次、连续等",
            ParameterType.TriggerSampleChannel => "触发采样通道选择，指定触发信号源",
            ParameterType.TriggerSampleLength => "触发采样长度设置，控制采样点数",
            ParameterType.TriggerSampleDelay => "触发采样延迟设置，控制触发后的延迟时间",
            ParameterType.TriggerSampleLevel => "触发采样电平设置，设定触发阈值",
            ParameterType.VelocityAmpCorrection => "速度信号幅度校正系数，用于信号标定",
            ParameterType.DisplacementAmpCorrection => "位移信号幅度校正系数，用于信号标定",
            ParameterType.AccelerationAmpCorrection => "加速度信号幅度校正系数，用于信号标定",
            ParameterType.OrthogonalityCorrectionSwitch => "正交性校正开关，控制是否启用正交性校正",
            ParameterType.OrthogonalityCorrectionMode => "正交性校正模式设置，选择校正算法",
            ParameterType.OrthogonalityCorrectionValue => "正交性校正值设置，校正参数的具体数值",
            ParameterType.VelocityLowPassFilterSwitch => "速度低通滤波器开关，控制速度信号滤波",
            ParameterType.DisplacementLowPassFilterSwitch => "位移低通滤波器开关，控制位移信号滤波",
            ParameterType.AccelerationLowPassFilterSwitch => "加速度低通滤波器开关，控制加速度信号滤波",
            _ => "暂无描述信息"
        };
    }

    /// <summary>
    /// 获取参数分类
    /// </summary>
    public static string GetParameterCategory(ParameterType paramType)
    {
        return paramType switch
        {
            // 基础配置
            ParameterType.SamplingRate => "基础配置",
            ParameterType.UploadDataType => "基础配置",
            ParameterType.LaserPowerIndicatorLevel => "基础配置",
            ParameterType.SignalStrength => "基础配置",
            
            // 硬件参数配置
            ParameterType.LaserDriveCurrent => "硬件参数配置",
            ParameterType.TECTargetTemperature => "硬件参数配置",
            
            // 信号处理
            ParameterType.LowPassFilter => "信号处理配置",
            ParameterType.HighPassFilter => "信号处理配置",
            ParameterType.VelocityLowPassFilterSwitch => "信号处理配置",
            ParameterType.DisplacementLowPassFilterSwitch => "信号处理配置",
            ParameterType.AccelerationLowPassFilterSwitch => "信号处理配置",
            
            // 量程配置
            ParameterType.VelocityRange => "量程配置",
            ParameterType.DisplacementRange => "量程配置",
            ParameterType.AccelerationRange => "量程配置",
            ParameterType.DigitalRange => "量程配置",
            
            // 模拟口配置（合并1、2）
            ParameterType.AnalogOutputType1 => "模拟口配置",
            ParameterType.AnalogOutputSwitch1 => "模拟口配置",
            ParameterType.AnalogOutputType2 => "模拟口配置",
            ParameterType.AnalogOutputSwitch2 => "模拟口配置",
            
            // 触发采样
            ParameterType.TriggerSampleType => "触发采样配置",
            ParameterType.TriggerSampleMode => "触发采样配置",
            ParameterType.TriggerSampleLevel => "触发采样配置",
            ParameterType.TriggerSampleChannel => "触发采样配置",
            ParameterType.TriggerSampleLength => "触发采样配置",
            ParameterType.TriggerSampleDelay => "触发采样配置",
            
            // 算法参数配置（包含前端算法、校正算法等）
            ParameterType.FrontendFilter => "算法参数配置",
            ParameterType.FrontendFilterType => "算法参数配置",
            ParameterType.FrontendFilterSwitch => "算法参数配置",
            ParameterType.FrontendDcRemovalSwitch => "算法参数配置",
            ParameterType.DataSegmentLength => "算法参数配置",
            ParameterType.VelocityAmpCorrection => "算法参数配置",
            ParameterType.DisplacementAmpCorrection => "算法参数配置",
            ParameterType.AccelerationAmpCorrection => "算法参数配置",
            ParameterType.OrthogonalityCorrectionSwitch => "算法参数配置",
            ParameterType.OrthogonalityCorrectionMode => "算法参数配置",
            ParameterType.OrthogonalityCorrectionValue => "算法参数配置",
            
            _ => "其他配置"
        };
    }

    #endregion

    #region 事件处理

    /// <summary>
    /// 处理设备管理器状态消息变化
    /// </summary>
    private void OnStatusMessageChanged(object? sender, string message)
    {
        StatusMessage = message;
    }

    /// <summary>
    /// 处理设备信息更新
    /// </summary>
    private void OnDeviceInfoUpdated(object? sender, DeviceInfoData deviceInfo)
    {
        DeviceInfoText = $"MAC地址: {deviceInfo.MacAddressString}\n" +
                        $"序列号: {deviceInfo.SerialNumberString}\n" +
                        $"设备类型: {deviceInfo.DeviceType}\n" +
                        $"IP地址: {deviceInfo.IpAddressString}\n" +
                        $"子网掩码: {deviceInfo.SubnetMaskString}\n" +
                        $"网关: {deviceInfo.GatewayString}\n" +
                        $"DNS1: {deviceInfo.Dns1String}\n" +
                        $"DNS2: {deviceInfo.Dns2String}";
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
            _deviceManager.StatusMessageChanged -= OnStatusMessageChanged;
            _deviceManager.DeviceInfoUpdated -= OnDeviceInfoUpdated;
        }
    }

    #endregion
}