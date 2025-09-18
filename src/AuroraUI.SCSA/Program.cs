using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using System.ComponentModel.Composition.Hosting;
using AuroraUI.Framework;
using AuroraUI.Framework.Services;
using AuroraUI.Services;
using AuroraUI.Views;

namespace SCSA;

/// <summary>
/// SCSA应用程序入口点
/// </summary>
class Program
{
    // 应用程序入口点
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia配置，也由设计器使用
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
