using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SCSA.Models;
using SCSA.Services;
using AuroraUI.Framework.Logging;

namespace SCSA.ViewModels;

/// <summary>
/// 设备连接管理ViewModel - 使用统一的设备管理器
/// </summary>
[Export(typeof(DeviceConnectionViewModel))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public class DeviceConnectionViewModel : ReactiveObject, IDisposable
{
    private static readonly ILogger Logger = LogManager.GetLogger("AuroraUI.SCSA.DeviceConnection");
    private readonly IConnectionManager _connectionManager;
    private readonly IDeviceManager _deviceManager;
    private bool _disposed;

    /// <summary>
    /// 监听端口
    /// </summary>
    [Reactive]
    public int Port { get; set; } = 9123;

    /// <summary>
    /// 选中的网络接口
    /// </summary>
    [Reactive]
    public NetworkInterfaceInfo? SelectedInterface { get; set; }

    /// <summary>
    /// 选中的设备
    /// </summary>
    [Reactive]
    public EnhancedDeviceConnection? SelectedDevice { get; set; }

    /// <summary>
    /// 服务器是否运行中
    /// </summary>
    [Reactive]
    public bool IsServerRunning { get; set; }

    /// <summary>
    /// 状态消息
    /// </summary>
    [Reactive]
    public string StatusMessage { get; set; } = "就绪";

    /// <summary>
    /// 网络接口列表
    /// </summary>
    public ObservableCollection<NetworkInterfaceInfo> NetworkInterfaces { get; } = new();

    /// <summary>
    /// 连接的设备列表
    /// </summary>
    public ObservableCollection<EnhancedDeviceConnection> ConnectedDevices { get; } = new();

    /// <summary>
    /// 启动服务器命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> StartServerCommand { get; }

    /// <summary>
    /// 停止服务器命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> StopServerCommand { get; }

    /// <summary>
    /// 断开设备命令
    /// </summary>
    public ReactiveCommand<EnhancedDeviceConnection, Unit> DisconnectDeviceCommand { get; }

    /// <summary>
    /// 刷新网络接口命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> RefreshInterfacesCommand { get; }

    /// <summary>
    /// 选择设备命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> SelectDeviceCommand { get; }

    /// <summary>
    /// 确认选择命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> ConfirmSelectionCommand { get; }

    /// <summary>
    /// 取消命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    /// <summary>
    /// 设备选择事件
    /// </summary>
    public event EventHandler<EnhancedDeviceConnection>? DeviceSelected;

    /// <summary>
    /// 对话框关闭事件
    /// </summary>
    public event EventHandler? DialogClosed;

    [ImportingConstructor]
    public DeviceConnectionViewModel(IConnectionManager connectionManager, IDeviceManager deviceManager)
    {
        _connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));
        _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
        
        // 订阅连接管理器事件
        _connectionManager.DeviceConnected += OnDeviceConnected;
        _connectionManager.DeviceDisconnected += OnDeviceDisconnected;
        _connectionManager.StatusChanged += OnStatusChanged;

        // 初始化网络接口
        InitializeNetworkInterfaces();

        // 定义命令的可执行条件
        var canStart = this.WhenAnyValue(x => x.SelectedInterface, x => x.IsServerRunning)
            .Select(x => x.Item1 != null && !x.Item2);

        var canStop = this.WhenAnyValue(x => x.IsServerRunning);

        var hasSelectedDevice = this.WhenAnyValue(x => x.SelectedDevice)
            .Select(device => device != null);

        // 创建命令
        StartServerCommand = ReactiveCommand.CreateFromTask(StartServer, canStart);
        StopServerCommand = ReactiveCommand.CreateFromTask(StopServer, canStop);
        DisconnectDeviceCommand = ReactiveCommand.CreateFromTask<EnhancedDeviceConnection>(DisconnectDevice);
        RefreshInterfacesCommand = ReactiveCommand.Create(InitializeNetworkInterfaces);
        SelectDeviceCommand = ReactiveCommand.Create(SelectDevice, hasSelectedDevice);
        ConfirmSelectionCommand = ReactiveCommand.Create(ConfirmSelection, hasSelectedDevice);
        CancelCommand = ReactiveCommand.Create(Cancel);

        // 监听服务器运行状态变化
        this.WhenAnyValue(x => x.IsServerRunning)
            .Subscribe(running =>
            {
                if (!running && _connectionManager.IsRunning)
                {
                    IsServerRunning = _connectionManager.IsRunning;
                }
            });
    }

    /// <summary>
    /// 启动TCP服务器
    /// </summary>
    private async Task StartServer()
    {
        try
        {
            if (SelectedInterface == null)
            {
                StatusMessage = "请选择网络接口";
                return;
            }

            var endPoint = NetworkHelper.CreateEndPoint(SelectedInterface, Port);
            var success = await _connectionManager.StartAsync(endPoint);
            
            IsServerRunning = success;
            
            if (success)
            {
                StatusMessage = $"正在监听 {SelectedInterface.Name}:{Port}";
                Logger.Info($"TCP服务器已启动，监听地址: {endPoint}");
            }
            else
            {
                StatusMessage = "启动失败，请检查网络设置";
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"启动TCP服务器失败: {ex.Message}", ex);
            StatusMessage = $"启动失败: {ex.Message}";
            IsServerRunning = false;
        }
    }

    /// <summary>
    /// 停止TCP服务器
    /// </summary>
    private async Task StopServer()
    {
        try
        {
            await _connectionManager.StopAsync();
            IsServerRunning = false;
            StatusMessage = "已停止";
            
            // 清空连接的设备
            ConnectedDevices.Clear();
            SelectedDevice = null;
            
            Logger.Info("TCP服务器已停止");
        }
        catch (Exception ex)
        {
            Logger.Error($"停止TCP服务器失败: {ex.Message}", ex);
            StatusMessage = $"停止失败: {ex.Message}";
        }
    }

    /// <summary>
    /// 断开设备连接
    /// </summary>
    private async Task DisconnectDevice(EnhancedDeviceConnection device)
    {
        try
        {
            await _connectionManager.DisconnectDeviceAsync(device);
            StatusMessage = $"已断开设备: {device.DeviceId}";
        }
        catch (Exception ex)
        {
            Logger.Error($"断开设备失败: {ex.Message}", ex);
            StatusMessage = $"断开失败: {ex.Message}";
        }
    }

    /// <summary>
    /// 选择当前设备
    /// </summary>
    private void SelectDevice()
    {
        if (SelectedDevice != null)
        {
            DeviceSelected?.Invoke(this, SelectedDevice);
            Logger.Info($"用户选择设备: {SelectedDevice.DeviceId}");
        }
    }

    /// <summary>
    /// 确认选择并关闭对话框 - 连接到设备管理器
    /// </summary>
    private async void ConfirmSelection()
    {
        if (SelectedDevice != null)
        {
            Logger.Info($"正在连接到设备: {SelectedDevice.DeviceId}");
            
            // 通过设备管理器连接设备
            var success = await _deviceManager.ConnectToDeviceAsync(SelectedDevice);
            
            if (success)
            {
                DeviceSelected?.Invoke(this, SelectedDevice);
                Logger.Info($"设备连接成功: {SelectedDevice.DeviceId}");
            }
            else
            {
                Logger.Warning($"设备连接失败: {SelectedDevice.DeviceId}");
                // 这里可以显示错误消息
                return; // 连接失败时不关闭对话框
            }
        }
        
        DialogClosed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 取消并关闭对话框
    /// </summary>
    private void Cancel()
    {
        Logger.Info("用户取消设备选择");
        DialogClosed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 设备连接事件处理
    /// </summary>
    private void OnDeviceConnected(object? sender, DeviceConnectionEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            ConnectedDevices.Add(e.Device);
            StatusMessage = $"新设备已连接: {e.Device.EndPoint}";
            
            // 如果没有选中设备，自动选中新连接的设备
            if (SelectedDevice == null)
            {
                SelectedDevice = e.Device;
            }
        });
    }

    /// <summary>
    /// 设备断开事件处理
    /// </summary>
    private void OnDeviceDisconnected(object? sender, DeviceConnectionEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            ConnectedDevices.Remove(e.Device);
            
            if (SelectedDevice == e.Device)
            {
                SelectedDevice = ConnectedDevices.FirstOrDefault();
            }
            
            StatusMessage = $"设备已断开: {e.Device.DeviceId}";
        });
    }

    /// <summary>
    /// 状态变化事件处理
    /// </summary>
    private void OnStatusChanged(object? sender, string message)
    {
        Dispatcher.UIThread.Post(() =>
        {
            StatusMessage = message;
            IsServerRunning = _connectionManager.IsRunning;
        });
    }

    /// <summary>
    /// 初始化网络接口列表
    /// </summary>
    private void InitializeNetworkInterfaces()
    {
        try
        {
            NetworkInterfaces.Clear();
            
            var interfaces = NetworkHelper.GetAvailableNetworkInterfaces();
            foreach (var networkInterface in interfaces)
            {
                NetworkInterfaces.Add(networkInterface);
            }

            // 自动选择第一个可用接口
            if (SelectedInterface == null && NetworkInterfaces.Count > 0)
            {
                SelectedInterface = NetworkInterfaces.First();
            }
            
            Logger.Info($"已加载 {NetworkInterfaces.Count} 个网络接口");
        }
        catch (Exception ex)
        {
            Logger.Error($"初始化网络接口失败: {ex.Message}", ex);
            StatusMessage = "获取网络接口失败";
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        try
        {
            _connectionManager?.Dispose();
        }
        catch (Exception ex)
        {
            Logger.Error("释放连接管理器资源时出错", ex);
        }
        
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}