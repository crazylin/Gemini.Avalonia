using System.ComponentModel.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using Gemini.Avalonia.Modules.Settings;
using System.Collections.ObjectModel;

namespace Gemini.Avalonia.Demo.ViewModels
{
    [Export(typeof(ISettingsEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class EditorSettingsViewModel : ObservableObject, ISettingsEditor
    {
        [ObservableProperty]
        private bool _showLineNumbers = true;

        [ObservableProperty]
        private bool _wordWrap = false;

        [ObservableProperty]
        private bool _showWhitespace = false;

        [ObservableProperty]
        private bool _highlightCurrentLine = true;

        [ObservableProperty]
        private int _tabSize = 4;

        [ObservableProperty]
        private bool _insertSpaces = true;

        [ObservableProperty]
        private string _editorFontFamily = "Consolas";

        [ObservableProperty]
        private double _editorFontSize = 12.0;

        [ObservableProperty]
        private bool _autoIndent = true;

        [ObservableProperty]
        private bool _bracketMatching = true;

        public ObservableCollection<string> AvailableEditorFonts { get; }

        public string SettingsPageName => "编辑器";

        public string SettingsPagePath => "编辑器";
        
        public int SortOrder => 200;

        public EditorSettingsViewModel()
        {
            AvailableEditorFonts = new ObservableCollection<string>
            {
                "Consolas",
                "Courier New",
                "Monaco",
                "Fira Code",
                "Source Code Pro",
                "JetBrains Mono"
            };

            LoadSettings();
        }

        public void ApplyChanges()
        {
            SaveSettings();
        }

        private void LoadSettings()
        {
            // 从配置文件加载编辑器设置
        }

        private void SaveSettings()
        {
            // 保存编辑器设置到配置文件
        }
    }
}