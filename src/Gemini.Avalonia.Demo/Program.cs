using System;
using Avalonia;
using Avalonia.ReactiveUI;
using Svg.Skia;

namespace Gemini.Avalonia.Demo
{
    /// <summary>
    /// 应用程序入口点
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// 应用程序主入口点
        /// </summary>
        /// <param name="args">命令行参数</param>
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                // 记录启动错误
                Console.WriteLine($"Application startup failed: {ex}");
                throw;
            }
        }
        
        /// <summary>
        /// 构建Avalonia应用程序
        /// </summary>
        /// <returns>应用程序构建器</returns>
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace()
                .UseReactiveUI();
        }
    }
}