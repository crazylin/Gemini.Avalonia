using System.Net;
using System.Net.Sockets;
using AuroraUI.Framework.Logging;

namespace AuroraUI.IO.Net.TCP;

/// <summary>
/// 简单TCP服务器实现
/// </summary>
/// <typeparam name="T">数据包类型</typeparam>
public class EasyTcpServer<T> : ITcpServer<T> where T : class, new()
{
    private static readonly ILogger Logger = LogManager.GetLogger("AuroraUI.IO.EasyTcpServer");
    private Thread? _acceptThread;
    private List<ITcpClient<T>>? _clientList;
    private bool _running;
    private Socket? _server;

    /// <summary>
    /// 会话关闭事件
    /// </summary>
    public event EventHandler<ITcpClient<T>>? SessionClosed;
    
    /// <summary>
    /// 会话连接事件
    /// </summary>
    public event EventHandler<ITcpClient<T>>? SessionConnected;

    /// <summary>
    /// 处理会话关闭
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="client">客户端</param>
    public void OnSessionClosed(object sender, ITcpClient<T> client)
    {
        SessionClosed?.Invoke(sender, client);
    }

    /// <summary>
    /// 处理会话连接
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="client">客户端</param>
    public void OnSessionConnected(object sender, ITcpClient<T> client)
    {
        SessionConnected?.Invoke(sender, client);
    }

    /// <summary>
    /// 向指定客户端发送数据
    /// </summary>
    /// <param name="netDataStream">数据流</param>
    /// <param name="ipEndPoint">客户端端点</param>
    public void Send(T netDataStream, IPEndPoint ipEndPoint)
    {
        var client = _clientList?.FirstOrDefault(c => c.IpEndPoint?.Address.Equals(ipEndPoint.Address) == true);
        client?.SendMessage(netDataStream);
    }

    /// <summary>
    /// 启动服务器
    /// </summary>
    /// <param name="server">服务器端点</param>
    public void Start(IPEndPoint server)
    {
        if (_server != null)
            Stop();
            
        _clientList = new List<ITcpClient<T>>();
        _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _server.Bind(server);
        _server.Listen(1000);
        _running = true;
        
        _acceptThread = new Thread(() =>
        {
            while (_running)
            {
                try
                {
                    var socket = _server.Accept();
                    socket.SendBufferSize = 1024 * 1024; // 1MB
                    socket.ReceiveBufferSize = 1024 * 1024 * 5; // 5MB
                    // 启用 Nagle 算法（小数据包合并）
                    socket.NoDelay = false;

                    var client = new EasyTcpClient<T>(this, socket);
                    _clientList.Add(client);
                    OnSessionConnected(this, client);
                }
                catch (Exception e)
                {
                    Logger.Error($"EasyTcpServer accept client failed: {e}");
                }
            }
        })
        {
            IsBackground = true
        };
        _acceptThread.Start();
    }

    /// <summary>
    /// 停止服务器
    /// </summary>
    public void Stop()
    {
        _running = false;
        try
        {
            _server?.Close();
        }
        catch (Exception e)
        {
            Logger.Error($"EasyTcpServer socket close failed: {e}");
        }

        if (_clientList != null)
        {
            foreach (var client in _clientList)
            {
                try
                {
                    client?.Stop();
                }
                catch (Exception e)
                {
                    Logger.Error($"EasyTcpServer stop client {client?.IpEndPoint} failed: {e}");
                }
            }
            _clientList.Clear();
        }

        _acceptThread?.Join();
    }
}
