using System.Net;
using System.Net.Sockets;
using AuroraUI.Framework.Logging;

namespace AuroraUI.IO.Net.TCP;

/// <summary>
/// 简单TCP客户端实现
/// </summary>
/// <typeparam name="T">数据包类型</typeparam>
public class EasyTcpClient<T> : ITcpClient<T> where T : class, new()
{
    private static readonly ILogger Logger = LogManager.GetLogger("AuroraUI.IO.EasyTcpClient");
    private readonly Thread _receiveThread;
    private bool _running;
    private Socket? _socket;

    /// <summary>
    /// 构造函数（服务器端使用）
    /// </summary>
    /// <param name="server">TCP服务器</param>
    /// <param name="socket">Socket连接</param>
    public EasyTcpClient(ITcpServer<T> server, Socket socket)
    {
        TcpServer = server;
        _socket = socket;
        IpEndPoint = socket.RemoteEndPoint as IPEndPoint;
        var stream = new BinaryReader(new NetworkStream(socket, true));
        
        _receiveThread = new Thread(() =>
        {
            while (_running) //循环接收消息
            {
                try
                {
                    var netDataPackage = new T();
                    ((INetDataPackage)netDataPackage).Read(stream);
                    DataReceived?.Invoke(this, netDataPackage);
                }
                catch (Exception e)
                {
                    Logger.Error($"EasyTcpClient receive data failed: {e}");
                    TcpServer.OnSessionClosed(TcpServer, this);
                    _socket?.Close();
                    _socket = null;
                    break;
                }
            }
        })
        {
            IsBackground = true
        }; //开启线程执行循环接收消息
    }

    /// <summary>
    /// 构造函数（客户端使用）
    /// </summary>
    public EasyTcpClient()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        var stream = new BinaryReader(new NetworkStream(_socket, true));
        _receiveThread = new Thread(() =>
        {
            while (_running) //循环接收消息
            {
                try
                {
                    var netDataPackage = new T();
                    ((INetDataPackage)netDataPackage).Read(stream);
                    DataReceived?.Invoke(this, netDataPackage);
                }
                catch (Exception e)
                {
                    Logger.Error($"EasyTcpClient receive data failed: {e}");
                    TcpServer?.OnSessionClosed(TcpServer, this);
                    _socket?.Close();
                    _socket = null;
                    break;
                }
            }
        })
        {
            IsBackground = true
        }; //开启线程执行循环接收消息
    }

    /// <summary>
    /// 数据接收事件
    /// </summary>
    public event EventHandler<T>? DataReceived;
    
    /// <summary>
    /// IP端点信息
    /// </summary>
    public IPEndPoint? IpEndPoint { set; get; }

    /// <summary>
    /// 关联的TCP服务器
    /// </summary>
    public ITcpServer<T>? TcpServer { set; get; }

    /// <summary>
    /// 启动客户端
    /// </summary>
    public void Start()
    {
        _running = true;
        _receiveThread.Start();
    }

    /// <summary>
    /// 停止客户端
    /// </summary>
    public void Stop()
    {
        _running = false;
        try
        {
            _socket?.Close();
        }
        catch (Exception e)
        {
            Logger.Error($"EasyTcpClient socket close failed: {e}");
        }
        _socket = null;
        _receiveThread.Join();
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="netDataPackage">网络数据包</param>
    /// <returns>发送是否成功</returns>
    public bool SendMessage(T netDataPackage) //发送消息
    {
        try
        {
            var bytes = ((INetDataPackage)netDataPackage).Get();
            return _socket?.Send(bytes) == bytes.Length;
        }
        catch (Exception e)
        {
            Logger.Error($"EasyTcpClient send message failed: {e}");
            return false;
        }
    }

    /// <summary>
    /// 连接状态
    /// </summary>
    public bool? Connected => _socket?.Connected;
}
