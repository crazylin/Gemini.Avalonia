namespace AuroraUI.IO.Net.TCP;

/// <summary>
/// 网络数据包接口
/// </summary>
public interface INetDataPackage
{
    /// <summary>
    /// 原始数据
    /// </summary>
    public byte[] RawData { set; get; }

    /// <summary>
    /// 从二进制读取器中读取数据
    /// </summary>
    /// <param name="reader">二进制读取器</param>
    public void Read(BinaryReader reader)
    {
    }

    /// <summary>
    /// 获取数据包的字节数组
    /// </summary>
    /// <returns>字节数组</returns>
    public byte[] Get()
    {
        return Array.Empty<byte>();
    }
}
