using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SCSA.ViewModels;

namespace SCSA.Views;

/// <summary>
/// 参数配置工具视图
/// </summary>
public partial class ParameterConfigurationToolView : UserControl
{
    public ParameterConfigurationToolView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

}
