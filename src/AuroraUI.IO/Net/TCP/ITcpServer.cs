using System.Net;

namespace AuroraUI.IO.Net.TCP;

/// <summary>
/// TCP服务器接口
/// </summary>
/// <typeparam name="T">数据包类型</typeparam>
public interface ITcpServer<T> where T : class, new()
{
    /// <summary>
    /// 启动服务器
    /// </summary>
    /// <param name="server">服务器端点</param>
    void Start(IPEndPoint server);
    
    /// <summary>
    /// 停止服务器
    /// </summary>
    void Stop();

    /// <summary>
    /// 向指定客户端发送数据
    /// </summary>
    /// <param name="netDataStream">数据流</param>
    /// <param name="client">客户端端点</param>
    void Send(T netDataStream, IPEndPoint client);

    /// <summary>
    /// 会话关闭事件
    /// </summary>
    public event EventHandler<ITcpClient<T>> SessionClosed;

    /// <summary>
    /// 会话连接事件
    /// </summary>
    public event EventHandler<ITcpClient<T>> SessionConnected;

    /// <summary>
    /// 处理会话关闭
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="client">客户端</param>
    public void OnSessionClosed(object sender, ITcpClient<T> client);
    
    /// <summary>
    /// 处理会话连接
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="client">客户端</param>
    public void OnSessionConnected(object sender, ITcpClient<T> client);
}
