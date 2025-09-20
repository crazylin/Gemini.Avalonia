using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SCSA.Models;
using SCSA.ViewModels;

namespace SCSA.Views;

/// <summary>
/// 设备连接对话框
/// </summary>
public partial class DeviceConnectionDialog : Window
{
    /// <summary>
    /// 选中的设备
    /// </summary>
    public EnhancedDeviceConnection? SelectedDevice { get; private set; }

    /// <summary>
    /// 对话框结果
    /// </summary>
    public bool DialogResult { get; private set; }

    public DeviceConnectionDialog()
    {
        InitializeComponent();
    }

    public DeviceConnectionDialog(DeviceConnectionViewModel viewModel) : this()
    {
        DataContext = viewModel;
        
        // 订阅ViewModel事件
        viewModel.DeviceSelected += OnDeviceSelected;
        viewModel.DialogClosed += OnDialogClosed;
        
        // 窗口关闭时清理事件订阅
        Closed += (_, _) =>
        {
            viewModel.DeviceSelected -= OnDeviceSelected;
            viewModel.DialogClosed -= OnDialogClosed;
            viewModel.Dispose();
        };
    }

    private void OnDeviceSelected(object? sender, EnhancedDeviceConnection device)
    {
        SelectedDevice = device;
        DialogResult = true;
    }

    private void OnDialogClosed(object? sender, EventArgs e)
    {
        Close();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
