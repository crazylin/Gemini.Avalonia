using System.Threading.Tasks;

namespace AuroraUI.Services
{
    public interface IConfigurationService
    {
        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <typeparam name="T">配置值类型</typeparam>
        /// <param name="key">配置键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>配置值</returns>
        T GetValue<T>(string key, T defaultValue = default);

        /// <summary>
        /// 设置配置值
        /// </summary>
        /// <typeparam name="T">配置值类型</typeparam>
        /// <param name="key">配置键</param>
        /// <param name="value">配置值</param>
        void SetValue<T>(string key, T value);

        /// <summary>
        /// 保存配置到文件
        /// </summary>
        /// <returns></returns>
        Task SaveAsync();

        /// <summary>
        /// 从文件加载配置
        /// </summary>
        /// <returns></returns>
        Task LoadAsync();
    }
}