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
        private string _selectedLanguage = "Follow System";

        [ObservableProperty]
        private string _selectedTheme = "Light";


        public ObservableCollection<string> AvailableLanguages { get; }
        public ObservableCollection<string> AvailableThemes { get; }

        public string SettingsPageName => _localizationService?.GetString("Settings.Page.Application");
        
        public string SettingsPagePath => _localizationService?.GetString("Settings.Page.System");
        
        public int SortOrder => 0; // 最高优先级，排在第一位
        
        // 本地化文本属性
        
        public string LanguageText => _localizationService?.GetString("Settings.Language");
        public string LanguageRestartNoteText => _localizationService?.GetString("Settings.LanguageRestartNote");
        public string ThemeText => _localizationService?.GetString("Settings.Theme");

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
                "Follow System",
                _localizationService.GetString("Language.Chinese"),
                _localizationService.GetString("Language.English")
            };

            AvailableThemes = new ObservableCollection<string>
            {
                "Light",
                "Dark",
                "Auto"
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
            SelectedLanguage = _configurationService.GetValue("Application.Language", "Follow System");
            SelectedTheme = _configurationService.GetValue("Application.Theme", "Light");
        }

        private void SaveSettings()
        {
            _configurationService.SetValue("Application.Language", SelectedLanguage);
            _configurationService.SetValue("Application.Theme", SelectedTheme);
            
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
                    case "Chinese":
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
                    case "Light":
                        targetTheme = ThemeType.Light;
                        break;
                    case "Dark":
                        targetTheme = ThemeType.Dark;
                        break;
                    case "Auto":
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