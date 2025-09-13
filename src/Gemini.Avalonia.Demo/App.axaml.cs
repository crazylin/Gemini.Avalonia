using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Gemini.Avalonia.Demo.Framework;
using Gemini.Avalonia.Framework.Logging;
using System;
using Avalonia.Threading;
using Avalonia.Controls;


namespace Gemini.Avalonia.Demo
{
    /// <summary>
    /// 演示应用程序主类
    /// </summary>
    public partial class App : Application
    {
        private DemoBootstrapper? _bootstrapper;
        
        /// <summary>
        /// 初始化应用程序
        /// </summary>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        
        /// <summary>
        /// 应用程序框架初始化完成后调用
        /// </summary>
        /// <param name="e">应用程序生命周期事件参数</param>
        public override async void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Gemini.Avalonia.Views.ShellView? mainWindow = null;
                
                try
                {
                    // 创建Demo引导器 - 使用新的按需加载架构
                    _bootstrapper = new DemoBootstrapper();
                    
                    // 初始化并启动应用程序（这会初始化LogManager）
                    _bootstrapper.Initialize();
                    
                    // 现在可以安全使用LogManager
                    LogManager.Info("DemoApp", "开始启动Demo应用程序");
                    mainWindow = await _bootstrapper.StartAsync();
                    
                    LogManager.Info("DemoApp", "Demo应用程序初始化和启动完成");
                }
                catch (Exception ex)
                {
                    // 如果LogManager未初始化，使用Console作为备用
                    try
                    {
                        LogManager.Error("DemoApp", $"Demo应用程序启动失败: {ex.Message}");
                    }
                    catch
                    {
                        Console.WriteLine($"Demo应用程序启动失败: {ex.Message}");
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
                
                // 处理应用程序退出
                desktop.ShutdownRequested += async (sender, args) =>
                {
                    args.Cancel = true; // 取消默认关闭行为
                    
                    try
                    {
                        try
                        {
                            LogManager.Info("DemoApp", "开始关闭Demo应用程序");
                        }
                        catch
                        {
                            Console.WriteLine("开始关闭Demo应用程序");
                        }
                        
                        if (_bootstrapper?.Shell != null)
                        {
                            var canClose = await _bootstrapper.Shell.CloseAsync();
                            if (canClose)
                            {
                                try
                                {
                                    LogManager.Info("DemoApp", "Demo应用程序关闭完成");
                                }
                                catch
                                {
                                    Console.WriteLine("Demo应用程序关闭完成");
                                }
                                desktop.Shutdown();
                            }
                            else
                            {
                                try
                                {
                                    LogManager.Info("DemoApp", "Demo应用程序关闭被取消");
                                }
                                catch
                                {
                                    Console.WriteLine("Demo应用程序关闭被取消");
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                LogManager.Warning("DemoApp", "Shell为空，直接关闭应用程序");
                            }
                            catch
                            {
                                Console.WriteLine("Shell为空，直接关闭应用程序");
                            }
                            desktop.Shutdown();
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            LogManager.Error("DemoApp", $"关闭Demo应用程序时出错: {ex.Message}");
                        }
                        catch
                        {
                            Console.WriteLine($"关闭Demo应用程序时出错: {ex.Message}");
                        }
                        desktop.Shutdown(); // 强制关闭
                    }
                };
            }
            
            base.OnFrameworkInitializationCompleted();
        }
        
        // 注意：Avalonia应用程序的退出处理已在OnFrameworkInitializationCompleted中的ShutdownRequested事件中处理
    }
}