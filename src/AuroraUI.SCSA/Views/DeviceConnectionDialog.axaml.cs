using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SCSA.ViewModels;

namespace SCSA.Views;

/// <summary>
/// 设备连接对话框
/// </summary>
public partial class DeviceConnectionDialog : Window
{
    public DeviceConnectionDialog()
    {
        InitializeComponent();
    }

    public DeviceConnectionDialog(DeviceConnectionViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
