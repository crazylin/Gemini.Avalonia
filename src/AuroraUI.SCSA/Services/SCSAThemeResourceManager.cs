using System;
using System.ComponentModel.Composition;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using AuroraUI.Framework;
using AuroraUI.Framework.Logging;
using AuroraUI.Modules.Theme.Models;
using AuroraUI.Modules.Theme.Services;

namespace SCSA.Services
{
    /// <summary>
    /// SCSA 主题资源管理器 - 扩展框架主题系统
    /// 监听框架主题服务的变化，同步加载 SCSA 特定主题
    /// </summary>
    [Export]
    public class SCSAThemeResourceManager : IThemeResourceManager
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        private ResourceDictionary? _currentSCSAThemeResources;
        private IThemeService? _themeService;
        private bool _isInitialized = false;
        
        /// <summary>
        /// 初始化 SCSA 主题管理器，订阅框架主题服务
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;
            
            try
            {
                Logger.Info("初始化 SCSA 主题资源管理器");
                
                // 获取框架主题服务
                _themeService = IoC.Get<IThemeService>();
                if (_themeService != null)
                {
                    // 订阅主题变化事件
                    _themeService.ThemeChanged += OnThemeChanged;
                    
                    // 加载当前主题
                    var currentTheme = _themeService.GetActualTheme();
                    LoadThemeResources(currentTheme);
                    
                    Logger.Info("SCSA 主题资源管理器初始化完成，当前主题: {0}", currentTheme);
                }
                else
                {
                    Logger.Warning("无法获取框架主题服务");
                }
                
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "初始化 SCSA 主题资源管理器失败");
            }
        }
        
        /// <summary>
        /// 处理框架主题变化事件
        /// </summary>
        private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            try
            {
                Logger.Info("框架主题已切换: {0} -> {1}，同步加载 SCSA 主题", e.OldTheme, e.NewTheme);
                
                // 获取实际主题（处理 System 主题）
                var actualTheme = _themeService?.GetActualTheme() ?? e.NewTheme;
                LoadThemeResources(actualTheme);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "同步 SCSA 主题失败");
            }
        }
        
        /// <summary>
        /// 加载 SCSA 特定的主题资源
        /// </summary>
        /// <param name="themeType">主题类型</param>
        public void LoadThemeResources(ThemeType themeType)
        {
            try
            {
                Logger.Info("加载 SCSA 主题资源: {0}", themeType);
                
                // 移除当前 SCSA 主题资源
                RemoveSCSAThemeResources();
                
                // 加载新的 SCSA 主题资源
                var resourceUri = GetSCSAThemeResourceUri(themeType);
                if (resourceUri != null)
                {
                    _currentSCSAThemeResources = AvaloniaXamlLoader.Load(resourceUri) as ResourceDictionary;
                    
                    if (_currentSCSAThemeResources != null && Application.Current != null)
                    {
                        Application.Current.Resources.MergedDictionaries.Add(_currentSCSAThemeResources);
                        Logger.Info("SCSA 主题资源加载成功: {0}", themeType);
                    }
                    else
                    {
                        Logger.Warning("SCSA 主题资源加载失败: {0}", themeType);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "加载 SCSA 主题资源时发生错误: {0}", themeType);
                throw;
            }
        }
        
        /// <summary>
        /// 移除 SCSA 主题资源
        /// </summary>
        /// <param name="themeType">主题类型</param>
        public void RemoveThemeResources(ThemeType themeType)
        {
            try
            {
                RemoveSCSAThemeResources();
                Logger.Info("SCSA 主题资源已移除: {0}", themeType);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "移除 SCSA 主题资源失败: {ThemeType}", themeType);
                throw;
            }
        }
        
        /// <summary>
        /// 移除当前 SCSA 主题资源
        /// </summary>
        private void RemoveSCSAThemeResources()
        {
            if (_currentSCSAThemeResources != null && Application.Current != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(_currentSCSAThemeResources);
                _currentSCSAThemeResources = null;
                Logger.Debug("已移除当前 SCSA 主题资源");
            }
        }
        
        /// <summary>
        /// 获取 SCSA 主题资源 URI
        /// </summary>
        /// <param name="themeType">主题类型</param>
        /// <returns>资源 URI</returns>
        private Uri? GetSCSAThemeResourceUri(ThemeType themeType)
        {
            return themeType switch
            {
                ThemeType.Light => new Uri("avares://AuroraUI.SCSA/Themes/Light/Theme.axaml"),
                ThemeType.Dark => new Uri("avares://AuroraUI.SCSA/Themes/Dark/Theme.axaml"),
                _ => null
            };
        }
        
        /// <summary>
        /// 清理资源和事件订阅
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (_themeService != null)
                {
                    _themeService.ThemeChanged -= OnThemeChanged;
                    _themeService = null;
                }
                
                RemoveSCSAThemeResources();
                _isInitialized = false;
                
                Logger.Info("SCSA 主题资源管理器已清理");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "清理 SCSA 主题资源管理器失败");
            }
        }
    }
}
