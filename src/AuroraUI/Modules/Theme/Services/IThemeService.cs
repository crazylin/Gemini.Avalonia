using System;
using AuroraUI.Modules.Theme.Models;

namespace AuroraUI.Modules.Theme.Services
{
    /// <summary>
    /// 主题服务接口
    /// </summary>
    public interface IThemeService
    {
        /// <summary>
        /// 当前主题类型
        /// </summary>
        ThemeType CurrentTheme { get; }
        
        /// <summary>
        /// 主题变更事件
        /// </summary>
        event EventHandler<ThemeChangedEventArgs> ThemeChanged;
        
        /// <summary>
        /// 切换主题
        /// </summary>
        /// <param name="themeType">目标主题类型</param>
        void ChangeTheme(ThemeType themeType);
        
        /// <summary>
        /// 初始化主题系统
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// 获取当前实际使用的主题（解析系统主题）
        /// </summary>
        /// <returns>实际主题类型</returns>
        ThemeType GetActualTheme();
    }
    
    /// <summary>
    /// 主题变更事件参数
    /// </summary>
    public class ThemeChangedEventArgs : EventArgs
    {
        public ThemeType OldTheme { get; }
        public ThemeType NewTheme { get; }
        
        public ThemeChangedEventArgs(ThemeType oldTheme, ThemeType newTheme)
        {
            OldTheme = oldTheme;
            NewTheme = newTheme;
        }
    }
}