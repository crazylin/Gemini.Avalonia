using System;
using System.ComponentModel.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Gemini.Avalonia.Services;
using Gemini.Avalonia.Framework.Logging;
using Gemini.Avalonia.Framework.Services;
using Gemini.Avalonia.Modules.Theme.Services;
using Gemini.Avalonia.Modules.Theme.Models;

namespace Gemini.Avalonia.Modules.Settings.ViewModels
{
    [Export(typeof(ISettingsEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ApplicationSettingsViewModel : ObservableObject, ISettingsEditor
    {
        [ObservableProperty]
        private string _selectedLanguage = "跟随系统";

        [ObservableProperty]
        private string _selectedTheme = "浅色";

        [ObservableProperty]
        private double _fontSize = 12.0;

        [ObservableProperty]
        private string _fontFamily = "Microsoft YaHei UI";

        [ObservableProperty]
        private bool _showStatusBar = true;

        [ObservableProperty]
        private bool _showToolbar = true;

        [ObservableProperty]
        private bool _showMenuBar = true;

        public ObservableCollection<string> AvailableLanguages { get; }
        public ObservableCollection<string> AvailableThemes { get; }
        public ObservableCollection<string> AvailableFonts { get; }

        public string SettingsPageName => _localizationService?.GetString("Settings.Page.Application");
        
        public string SettingsPagePath => _localizationService?.GetString("Settings.Page.System");
        
        public int SortOrder => 0; // 最高优先级，排在第一位
        
        // 本地化文本属性
        
        public string LanguageText => _localizationService?.GetString("Settings.Language");
        public string LanguageRestartNoteText => _localizationService?.GetString("Settings.LanguageRestartNote");
        public string ThemeText => _localizationService?.GetString("Settings.Theme");
        public string FontText => _localizationService?.GetString("Settings.Font");
        public string UIElementsText => _localizationService?.GetString("Settings.UIElements");
        public string ShowMenuBarText => _localizationService?.GetString("Settings.ShowMenuBar");
        public string ShowToolbarText => _localizationService?.GetString("Settings.ShowToolbar");
        public string ShowStatusBarText => _localizationService?.GetString("Settings.ShowStatusBar");

        private readonly IConfigurationService _configurationService;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IThemeService _themeService;

        [ImportingConstructor]
        public ApplicationSettingsViewModel(IConfigurationService configurationService, ILocalizationService localizationService, ILanguageService languageService, IThemeService themeService)
        {
            _configurationService = configurationService;
            _localizationService = localizationService;
            _languageService = languageService;
            _themeService = themeService;
            
            AvailableLanguages = new ObservableCollection<string>
            {
                "跟随系统",
                _localizationService.GetString("Language.Chinese"),
                _localizationService.GetString("Language.English")
            };

            AvailableThemes = new ObservableCollection<string>
            {
                "浅色",
                "深色",
                "自动"
            };

            AvailableFonts = new ObservableCollection<string>
            {
                "Microsoft YaHei UI",
                "SimSun",
                "Consolas",
                "Arial",
                "Times New Roman"
            };

            // 语言切换改为重启模式，不再订阅CultureChanged事件

            LoadSettings();
        }

        public void ApplyChanges()
        {
            // 先保存所有设置到配置服务
            SaveSettings();
            
            // 检查语言是否发生变化，如果变化则触发语言切换（不重复保存配置）
            ApplyLanguageSettings();
            
            // 应用主题设置
            ApplyThemeSettings();
        }

        private void LoadSettings()
        {
            SelectedLanguage = _configurationService.GetValue("Application.Language", "跟随系统");
            SelectedTheme = _configurationService.GetValue("Application.Theme", "浅色");
            FontSize = _configurationService.GetValue("Application.FontSize", 12.0);
            FontFamily = _configurationService.GetValue("Application.FontFamily", "Microsoft YaHei UI");
            ShowStatusBar = _configurationService.GetValue("Application.ShowStatusBar", true);
            ShowToolbar = _configurationService.GetValue("Application.ShowToolbar", true);
            ShowMenuBar = _configurationService.GetValue("Application.ShowMenuBar", true);
        }

        private void SaveSettings()
        {
            _configurationService.SetValue("Application.Language", SelectedLanguage);
            _configurationService.SetValue("Application.Theme", SelectedTheme);
            _configurationService.SetValue("Application.FontSize", FontSize);
            _configurationService.SetValue("Application.FontFamily", FontFamily);
            _configurationService.SetValue("Application.ShowStatusBar", ShowStatusBar);
            _configurationService.SetValue("Application.ShowToolbar", ShowToolbar);
            _configurationService.SetValue("Application.ShowMenuBar", ShowMenuBar);
            
            // 异步保存到文件
            _ = _configurationService.SaveAsync();
        }
        
        private void ApplyLanguageSettings()
        {
            // 调用语言服务进行语言切换（重启模式）
            try
            {
                CultureInfo targetCulture;
                switch (SelectedLanguage)
                {
                    case "中文":
                        targetCulture = new CultureInfo("zh-CN");
                        break;
                    case "English":
                        targetCulture = new CultureInfo("en-US");
                        break;
                    default:
                        // 跟随系统或其他情况，使用系统默认语言
                        targetCulture = CultureInfo.CurrentUICulture;
                        break;
                }
                
                // 调用语言服务的ChangeLanguage方法，不重复保存配置（配置已在SaveSettings中保存）
                _languageService.ChangeLanguage(targetCulture, saveConfig: false);
                
                LogManager.Info("ApplicationSettingsViewModel", $"语言切换请求已发送: {SelectedLanguage} -> {targetCulture.Name}");
            }
            catch (Exception ex)
            {
                LogManager.Error("ApplicationSettingsViewModel", $"语言切换失败: {ex.Message}");
            }
        }
        
        private void ApplyThemeSettings()
        {
            try
            {
                ThemeType targetTheme;
                switch (SelectedTheme)
                {
                    case "浅色":
                        targetTheme = ThemeType.Light;
                        break;
                    case "深色":
                        targetTheme = ThemeType.Dark;
                        break;
                    case "自动":
                        targetTheme = ThemeType.System;
                        break;
                    default:
                        targetTheme = ThemeType.System;
                        break;
                }
                
                _themeService?.ChangeTheme(targetTheme);
                LogManager.Info("ApplicationSettingsViewModel", $"主题切换请求已发送: {SelectedTheme} -> {targetTheme}");
            }
            catch (Exception ex)
            {
                LogManager.Error("ApplicationSettingsViewModel", $"主题切换失败: {ex.Message}");
            }
        }
    }
}