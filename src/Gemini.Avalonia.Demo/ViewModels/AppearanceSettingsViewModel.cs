using System.ComponentModel.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using Gemini.Avalonia.Modules.Settings;
using System.Collections.ObjectModel;

namespace Gemini.Avalonia.Demo.ViewModels
{
    [Export(typeof(ISettingsEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AppearanceSettingsViewModel : ObservableObject, ISettingsEditor
    {
        [ObservableProperty]
        private string _selectedColorScheme = "默认";

        [ObservableProperty]
        private bool _enableAnimations = true;

        [ObservableProperty]
        private double _windowOpacity = 1.0;

        [ObservableProperty]
        private bool _showGridLines = false;

        [ObservableProperty]
        private string _customCssPath = "";

        public ObservableCollection<string> AvailableColorSchemes { get; }

        public string SettingsPageName => "外观";

        public string SettingsPagePath => "演示应用";
        
        public int SortOrder => 101; // 较低优先级，排在常规设置后面

        public AppearanceSettingsViewModel()
        {
            AvailableColorSchemes = new ObservableCollection<string>
            {
                "默认",
                "蓝色",
                "绿色",
                "紫色",
                "自定义"
            };

            LoadSettings();
        }

        public void ApplyChanges()
        {
            SaveSettings();
        }

        private void LoadSettings()
        {
            // 从配置文件加载设置
        }

        private void SaveSettings()
        {
            // 保存设置到配置文件
        }
    }
}