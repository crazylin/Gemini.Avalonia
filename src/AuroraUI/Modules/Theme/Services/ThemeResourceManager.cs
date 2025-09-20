using System;
using System.ComponentModel.Composition;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using AuroraUI.Modules.Theme.Models;
using AuroraUI.Framework.Logging;
using AuroraUI.Modules.Theme.Services;

namespace AuroraUI.Modules.Theme.Services
{
    /// <summary>
    /// 主题资源管理器
    /// </summary>
    [Export(typeof(IThemeResourceManager))]
    public class ThemeResourceManager : IThemeResourceManager
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        private ResourceDictionary? _currentThemeResources;
        
        [Import]
        private IThemeManager? _themeManager;
        
        public void LoadThemeResources(ThemeType themeType)
        {
            try
            {
                Logger.Info("加载主题资源: {0}", themeType);
                
                // 移除当前主题资源
                RemoveCurrentThemeResources();
                
                // 加载新主题资源
                var resourceUri = GetThemeResourceUri(themeType);
                if (resourceUri != null)
                {
                    _currentThemeResources = AvaloniaXamlLoader.Load(resourceUri) as ResourceDictionary;
                    
                    if (_currentThemeResources != null && Application.Current != null)
                    {
                        Application.Current.Resources.MergedDictionaries.Add(_currentThemeResources);
                        Logger.Info("主题资源加载成功: {0}", themeType);
                    }
                    else
                    {
                        Logger.Warning("主题资源加载失败: {0}", themeType);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "加载主题资源时发生错误: {0}", themeType);
                throw;
            }
        }
        
        private void RemoveCurrentThemeResources()
        {
            if (_currentThemeResources != null && Application.Current != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(_currentThemeResources);
                _currentThemeResources = null;
                Logger.Debug("已移除当前主题资源");
            }
        }
        
        public void RemoveThemeResources(ThemeType themeType)
        {
            try
            {
                if (_currentThemeResources != null)
                {
                    Application.Current?.Resources.MergedDictionaries.Remove(_currentThemeResources);
                    _currentThemeResources = null;
                    Logger.Info("主题资源已移除: {0}", themeType);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "移除主题资源失败: {ThemeType}", themeType);
                throw;
            }
        }
        
        private Uri? GetThemeResourceUri(ThemeType themeType)
        {
            // 首先检查基础主题
            switch (themeType)
            {
                case ThemeType.Light:
                    return new Uri("avares://AuroraUI/Modules/Theme/Resources/LightTheme.axaml");
                case ThemeType.Dark:
                    return new Uri("avares://AuroraUI/Modules/Theme/Resources/DarkTheme.axaml");
            }
            
            // 检查扩展主题
            if (_themeManager != null)
            {
                var themeInfo = _themeManager.GetThemeInfo(themeType);
                if (themeInfo != null && !string.IsNullOrEmpty(themeInfo.ResourcePath))
                {
                    Logger.Debug("找到扩展主题资源: {0} -> {1}", themeType, themeInfo.ResourcePath);
                    return new Uri(themeInfo.ResourcePath);
                }
            }
            
            Logger.Warning("未找到主题资源: {0}", themeType);
            return null;
        }
    }
}