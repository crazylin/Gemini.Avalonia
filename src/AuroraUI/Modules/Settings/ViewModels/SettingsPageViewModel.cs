using System.Collections.Generic;

namespace AuroraUI.Modules.Settings.ViewModels
{
    public class SettingsPageViewModel
    {
        public SettingsPageViewModel()
        {
            Name = string.Empty;
            Children = new List<SettingsPageViewModel>();
            Editors = new List<object>();
        }

        public string Name { get; set; }

        public List<object> Editors { get; } // ISettingsEditor or ISettingsEditorAsync

        public List<SettingsPageViewModel> Children { get; }
    }
}