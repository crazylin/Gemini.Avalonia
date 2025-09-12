using System.ComponentModel.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using Gemini.Avalonia.Modules.Settings;

namespace Gemini.Avalonia.Demo.ViewModels
{
    [Export(typeof(ISettingsEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class GeneralSettingsViewModel : ObservableObject, ISettingsEditor
    {
        [ObservableProperty]
        private bool _confirmExit = true;

        [ObservableProperty]
        private bool _autoSave = false;

        [ObservableProperty]
        private int _autoSaveInterval = 5;

        [ObservableProperty]
        private bool _showWelcomeScreen = true;

        [ObservableProperty]
        private bool _enableDebugMode = false;

        [ObservableProperty]
        private int _maxRecentFiles = 10;

        public string SettingsPageName => "常规";

        public string SettingsPagePath => "演示应用";
        
        public int SortOrder => 100; // 较低优先级

        public GeneralSettingsViewModel()
        {
            // 从配置文件或其他地方加载设置
            LoadSettings();
        }

        public void ApplyChanges()
        {
            // 保存设置到配置文件或其他地方
            SaveSettings();
        }

        private void LoadSettings()
        {
            // 这里可以从配置文件加载设置
            // 暂时使用默认值
        }

        private void SaveSettings()
        {
            // 这里可以保存设置到配置文件
            // 暂时只是占位符
        }
    }
}