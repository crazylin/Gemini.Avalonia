using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using AuroraUI.Framework;
using AuroraUI.Framework.Services;
using AuroraUI.Framework.Logging;

namespace AuroraUI.Services
{
    public class LanguageService : ILanguageService, ILocalizationService
    {
        private CultureInfo _currentCulture;
        private readonly CultureInfo[] _availableLanguages;
        
        // 保持接口完整性，即使在重启模式下也需要声明此事件
        public event EventHandler<CultureInfo> LanguageChanged;
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<CultureInfo>? CultureChanged;
        
        public CultureInfo CurrentCulture 
        {
            get => _currentCulture;
            private set
            {
                if (_currentCulture != value)
                {
                    _currentCulture = value;
                    OnPropertyChanged(nameof(CurrentCulture));
                    // LanguageChanged?.Invoke(this, value); // 已移除动态语言切换事件
                    CultureChanged?.Invoke(this, value);
                }
            }
        }
        
        public LanguageService()
        {
            // 定义支持的语言
            _availableLanguages = new[]
            {
                new CultureInfo("zh-CN"), // 中文优先
                new CultureInfo("en-US")
            };
            
            // 设置默认语言：优先使用当前UI文化信息（这是AppBootstrapper设置的），如果不支持则使用中文
            var systemCulture = CultureInfo.CurrentUICulture;
            var supportedCulture = _availableLanguages.FirstOrDefault(c => 
                c.Name == systemCulture.Name) 
                ?? _availableLanguages.FirstOrDefault(c => 
                    c.TwoLetterISOLanguageName == systemCulture.TwoLetterISOLanguageName) 
                ?? _availableLanguages[0]; // 默认中文
                
            _currentCulture = supportedCulture;
        }
        
        /// <summary>
        /// 内部方法：仅设置当前语言，不触发重启逻辑（用于应用启动时初始化）
        /// </summary>
        internal void SetCurrentCultureInternal(CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));
                
            if (!_availableLanguages.Any(c => c.Name == culture.Name))
                throw new ArgumentException($"Language '{culture.Name}' is not supported.", nameof(culture));

            LogManager.Info("LanguageService", $"设置语言为: {culture.Name} ({culture.DisplayName})");
            CurrentCulture = culture;
        }
        
        public void ChangeLanguage(CultureInfo culture, bool saveConfig = true)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));
                
            if (!_availableLanguages.Contains(culture))
                throw new ArgumentException($"Language '{culture.Name}' is not supported.", nameof(culture));

            // 语言切换改为重启模式：保存设置并提示重启
            try
            {
                LogManager.Info("开始切换语言到: {0}", culture.Name);
                
                if (saveConfig)
                {
                    // 获取配置服务并保存语言设置
                    var configService = IoC.Get<IConfigurationService>();
                    if (configService != null)
                    {
                        LogManager.Debug("获取到配置服务");
                        
                        string languageName;
                        switch (culture.Name)
                        {
                            case "zh-CN":
                                languageName = "中文";
                                break;
                            case "en-US":
                                languageName = "English";
                                break;
                            default:
                                languageName = "跟随系统";
                                break;
                        }
                        
                        LogManager.Info("映射语言名称: {0} -> {1}", culture.Name, languageName);
                        
                        configService.SetValue("Application.Language", languageName);
                        LogManager.Debug("已设置配置值: Application.Language = {0}", languageName);
                        
                        var saveTask = configService.SaveAsync();
                        LogManager.Debug("开始保存配置文件");
                    }
                    else
                    {
                        LogManager.Error("错误: 无法获取配置服务");
                    }
                }
                
                // 只有在需要保存配置时才显示重启提示对话框
                if (saveConfig)
                {
                    LogManager.Debug("显示重启对话框");
                    ShowRestartDialog();
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(ex, "保存语言设置失败");
            }
        }
        
        public void ChangeLanguage(string cultureName)
        {
            if (string.IsNullOrEmpty(cultureName))
                throw new ArgumentException("Culture name cannot be null or empty.", nameof(cultureName));
                
            var culture = new CultureInfo(cultureName);
            ChangeLanguage(culture);
        }
        
        public CultureInfo[] GetAvailableLanguages()
        {
            return _availableLanguages.ToArray();
        }
        
        private async void ShowRestartDialog()
        {
            try
            {
                // 创建确认对话框
                var dialog = new Window
                {
                    Title = "语言设置",
                    Width = 400,
                    Height = 150,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    CanResize = false
                };
                
                var panel = new StackPanel
                {
                    Margin = new Thickness(20),
                    Spacing = 20
                };
                
                var messageText = new TextBlock
                {
                    Text = "语言设置已保存，需要重启应用程序才能生效。是否现在重启？",
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                
                var buttonPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Spacing = 10
                };
                
                var yesButton = new Button
                {
                    Content = "是",
                    Width = 80,
                    Height = 30
                };
                
                var noButton = new Button
                {
                    Content = "否",
                    Width = 80,
                    Height = 30
                };
                
                bool? result = null;
                
                yesButton.Click += (s, e) => 
                {
                    result = true;
                    dialog.Close();
                };
                
                noButton.Click += (s, e) => 
                {
                    result = false;
                    dialog.Close();
                };
                
                buttonPanel.Children.Add(yesButton);
                buttonPanel.Children.Add(noButton);
                
                panel.Children.Add(messageText);
                panel.Children.Add(buttonPanel);
                
                dialog.Content = panel;
                
                // 获取主窗口作为父窗口
                Window? parentWindow = null;
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    parentWindow = desktop.MainWindow;
                }
                
                await dialog.ShowDialog(parentWindow);
                
                if (result == true)
                {
                    RestartApplication();
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(ex, "显示重启对话框失败");
            }
        }
        
        private void RestartApplication()
        {
            try
            {
                var processModule = System.Diagnostics.Process.GetCurrentProcess().MainModule;
                if (processModule?.FileName != null)
                {
                    System.Diagnostics.Process.Start(processModule.FileName);
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(ex, "重启应用程序失败");
            }
        }

        // 移除动态资源字典更新方法，改为重启模式
        // private void UpdateResourceDictionary(CultureInfo culture) { ... }
        
        /// <summary>
        /// 初始化语言服务，加载默认语言资源
        /// </summary>
        public void Initialize()
        {
            // 重启模式下不需要动态更新资源字典
            // UpdateResourceDictionary(CurrentCulture);
        }
        
        // ILocalizationService 接口实现
        public string GetString(string key)
        {
            try
            {
                // 尝试从当前应用程序的资源中获取字符串
                if (Application.Current?.Resources != null)
                {
                    if (Application.Current.Resources.TryGetResource(key, null, out var resource) && resource is string stringValue)
                    {
                       return stringValue;
                    }
                    else
                    {
                        // 如果当前语言找不到，且当前语言不是英文，则尝试获取英文翻译
                        if (CurrentCulture.Name != "en-US")
                        {
                            var englishFallback = TryGetEnglishString(key);
                            if (!string.IsNullOrEmpty(englishFallback))
                            {
                               return englishFallback;
                            }
                        }
     
                    }
                }
                
                // 如果都找不到，返回键名作为默认值
                return key;
            }
            catch (Exception ex)
            {
                LogManager.Error("LanguageService", $"获取本地化字符串时发生异常: {ex.Message}");
                return key;
            }
        }
        
        public string GetString(string key, string defaultValue)
        {
            try
            {
                // 尝试从当前应用程序的资源中获取字符串
                if (Application.Current?.Resources != null)
                {
                    if (Application.Current.Resources.TryGetResource(key, null, out var resource) && resource is string stringValue)
                    {
                         return stringValue;
                    }
                    else
                    {
                        // 如果当前语言找不到，且当前语言不是英文，则尝试获取英文翻译
                        if (CurrentCulture.Name != "en-US")
                        {
                            // 尝试从英文资源中获取
                            var englishFallback = TryGetEnglishString(key);
                            if (!string.IsNullOrEmpty(englishFallback))
                            {
                                return englishFallback;
                            }
                        }
                        
                   }
                }

                
                return defaultValue;
            }
            catch (Exception ex)
            {
                LogManager.Error("LanguageService", $"获取本地化字符串时发生异常: {ex.Message}");
                return defaultValue;
            }
        }
        
        public void SetCulture(CultureInfo culture)
        {
            ChangeLanguage(culture);
        }
        
        public void SetCulture(string cultureName)
        {
            ChangeLanguage(cultureName);
        }
        
        public CultureInfo GetSystemCulture()
        {
            var systemCulture = CultureInfo.CurrentUICulture;
            
            // 检查系统语言是否在支持列表中
            foreach (var supportedCulture in _availableLanguages)
            {
                if (systemCulture.TwoLetterISOLanguageName == supportedCulture.TwoLetterISOLanguageName)
                {
                    return supportedCulture;
                }
            }
            
            // 如果系统语言不支持，返回英文作为默认语言
            return new CultureInfo("en-US");
        }
        
        public CultureInfo[] GetSupportedCultures()
        {
            return GetAvailableLanguages();
        }
        
        /// <summary>
        /// 尝试获取英文翻译作为回退
        /// </summary>
        /// <param name="key">资源键</param>
        /// <returns>英文翻译字符串，如果找不到则返回null</returns>
        private string? TryGetEnglishString(string key)
        {
            // 暂时简化实现，直接返回null
            // TODO: 实现英文回退功能
            return null;
        }
        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}