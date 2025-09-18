using System;
using System.Globalization;

namespace AuroraUI.Services
{
    public interface ILanguageService
    {
        /// <summary>
        /// 当前语言文化
        /// </summary>
        CultureInfo CurrentCulture { get; }
        
        /// <summary>
        /// 语言改变事件
        /// </summary>
        event EventHandler<CultureInfo> LanguageChanged;
        
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="culture">目标语言文化</param>
        /// <param name="saveConfig">是否保存配置到文件，默认为true</param>
        void ChangeLanguage(CultureInfo culture, bool saveConfig = true);
        
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="cultureName">语言文化名称，如 "en-US", "zh-CN"</param>
        void ChangeLanguage(string cultureName);
        
        /// <summary>
        /// 获取可用的语言列表
        /// </summary>
        /// <returns>可用语言文化列表</returns>
        CultureInfo[] GetAvailableLanguages();
    }
}