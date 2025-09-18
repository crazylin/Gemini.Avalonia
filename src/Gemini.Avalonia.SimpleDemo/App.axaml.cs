using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Gemini.Avalonia.SimpleDemo.Framework;
using Gemini.Avalonia.Framework.Logging;

namespace Gemini.Avalonia.SimpleDemo
{
    public partial class App : Application
    {
        private SimpleDemoBootstrapper? _bootstrapper;
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override async void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Gemini.Avalonia.Views.ShellView? mainWindow = null;
                
                try
                {
                    // 创建SimpleDemo引导器
                    _bootstrapper = new SimpleDemoBootstrapper();
                    
                    // 初始化并启动应用程序（这会初始化LogManager）
                    _bootstrapper.Initialize();
                    
                    // 现在可以安全使用LogManager
                    LogManager.Info("SimpleDemoApp", "开始启动SimpleDemo应用程序");
                    mainWindow = await _bootstrapper.StartAsync();
                    
                    LogManager.Info("SimpleDemoApp", "SimpleDemo应用程序初始化和启动完成");
                }
                catch (Exception ex)
                {
                    // 如果LogManager未初始化，使用Console作为备用
                    try
                    {
                        LogManager.Error("SimpleDemoApp", $"SimpleDemo应用程序启动失败: {ex.Message}");
                    }
                    catch
                    {
                        Console.WriteLine($"SimpleDemo应用程序启动失败: {ex.Message}");
                    }
                    throw;
                }
                
                desktop.MainWindow = mainWindow;
                
                if (mainWindow != null)
                {
                    // 在UI线程上确保窗口显示
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        mainWindow.Show();
                        mainWindow.Activate();
                        
                        // 尝试将窗口置于前台
                        mainWindow.Topmost = true;
                        mainWindow.Topmost = false;
                    });
                }
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
