using System.Net;

namespace AuroraUI.IO.Net.TCP;

/// <summary>
/// TCP客户端接口
/// </summary>
/// <typeparam name="T">数据包类型</typeparam>
public interface ITcpClient<T> where T : class, new()
{
    /// <summary>
    /// 关联的TCP服务器
    /// </summary>
    public ITcpServer<T> TcpServer { set; get; }
    
    /// <summary>
    /// IP端点信息
    /// </summary>
    public IPEndPoint IpEndPoint { set; get; }

    /// <summary>
    /// 连接状态
    /// </summary>
    public bool? Connected { get; }
    
    /// <summary>
    /// 启动客户端
    /// </summary>
    public void Start();
    
    /// <summary>
    /// 停止客户端
    /// </summary>
    public void Stop();

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="netDataPackage">网络数据包</param>
    /// <returns>发送是否成功</returns>
    public bool SendMessage(T netDataPackage);

    /// <summary>
    /// 数据接收事件
    /// </summary>
    event EventHandler<T> DataReceived;
}
