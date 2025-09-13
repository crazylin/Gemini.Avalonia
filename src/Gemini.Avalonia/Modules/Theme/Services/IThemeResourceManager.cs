using Gemini.Avalonia.Modules.Theme.Models;

namespace Gemini.Avalonia.Modules.Theme.Services
{
    /// <summary>
    /// 主题资源管理器接口
    /// </summary>
    public interface IThemeResourceManager
    {
        /// <summary>
        /// 加载主题资源
        /// </summary>
        /// <param name="themeType">主题类型</param>
        void LoadThemeResources(ThemeType themeType);
        
        /// <summary>
        /// 移除主题资源
        /// </summary>
        /// <param name="themeType">主题类型</param>
        void RemoveThemeResources(ThemeType themeType);
    }
}