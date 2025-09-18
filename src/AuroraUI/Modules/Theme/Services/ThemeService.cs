using System;
using System.ComponentModel.Composition;
using Avalonia;
using Avalonia.Styling;
using AuroraUI.Modules.Theme.Models;
using AuroraUI.Framework.Logging;

namespace AuroraUI.Modules.Theme.Services
{
    /// <summary>
    /// 主题服务实现
    /// </summary>
    [Export(typeof(IThemeService))]
    public class ThemeService : IThemeService
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        private ThemeType _currentTheme = ThemeType.System;
        
        [Import]
        private IThemeResourceManager? _themeResourceManager;
        
        public ThemeType CurrentTheme => _currentTheme;
        
        public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
        
        public void Initialize()
        {
            Logger.Info("初始化主题服务");
            
            // 设置默认主题
            ChangeTheme(ThemeType.System);
            
            Logger.Info("主题服务初始化完成，当前主题: {0}", _currentTheme);
        }
        
        public void ChangeTheme(ThemeType themeType)
        {
            var oldTheme = _currentTheme;
            _currentTheme = themeType;
            
            Logger.Info("切换主题: {0} -> {1}", oldTheme, themeType);
            
            try
            {
                ApplyTheme(GetActualTheme());
                ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(oldTheme, themeType));
                Logger.Info("主题切换成功");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "切换主题时发生错误: {0}", themeType);
                _currentTheme = oldTheme; // 回滚
                throw;
            }
        }
        
        public ThemeType GetActualTheme()
        {
            if (_currentTheme == ThemeType.System)
            {
                // 检测系统主题设置
                // 这里可以根据系统API或Avalonia的主题检测来实现
                // 暂时默认返回浅色主题
                return ThemeType.Light;
            }
            
            return _currentTheme;
        }
        
        private void ApplyTheme(ThemeType actualTheme)
        {
            if (Application.Current == null)
            {
                Logger.Warning("Application.Current为null，无法应用主题");
                return;
            }
            
            // 加载主题资源
            _themeResourceManager?.LoadThemeResources(actualTheme);
            
            var themeVariant = actualTheme switch
            {
                ThemeType.Light => ThemeVariant.Light,
                ThemeType.Dark => ThemeVariant.Dark,
                _ => ThemeVariant.Default
            };
            
            Application.Current.RequestedThemeVariant = themeVariant;
            Logger.Debug("应用主题变体: {ThemeVariant}", themeVariant);
        }
    }
}