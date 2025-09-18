using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AuroraUI.Framework.Logging;

namespace SCSA
{
  
    /// <summary>
    /// SCSA应用程序类
    /// </summary>
    public partial class App : Application
    {
        private SCSABootstrapper? _bootstrapper;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override async void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                try
                {
                    // 初始化 AuroraUI 框架
                    _bootstrapper = new SCSABootstrapper();
                    _bootstrapper.Initialize();

                    desktop.MainWindow = await _bootstrapper.StartAsync();;

                    // 注意：DeviceConnectionViewModel 现在通过工具栏按钮来加载，不在这里自动显示
                }
                catch (Exception ex)
                {
                    // 如果初始化失败，显示错误对话框
                    var errorWindow = new ErrorWindow(ex);
                    desktop.MainWindow = errorWindow;
                }
            }

            base.OnFrameworkInitializationCompleted();
        }
    }

    /// <summary>
    /// 错误显示窗口
    /// </summary>
    public class ErrorWindow : Avalonia.Controls.Window
    {
        public ErrorWindow(Exception exception)
        {
            Title = "SCSA 启动错误";
            Width = 600;
            Height = 400;
            WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterScreen;

            Content = new Avalonia.Controls.TextBlock
            {
                Text = $"SCSA应用程序启动时发生错误:\n\n{exception}",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Margin = new Avalonia.Thickness(20)
            };
        }
    }

}