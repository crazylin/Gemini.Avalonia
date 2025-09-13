using System;
using System.ComponentModel;
using System.Globalization;

namespace Gemini.Avalonia.Framework.Services
{
    /// <summary>
    /// 本地化服务接口，提供多语言支持
    /// </summary>
    public interface ILocalizationService : INotifyPropertyChanged
    {
        /// <summary>
        /// 当前文化信息
        /// </summary>
        CultureInfo CurrentCulture { get; }
        
        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        /// <param name="key">资源键</param>
        /// <returns>本地化字符串</returns>
        string GetString(string key);
        
        /// <summary>
        /// 获取本地化字符串，如果找不到则返回默认值
        /// </summary>
        /// <param name="key">资源键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>本地化字符串</returns>
        string GetString(string key, string defaultValue);
        
        /// <summary>
        /// 设置当前语言
        /// </summary>
        /// <param name="culture">文化信息</param>
        void SetCulture(CultureInfo culture);
        
        /// <summary>
        /// 设置当前语言
        /// </summary>
        /// <param name="cultureName">文化名称，如"zh-CN"、"en-US"</param>
        void SetCulture(string cultureName);
        
        /// <summary>
        /// 获取系统默认语言
        /// </summary>
        /// <returns>系统默认文化信息</returns>
        CultureInfo GetSystemCulture();
        
        /// <summary>
        /// 获取支持的语言列表
        /// </summary>
        /// <returns>支持的文化信息数组</returns>
        CultureInfo[] GetSupportedCultures();
        
        /// <summary>
        /// 语言改变事件
        /// </summary>
        event EventHandler<CultureInfo> CultureChanged;
    }
}