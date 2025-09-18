using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using AuroraUI.Framework.Logging;

namespace AuroraUI.IO.Net.TCP;

/// <summary>
/// 基于Pipeline的高性能TCP服务器
/// </summary>
/// <typeparam name="T">数据包类型</typeparam>
public class PipelineTcpServer<T> : IDisposable where T : class, IPipelineDataPackage<T>, IPacketWritable, new()
{
    private static readonly ILogger Logger = LogManager.GetLogger("AuroraUI.IO.PipelineTcpServer");
    private ConcurrentDictionary<EndPoint, PipelineTcpClient<T>>? _clients;
    private TcpListener? _listener;
    private IPEndPoint? _localEndPoint;
    private bool _running;
    private bool _disposed;

    /// <summary>
    /// 新客户端连接时触发
    /// </summary>
    public event EventHandler<PipelineTcpClient<T>>? ClientConnected;

    /// <summary>
    /// 客户端断开时触发
    /// </summary>
    public event EventHandler<PipelineTcpClient<T>>? ClientDisconnected;

    /// <summary>
    /// 当某个客户端收到一个完整包时触发
    /// </summary>
    public event EventHandler<(PipelineTcpClient<T> Client, T Packet)>? DataReceived;

    /// <summary>
    /// 启动监听
    /// </summary>
    /// <param name="localEndPoint">本地端点</param>
    public void Start(IPEndPoint localEndPoint)
    {
        if (_running)
            return;

        _localEndPoint = localEndPoint;

        _listener = new TcpListener(_localEndPoint);
        _listener.Server.NoDelay = true;
        _clients = new ConcurrentDictionary<EndPoint, PipelineTcpClient<T>>();

        try
        {
            _listener.Start(1024);
        }
        catch (Exception e)
        {
            Logger.Error($"PipelineTcpServer start failed: {e}");
            _running = false;
            return;
        }
        _running = true;
        AcceptLoop();
    }

    /// <summary>
    /// 停止并关闭所有客户端
    /// </summary>
    public void Stop()
    {
        _running = false;
        try
        {
            _listener?.Stop();
        }
        catch (Exception e)
        {
            Logger.Error($"PipelineTcpServer listener stop failed: {e}");
        }

        if (_clients != null)
        {
            foreach (var kv in _clients)
            {
                try
                {
                    kv.Value.Close();
                }
                catch (Exception e)
                {
                    Logger.Error($"PipelineTcpServer close client {kv.Key} failed: {e}");
                }
            }
            _clients.Clear();
        }
    }

    private void AcceptLoop()
    {
        Task.Run(async () =>
        {
            while (_running)
            {
                TcpClient clientSocket;
                try
                {
                    clientSocket = await _listener!.AcceptTcpClientAsync();
                }
                catch (Exception e)
                {
                    if (_running) // 只有在运行状态下才记录错误
                    {
                        Logger.Error($"PipelineTcpServer accept client failed: {e}");
                    }
                    break;
                }

                // 1) 为这个 clientSocket 创建一个 PipelineTcpClient
                var pipelineClient = new PipelineTcpClient<T>(clientSocket);

                // 2) 把它加入到客户端字典里
                _clients!.TryAdd(clientSocket.Client.RemoteEndPoint!, pipelineClient);

                // 3) 订阅该 client 的事件，用于转发给外层订阅者
                pipelineClient.DataReceived += (s, packet) => { DataReceived?.Invoke(this, (pipelineClient, packet)); };
                pipelineClient.Disconnected += (s, e) =>
                {
                    if (pipelineClient.RemoteEndPoint != null)
                    {
                        _clients.TryRemove(pipelineClient.RemoteEndPoint, out _);
                    }
                    ClientDisconnected?.Invoke(this, pipelineClient);
                };

                // 4) 通知外层"新的客户端连上来了"
                ClientConnected?.Invoke(this, pipelineClient);

                // 5) 启动它的拆包/接收循环
                pipelineClient.Start();
            }
        });
    }

    /// <summary>
    /// 向指定客户端发送一个包
    /// </summary>
    /// <param name="remoteEP">远程端点</param>
    /// <param name="packet">数据包</param>
    /// <returns>发送是否成功</returns>
    public async Task<bool> SendAsync(IPEndPoint remoteEP, T packet)
    {
        if (_clients?.TryGetValue(remoteEP, out var client) == true) 
        {
            return await client.SendAsync(packet);
        }
        return false;
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="disposing">是否正在释放</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Stop();
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
