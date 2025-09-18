using Avalonia.Controls;
using Avalonia.Interactivity;
using AuroraUI.Modules.Settings.ViewModels;

namespace AuroraUI.Modules.Settings.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private async void OnOkClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is SettingsViewModel viewModel)
            {
                await viewModel.SaveChangesCommand.ExecuteAsync(null);
            }
            Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}