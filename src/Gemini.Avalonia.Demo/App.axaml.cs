using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Gemini.Avalonia.Demo.Modules;
using Gemini.Avalonia.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Threading;
using Avalonia.Controls;


namespace Gemini.Avalonia.Demo
{
    /// <summary>
    /// 演示应用程序主类
    /// </summary>
    public partial class App : Application
    {
        private AppBootstrapper? _bootstrapper;
        
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
                // 创建并配置应用程序引导器
                _bootstrapper = new AppBootstrapper()
                    .AddModule<Gemini.Avalonia.Modules.MainMenu.Module>() // 主菜单模块
                    .AddModule<Gemini.Avalonia.Modules.ToolBars.Module>() // 工具栏模块
                    .AddModule<Gemini.Avalonia.Modules.Settings.Module>() // 设置模块
                    .AddModule<Gemini.Avalonia.Modules.Output.Module>() // 输出模块
                    .AddModule<Gemini.Avalonia.Modules.Properties.Module>() // 属性模块
                    .AddModule<Gemini.Avalonia.Modules.ProjectManagement.Module>() // 项目管理模块
                    .AddModule<SampleDocumentModule>(); // 示例文档模块
            
  
                
                // 初始化并启动应用程序
                _bootstrapper.Initialize();
                var mainWindow = await _bootstrapper.StartAsync();
                
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
                    
                    if (_bootstrapper?.Shell != null)
                    {
                        var canClose = await _bootstrapper.Shell.CloseAsync();
                        if (canClose)
                        {
                            await _bootstrapper.StopAsync();
                            desktop.Shutdown();
                        }
                    }
                    else
                    {
                        desktop.Shutdown();
                    }
                };
            }
            
            base.OnFrameworkInitializationCompleted();
        }
        
        // 注意：Avalonia应用程序的退出处理已在OnFrameworkInitializationCompleted中的ShutdownRequested事件中处理
    }
}