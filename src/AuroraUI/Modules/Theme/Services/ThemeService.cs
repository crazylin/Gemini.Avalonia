using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Styling;
using AuroraUI.Modules.Theme.Models;
using AuroraUI.Framework.Logging;
using AuroraUI.Modules.Theme.Services;
using AuroraUI.Services;

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
        
        [Import]
        private IThemeManager? _themeManager;
        
        [Import]
        private IConfigurationService? _configurationService;
        
        public ThemeType CurrentTheme => _currentTheme;
        
        public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
        
        public void Initialize()
        {
            Logger.Info("初始化主题服务");
            
            // 异步初始化，但不阻塞主线程
            _ = InitializeAsync();
        }
        
        private async Task InitializeAsync()
        {
            // 从配置文件读取保存的主题设置
            ThemeType savedTheme = ThemeType.System; // 默认值
            
            if (_configurationService != null)
            {
                try
                {
                    // 确保配置文件已加载
                    await _configurationService.LoadAsync();
                    
                    var savedThemeString = _configurationService.GetValue("Application.Theme", "System");
                    if (Enum.TryParse<ThemeType>(savedThemeString, out var parsedTheme))
                    {
                        savedTheme = parsedTheme;
                        Logger.Info("从配置文件加载主题设置: {0}", savedTheme);
                    }
                    else
                    {
                        Logger.Warning("配置文件中的主题设置无效: {0}，使用默认主题", savedThemeString);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "读取主题配置时发生错误，使用默认主题");
                }
            }
            else
            {
                Logger.Warning("配置服务不可用，使用默认主题");
            }
            
            // 应用读取到的主题
            ChangeTheme(savedTheme);
            
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
            
            // 确定基础主题变体
            var themeVariant = GetBaseThemeVariant(actualTheme);
            
            Application.Current.RequestedThemeVariant = themeVariant;
            Logger.Debug("应用主题: {0}, 基础变体: {1}", actualTheme, themeVariant);
        }
        
        private ThemeVariant GetBaseThemeVariant(ThemeType themeType)
        {
            // 对于基础主题，直接映射
            switch (themeType)
            {
                case ThemeType.Light:
                    return ThemeVariant.Light;
                case ThemeType.Dark:
                    return ThemeVariant.Dark;
            }
            
            // 对于扩展主题，根据其是否为深色主题来确定基础变体
            if (_themeManager != null)
            {
                var themeInfo = _themeManager.GetThemeInfo(themeType);
                if (themeInfo != null)
                {
                    Logger.Debug("扩展主题 {0} 是否为深色: {1}", themeType, themeInfo.IsDark);
                    return themeInfo.IsDark ? ThemeVariant.Dark : ThemeVariant.Light;
                }
            }
            
            // 默认使用浅色主题
            Logger.Debug("未知主题 {0}，使用默认浅色变体", themeType);
            return ThemeVariant.Light;
        }
    }
}