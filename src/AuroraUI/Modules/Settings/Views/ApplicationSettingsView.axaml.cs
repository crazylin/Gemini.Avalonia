using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AuroraUI.Modules.Settings.Views
{
    public partial class ApplicationSettingsView : UserControl
    {
        public ApplicationSettingsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}