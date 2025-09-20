using System;
using System.ComponentModel.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using AuroraUI.Services;
using AuroraUI.Framework.Logging;
using AuroraUI.Framework.Services;
using AuroraUI.Modules.Theme.Services;
using AuroraUI.Modules.Theme.Models;
using CommunityToolkit.Mvvm.Input;

namespace AuroraUI.Modules.Settings.ViewModels
{
    [Export(typeof(ISettingsEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ApplicationSettingsViewModel : ObservableObject, ISettingsEditor
    {
        [ObservableProperty]
        private string _selectedLanguage = "Follow System";

        [ObservableProperty]
        private ThemeInfo? _selectedTheme;

        [ObservableProperty]
        private ThemeCategoryInfo? _selectedThemeCategory;

        public ObservableCollection<string> AvailableLanguages { get; }
        public ObservableCollection<ThemeInfo> AvailableThemes { get; }
        public ObservableCollection<ThemeCategoryInfo> ThemeCategories { get; }
        public ObservableCollection<ThemeInfo> FilteredThemes { get; }

        public string SettingsPageName => _localizationService?.GetString("Settings.Page.Application") ?? "应用程序设置";
        
        public string SettingsPagePath => _localizationService?.GetString("Settings.Page.System") ?? "系统";
        
        public int SortOrder => 0; // 最高优先级，排在第一位
        
        // 本地化文本属性
        
        public string LanguageText => _localizationService?.GetString("Settings.Language") ?? "语言";
        public string LanguageRestartNoteText => _localizationService?.GetString("Settings.LanguageRestartNote") ?? "更改语言需要重启应用程序";
        public string ThemeText => _localizationService?.GetString("Settings.Theme") ?? "主题";

        private readonly IConfigurationService _configurationService;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IThemeService _themeService;
        private readonly IThemeManager _themeManager;

        [ImportingConstructor]
        public ApplicationSettingsViewModel(IConfigurationService configurationService, ILocalizationService localizationService, ILanguageService languageService, IThemeService themeService, IThemeManager themeManager)
        {
            _configurationService = configurationService;
            _localizationService = localizationService;
            _languageService = languageService;
            _themeService = themeService;
            _themeManager = themeManager;
            
            AvailableLanguages = new ObservableCollection<string>
            {
                "Follow System",
                _localizationService.GetString("Language.Chinese"),
                _localizationService.GetString("Language.English")
            };

            // 初始化主题相关集合
            AvailableThemes = new ObservableCollection<ThemeInfo>(_themeManager.GetAllThemes());
            ThemeCategories = new ObservableCollection<ThemeCategoryInfo>(_themeManager.GetCategories());
            FilteredThemes = new ObservableCollection<ThemeInfo>();

            // 订阅分类选择变化
            PropertyChanged += OnPropertyChanged;

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

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedThemeCategory))
            {
                UpdateFilteredThemes();
            }
        }

        private void UpdateFilteredThemes()
        {
            FilteredThemes.Clear();
            
            if (SelectedThemeCategory != null)
            {
                var themesInCategory = _themeManager.GetThemesByCategory(SelectedThemeCategory.Category);
                foreach (var theme in themesInCategory)
                {
                    FilteredThemes.Add(theme);
                }
            }
            else
            {
                // 如果没有选择分类，显示所有主题
                foreach (var theme in AvailableThemes)
                {
                    FilteredThemes.Add(theme);
                }
            }
        }

        [RelayCommand]
        private void SelectTheme(ThemeInfo theme)
        {
            SelectedTheme = theme;
        }

        private void LoadSettings()
        {
            SelectedLanguage = _configurationService.GetValue("Application.Language", "Follow System");
            
            // 加载主题设置
            var savedThemeType = _configurationService.GetValue("Application.Theme", "Light");
            
            // 尝试解析保存的主题类型
            if (Enum.TryParse<ThemeType>(savedThemeType, out var themeType))
            {
                SelectedTheme = _themeManager.GetThemeInfo(themeType);
            }
            else
            {
                // 如果解析失败，使用默认主题
                SelectedTheme = _themeManager.GetThemeInfo(ThemeType.Light);
            }

            // 设置默认分类为系统主题
            SelectedThemeCategory = ThemeCategories.FirstOrDefault(c => c.Category == ThemeCategory.System);
            UpdateFilteredThemes();
        }

        private void SaveSettings()
        {
            _configurationService.SetValue("Application.Language", SelectedLanguage);
            _configurationService.SetValue("Application.Theme", SelectedTheme?.Type.ToString() ?? "Light");
            
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
                if (SelectedTheme != null)
                {
                    _themeService?.ChangeTheme(SelectedTheme.Type);
                    LogManager.Info("ApplicationSettingsViewModel", $"主题切换请求已发送: {SelectedTheme.Name} -> {SelectedTheme.Type}");
                }
                else
                {
                    // 如果没有选择主题，使用默认的浅色主题
                    _themeService?.ChangeTheme(ThemeType.Light);
                    LogManager.Info("ApplicationSettingsViewModel", "使用默认浅色主题");
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("ApplicationSettingsViewModel", $"主题切换失败: {ex.Message}");
            }
        }
    }
}