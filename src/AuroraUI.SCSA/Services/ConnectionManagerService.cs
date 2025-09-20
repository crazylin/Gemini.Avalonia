using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reactive.Subjects;
using AuroraUI.Framework.Logging;
using AuroraUI.IO.Net.TCP;
using SCSA.Models;

namespace SCSA.Services;

/// <summary>
/// 连接管理器事件参数
/// </summary>
public class DeviceConnectionEventArgs : EventArgs
{
    public EnhancedDeviceConnection Device { get; }
    
    public DeviceConnectionEventArgs(EnhancedDeviceConnection device)
    {
        Device = device;
    }
}

/// <summary>
/// 数据接收事件参数
/// </summary>
public class DataReceivedEventArgs : EventArgs
{
    public EnhancedDeviceConnection Device { get; }
    public SCSADataPackage DataPackage { get; }
    
    public DataReceivedEventArgs(EnhancedDeviceConnection device, SCSADataPackage dataPackage)
    {
        Device = device;
        DataPackage = dataPackage;
    }
}

/// <summary>
/// 连接管理器接口
/// </summary>
public interface IConnectionManager : IDisposable
{
    /// <summary>
    /// 是否正在运行
    /// </summary>
    bool IsRunning { get; }
    
    /// <summary>
    /// 当前监听端点
    /// </summary>
    IPEndPoint? ListeningEndPoint { get; }
    
    /// <summary>
    /// 已连接的设备列表
    /// </summary>
    IReadOnlyList<EnhancedDeviceConnection> ConnectedDevices { get; }
    
    /// <summary>
    /// 设备连接事件
    /// </summary>
    event EventHandler<DeviceConnectionEventArgs>? DeviceConnected;
    
    /// <summary>
    /// 设备断开事件
    /// </summary>
    event EventHandler<DeviceConnectionEventArgs>? DeviceDisconnected;
    
    /// <summary>
    /// 连接设备列表变化事件
    /// </summary>
    event EventHandler? ConnectedDevicesChanged;
    
    /// <summary>
    /// 数据接收事件
    /// </summary>
    event EventHandler<DataReceivedEventArgs>? DataReceived;
    
    /// <summary>
    /// 状态变化事件
    /// </summary>
    event EventHandler<string>? StatusChanged;
    
    /// <summary>
    /// 启动连接管理器
    /// </summary>
    Task<bool> StartAsync(IPEndPoint endPoint);
    
    /// <summary>
    /// 停止连接管理器
    /// </summary>
    Task StopAsync();
    
    /// <summary>
    /// 刷新设备列表
    /// </summary>
    Task RefreshDevicesAsync();
    
    /// <summary>
    /// 断开指定设备
    /// </summary>
    Task DisconnectDeviceAsync(EnhancedDeviceConnection device);
    
    /// <summary>
    /// 向设备发送数据
    /// </summary>
    Task<bool> SendToDeviceAsync(EnhancedDeviceConnection device, SCSADataPackage package);
    
    /// <summary>
    /// 广播数据到所有设备
    /// </summary>
    Task<int> BroadcastAsync(SCSADataPackage package);
}

/// <summary>
/// TCP连接管理器实现
/// </summary>
[Export(typeof(IConnectionManager))]
[Export(typeof(TcpConnectionManager))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class TcpConnectionManager : IConnectionManager
{
    private static readonly ILogger Logger = LogManager.GetLogger("SCSA.ConnectionManager");
    
    private PipelineTcpServer<SCSADataPackage>? _tcpServer;
    private readonly ConcurrentDictionary<EndPoint, EnhancedDeviceConnection> _deviceConnections;
    private readonly object _lockObject = new();
    private bool _disposed;

    public bool IsRunning { get; private set; }
    public IPEndPoint? ListeningEndPoint { get; private set; }
    
    public IReadOnlyList<EnhancedDeviceConnection> ConnectedDevices
    {
        get
        {
            lock (_lockObject)
            {
                return _deviceConnections.Values.ToList();
            }
        }
    }

    public event EventHandler<DeviceConnectionEventArgs>? DeviceConnected;
    public event EventHandler<DeviceConnectionEventArgs>? DeviceDisconnected;
    public event EventHandler? ConnectedDevicesChanged;
    public event EventHandler<DataReceivedEventArgs>? DataReceived;
    public event EventHandler<string>? StatusChanged;

    public TcpConnectionManager()
    {
        _deviceConnections = new ConcurrentDictionary<EndPoint, EnhancedDeviceConnection>();
    }

    public async Task<bool> StartAsync(IPEndPoint endPoint)
    {
        if (IsRunning)
        {
            Logger.Warning("连接管理器已在运行中");
            return true;
        }

        try
        {
            _tcpServer = new PipelineTcpServer<SCSADataPackage>();
            
            // 订阅服务器事件
            _tcpServer.ClientConnected += OnClientConnected;
            _tcpServer.ClientDisconnected += OnClientDisconnected;
            _tcpServer.DataReceived += OnDataReceived;
            
            // 启动TCP服务器
            _tcpServer.Start(endPoint);
            
            IsRunning = true;
            ListeningEndPoint = endPoint;
            
            Logger.Info($"TCP连接管理器已启动，监听地址: {endPoint}");
            StatusChanged?.Invoke(this, $"正在监听 {endPoint}");
            
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error($"启动TCP连接管理器失败: {ex.Message}", ex);
            StatusChanged?.Invoke(this, $"启动失败: {ex.Message}");
            
            // 清理资源
            await CleanupAsync();
            return false;
        }
    }

    public async Task StopAsync()
    {
        if (!IsRunning)
        {
            Logger.Warning("连接管理器未在运行");
            return;
        }

        try
        {
            Logger.Info("正在停止TCP连接管理器...");
            
            // 断开所有设备连接
            var devices = ConnectedDevices.ToList();
            var disconnectTasks = devices.Select(device => DisconnectDeviceAsync(device));
            await Task.WhenAll(disconnectTasks);
            
            // 停止TCP服务器
            _tcpServer?.Stop();
            
            await CleanupAsync();
            
            Logger.Info("TCP连接管理器已停止");
            StatusChanged?.Invoke(this, "已停止");
        }
        catch (Exception ex)
        {
            Logger.Error($"停止TCP连接管理器失败: {ex.Message}", ex);
            StatusChanged?.Invoke(this, $"停止失败: {ex.Message}");
        }
    }

    public async Task DisconnectDeviceAsync(EnhancedDeviceConnection device)
    {
        try
        {
            if (_deviceConnections.TryRemove(device.EndPoint!, out var removedDevice))
            {
                removedDevice.TcpClient?.Close();
                removedDevice.IsConnected = false;
                
                Logger.Info($"设备已断开: {device.DeviceId} ({device.EndPoint})");
                DeviceDisconnected?.Invoke(this, new DeviceConnectionEventArgs(removedDevice));
                ConnectedDevicesChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"断开设备连接失败: {device.DeviceId}", ex);
        }
        
        await Task.CompletedTask;
    }

    public async Task<bool> SendToDeviceAsync(EnhancedDeviceConnection device, SCSADataPackage package)
    {
        try
        {
            if (device.TcpClient == null || !device.IsConnected)
            {
                Logger.Warning($"设备未连接: {device.DeviceId}");
                return false;
            }

            var success = await device.TcpClient.SendAsync(package);
            if (success)
            {
                device.LastCommunicationTime = DateTime.Now;
                device.CommunicationStatus = "正常";
                Logger.Debug($"向设备发送数据成功: {device.DeviceId}, 包: {package}");
            }
            else
            {
                device.CommunicationStatus = "发送失败";
                Logger.Warning($"向设备发送数据失败: {device.DeviceId}");
            }

            return success;
        }
        catch (Exception ex)
        {
            device.CommunicationStatus = "发送异常";
            Logger.Error($"向设备发送数据异常: {device.DeviceId}", ex);
            return false;
        }
    }

    public async Task<int> BroadcastAsync(SCSADataPackage package)
    {
        var devices = ConnectedDevices.ToList();
        var sendTasks = devices.Select(device => SendToDeviceAsync(device, package));
        var results = await Task.WhenAll(sendTasks);
        
        var successCount = results.Count(r => r);
        Logger.Debug($"广播消息完成: {successCount}/{devices.Count} 设备发送成功");
        
        return successCount;
    }

    public async Task RefreshDevicesAsync()
    {
        try
        {
            Logger.Info("正在刷新设备列表...");
            
            // 检查现有连接状态
            var devicesToRemove = new List<EnhancedDeviceConnection>();
            
            foreach (var device in ConnectedDevices.ToList())
            {
                if (device.TcpClient == null || !device.IsConnected)
                {
                    devicesToRemove.Add(device);
                }
            }
            
            // 移除断开的设备
            foreach (var device in devicesToRemove)
            {
                await DisconnectDeviceAsync(device);
            }
            
            // 触发设备列表变化事件
            ConnectedDevicesChanged?.Invoke(this, EventArgs.Empty);
            
            Logger.Info($"设备列表刷新完成，当前连接设备数: {ConnectedDevices.Count}");
        }
        catch (Exception ex)
        {
            Logger.Error("刷新设备列表失败", ex);
            StatusChanged?.Invoke(this, $"刷新设备列表失败: {ex.Message}");
        }
    }

    private void OnClientConnected(object? sender, PipelineTcpClient<SCSADataPackage> client)
    {
        try
        {
            var device = new EnhancedDeviceConnection
            {
                DeviceId = $"Device_{DateTime.Now:yyyyMMddHHmmss}_{_deviceConnections.Count + 1:D3}",
                DeviceName = "SCSA设备",
                EndPoint = client.RemoteEndPoint,
                ConnectTime = DateTime.Now,
                IsConnected = true,
                DeviceType = "TCP SCSA设备",
                TcpClient = client,
                LastCommunicationTime = DateTime.Now,
                CommunicationStatus = "已连接"
            };

            if (_deviceConnections.TryAdd(client.RemoteEndPoint!, device))
            {
                Logger.Info($"新设备连接: {device.DeviceId} ({device.EndPoint})");
                DeviceConnected?.Invoke(this, new DeviceConnectionEventArgs(device));
                ConnectedDevicesChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"处理客户端连接失败: {client.RemoteEndPoint}", ex);
        }
    }

    private void OnClientDisconnected(object? sender, PipelineTcpClient<SCSADataPackage> client)
    {
        try
        {
            if (_deviceConnections.TryRemove(client.RemoteEndPoint!, out var device))
            {
                device.IsConnected = false;
                device.CommunicationStatus = "已断开";
                
                Logger.Info($"设备断开连接: {device.DeviceId} ({device.EndPoint})");
                DeviceDisconnected?.Invoke(this, new DeviceConnectionEventArgs(device));
                ConnectedDevicesChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"处理客户端断开失败: {client.RemoteEndPoint}", ex);
        }
    }

    private void OnDataReceived(object? sender, (PipelineTcpClient<SCSADataPackage> Client, SCSADataPackage Packet) e)
    {
        try
        {
            if (_deviceConnections.TryGetValue(e.Client.RemoteEndPoint!, out var device))
            {
                device.LastCommunicationTime = DateTime.Now;
                device.CommunicationStatus = "正常";
                
                Logger.Debug($"收到设备数据: {device.DeviceId}, 包: {e.Packet}");
                DataReceived?.Invoke(this, new DataReceivedEventArgs(device, e.Packet));
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"处理数据接收失败: {e.Client.RemoteEndPoint}", ex);
        }
    }

    private async Task CleanupAsync()
    {
        try
        {
            if (_tcpServer != null)
            {
                _tcpServer.ClientConnected -= OnClientConnected;
                _tcpServer.ClientDisconnected -= OnClientDisconnected;
                _tcpServer.DataReceived -= OnDataReceived;
                _tcpServer.Dispose();
                _tcpServer = null;
            }

            _deviceConnections.Clear();
            IsRunning = false;
            ListeningEndPoint = null;
        }
        catch (Exception ex)
        {
            Logger.Error("清理连接管理器资源失败", ex);
        }
        
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        try
        {
            StopAsync().Wait(TimeSpan.FromSeconds(5));
        }
        catch (Exception ex)
        {
            Logger.Error("释放连接管理器资源时出错", ex);
        }
        
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// 网络工具类
/// </summary>
public static class NetworkHelper
{
    /// <summary>
    /// 获取可用的网络接口列表
    /// </summary>
    public static List<NetworkInterfaceInfo> GetAvailableNetworkInterfaces()
    {
        var interfaces = new List<NetworkInterfaceInfo>();
        
        try
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                           ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up);

            foreach (var ni in networkInterfaces)
            {
                var ipAddress = GetIpAddress(ni);
                if (!string.IsNullOrEmpty(ipAddress) && ipAddress != "无 IP 地址")
                {
                    interfaces.Add(new NetworkInterfaceInfo
                    {
                        Name = ni.Name,
                        Description = ni.Description,
                        IPAddress = ipAddress,
                        IsAvailable = true
                    });
                }
            }
        }
        catch (Exception ex)
        {
            LogManager.GetLogger("SCSA.NetworkHelper").Error("获取网络接口失败", ex);
        }
        
        return interfaces.OrderBy(ni => ni.Name).ToList();
    }

    /// <summary>
    /// 获取网络接口的IPv4地址
    /// </summary>
    private static string GetIpAddress(NetworkInterface networkInterface)
    {
        return networkInterface.GetIPProperties().UnicastAddresses
                   .FirstOrDefault(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork)?.Address
                   ?.ToString() ?? "无 IP 地址";
    }

    /// <summary>
    /// 根据网络接口信息和端口创建EndPoint
    /// </summary>
    public static IPEndPoint CreateEndPoint(NetworkInterfaceInfo networkInterface, int port)
    {
        if (!IPAddress.TryParse(networkInterface.IPAddress, out var ipAddress))
        {
            throw new InvalidOperationException($"无效的IP地址: {networkInterface.IPAddress}");
        }
        return new IPEndPoint(ipAddress, port);
    }
}
