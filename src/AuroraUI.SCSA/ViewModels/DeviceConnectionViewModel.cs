using System.Buffers;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SCSA.Models;
using AuroraUI.Framework.Logging;
using AuroraUI.IO.Net.TCP;

namespace SCSA.ViewModels;

/// <summary>
/// 设备连接管理ViewModel
/// </summary>
[Export(typeof(DeviceConnectionViewModel))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public class DeviceConnectionViewModel : ReactiveObject
{
    private static readonly ILogger Logger = LogManager.GetLogger("AuroraUI.SCSA.DeviceConnection");
    private PipelineTcpServer<TestDataPackage>? _tcpServer;

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
    public DeviceConnection? SelectedDevice { get; set; }

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
    public ObservableCollection<DeviceConnection> ConnectedDevices { get; } = new();

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
    public ReactiveCommand<DeviceConnection, Unit> DisconnectDeviceCommand { get; }

    /// <summary>
    /// 刷新网络接口命令
    /// </summary>
    public ReactiveCommand<Unit, Unit> RefreshInterfacesCommand { get; }

    public DeviceConnectionViewModel()
    {
        // 初始化网络接口
        InitializeNetworkInterfaces();

        // 定义命令的可执行条件
        var canStart = this.WhenAnyValue(x => x.SelectedInterface, x => x.IsServerRunning)
            .Select(x => x.Item1 != null && !x.Item2);

        var canStop = this.WhenAnyValue(x => x.IsServerRunning);

        // 创建命令
        StartServerCommand = ReactiveCommand.Create(StartServer, canStart);
        StopServerCommand = ReactiveCommand.Create(StopServer, canStop);
        DisconnectDeviceCommand = ReactiveCommand.Create<DeviceConnection>(DisconnectDevice);
        RefreshInterfacesCommand = ReactiveCommand.Create(InitializeNetworkInterfaces);

        // 监听属性变化
        this.WhenAnyValue(x => x.IsServerRunning)
            .Subscribe(running =>
            {
                StatusMessage = running ? $"正在监听端口 {Port}" : "已停止";
            });
    }

    /// <summary>
    /// 启动TCP服务器
    /// </summary>
    private void StartServer()
    {
        try
        {
            if (SelectedInterface == null)
            {
                StatusMessage = "请选择网络接口";
                return;
            }

            var endPoint = GetEndPoint(SelectedInterface, Port);
            _tcpServer = new PipelineTcpServer<TestDataPackage>();
            
            // 订阅事件
            _tcpServer.ClientConnected += OnClientConnected;
            _tcpServer.ClientDisconnected += OnClientDisconnected;
            
            _tcpServer.Start(endPoint);
            IsServerRunning = true;
            StatusMessage = $"正在监听 {SelectedInterface.Name}:{Port}";
            
            Logger.Info($"TCP服务器已启动，监听地址: {endPoint}");
        }
        catch (Exception ex)
        {
            Logger.Error($"启动TCP服务器失败: {ex.Message}");
            StatusMessage = $"启动失败: {ex.Message}";
        }
    }

    /// <summary>
    /// 停止TCP服务器
    /// </summary>
    private void StopServer()
    {
        try
        {
            _tcpServer?.Stop();
            _tcpServer = null;
            IsServerRunning = false;
            StatusMessage = "已停止";
            
            // 清空连接的设备
            ConnectedDevices.Clear();
            SelectedDevice = null;
            
            Logger.Info("TCP服务器已停止");
        }
        catch (Exception ex)
        {
            Logger.Error($"停止TCP服务器失败: {ex.Message}");
            StatusMessage = $"停止失败: {ex.Message}";
        }
    }

    /// <summary>
    /// 断开设备连接
    /// </summary>
    /// <param name="device">要断开的设备</param>
    private void DisconnectDevice(DeviceConnection device)
    {
        try
        {
            ConnectedDevices.Remove(device);
            if (SelectedDevice == device)
            {
                SelectedDevice = null;
            }
            
            StatusMessage = $"已断开设备: {device.DeviceId}";
            Logger.Info($"设备已断开: {device.DeviceId} ({device.EndPoint})");
        }
        catch (Exception ex)
        {
            Logger.Error($"断开设备失败: {ex.Message}");
            StatusMessage = $"断开失败: {ex.Message}";
        }
    }

    /// <summary>
    /// 客户端连接事件处理
    /// </summary>
    private void OnClientConnected(object? sender, PipelineTcpClient<TestDataPackage> client)
    {
        var device = new DeviceConnection
        {
            DeviceId = $"Device_{ConnectedDevices.Count + 1:D3}",
            DeviceName = "未知设备",
            EndPoint = client.RemoteEndPoint,
            ConnectTime = DateTime.Now,
            IsConnected = true,
            DeviceType = "TCP设备"
        };

        ConnectedDevices.Add(device);
        StatusMessage = $"新设备已连接: {device.EndPoint}";
        
        Logger.Info($"新设备连接: {device.DeviceId} ({device.EndPoint})");
        
        // 如果没有选中设备，自动选中新连接的设备
        if (SelectedDevice == null)
        {
            SelectedDevice = device;
        }
    }

    /// <summary>
    /// 客户端断开事件处理
    /// </summary>
    private void OnClientDisconnected(object? sender, PipelineTcpClient<TestDataPackage> client)
    {
        var device = ConnectedDevices.FirstOrDefault(d => Equals(d.EndPoint, client.RemoteEndPoint));
        if (device != null)
        {
            device.IsConnected = false;
            ConnectedDevices.Remove(device);
            
            if (SelectedDevice == device)
            {
                SelectedDevice = null;
            }
            
            StatusMessage = $"设备已断开: {device.DeviceId}";
            Logger.Info($"设备断开: {device.DeviceId} ({device.EndPoint})");
        }
    }

    /// <summary>
    /// 初始化网络接口列表
    /// </summary>
    private void InitializeNetworkInterfaces()
    {
        try
        {
            NetworkInterfaces.Clear();
            
            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                           ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
                .Select(ni => new NetworkInterfaceInfo
                {
                    Name = ni.Name,
                    Description = ni.Description,
                    IPAddress = GetIpAddress(ni),
                    IsAvailable = true
                })
                .Where(ni => !string.IsNullOrEmpty(ni.IPAddress) && ni.IPAddress != "无 IP 地址")
                .OrderBy(ni => ni.Name);

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
            Logger.Error($"初始化网络接口失败: {ex.Message}");
            StatusMessage = "获取网络接口失败";
        }
    }

    /// <summary>
    /// 获取网络接口的IP地址
    /// </summary>
    private static string GetIpAddress(NetworkInterface networkInterface)
    {
        return networkInterface.GetIPProperties().UnicastAddresses
                   .FirstOrDefault(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork)?.Address
                   ?.ToString() ?? "无 IP 地址";
    }

    /// <summary>
    /// 根据网络接口和端口获取EndPoint
    /// </summary>
    private static IPEndPoint GetEndPoint(NetworkInterfaceInfo networkInterface, int port)
    {
        if (!IPAddress.TryParse(networkInterface.IPAddress, out var ipAddress))
        {
            throw new InvalidOperationException($"无效的IP地址: {networkInterface.IPAddress}");
        }
        return new IPEndPoint(ipAddress, port);
    }
}

/// <summary>
/// 测试数据包（临时实现）
/// </summary>
public class TestDataPackage : IPipelineDataPackage<TestDataPackage>, IPacketWritable
{
    public byte[] Data { get; set; } = Array.Empty<byte>();

    public bool TryParse(ReadOnlySequence<byte> buffer, out TestDataPackage packet, out SequencePosition frameEnd)
    {
        packet = new TestDataPackage();
        frameEnd = buffer.Start;
        
        // 简单实现：如果有数据就认为是一个完整包
        if (buffer.Length > 0)
        {
            packet.Data = buffer.ToArray();
            frameEnd = buffer.End;
            return true;
        }
        
        return false;
    }

    public byte[] GetBytes()
    {
        return Data;
    }
}
