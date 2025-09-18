using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AuroraUI.Framework;
using AuroraUI.Framework.Commands;
using AuroraUI.Framework.Logging;
using AuroraUI.Framework.Services;

namespace AuroraUI.Modules.MainMenu.Commands
{
    /// <summary>
    /// 应用程序退出命令定义
    /// </summary>
    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class ExitApplicationCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Application.Exit";
        
        public override string Name => CommandName;
        public override string Text => LocalizationService?.GetString("ExitApplication");
        public override string ToolTip => LocalizationService?.GetString("ExitApplication.ToolTip", "退出应用程序");
        public override Uri IconSource => new Uri("avares://AuroraUI/Assets/Icons/exit.svg");
    }
    
    /// <summary>
    /// 应用程序退出命令处理器
    /// </summary>
    [Export(typeof(ICommandHandler))]
    public class ExitApplicationCommandHandler : CommandHandlerBase<ExitApplicationCommandDefinition>
    {
        public override async Task Run(Command command)
        {
            try
            {
                LogManager.Info("ExitApplicationCommand", "用户请求退出应用程序");
                
                // 获取Shell服务
                var shell = IoC.Get<IShell>();
                if (shell != null)
                {
                    LogManager.Info("ExitApplicationCommand", $"检查 {shell.Documents.Count} 个打开的文档");
                    
                    // 检查所有打开的文档是否可以关闭
                    var documentsToClose = shell.Documents.ToList();
                    foreach (var document in documentsToClose)
                    {
                        LogManager.Info("ExitApplicationCommand", $"尝试关闭文档: {document.DisplayName}");
                        
                        try
                        {
                            // 直接调用IDocument的TryCloseAsync方法处理保存确认等逻辑
                            await document.TryCloseAsync();
                            LogManager.Info("ExitApplicationCommand", $"文档 {document.DisplayName} 已成功关闭");
                        }
                        catch (OperationCanceledException)
                        {
                            // 用户取消了保存操作，停止退出流程
                            LogManager.Info("ExitApplicationCommand", $"用户取消了文档 {document.DisplayName} 的关闭操作，退出流程已取消");
                            return;
                        }
                        catch (Exception docEx)
                        {
                            LogManager.Error("ExitApplicationCommand", $"关闭文档 {document.DisplayName} 时发生错误: {docEx.Message}");
                            // 其他异常也取消退出，确保数据安全
                            LogManager.Info("ExitApplicationCommand", "由于文档关闭错误，退出操作已取消");
                            return;
                        }
                    }
                    
                    // 所有文档都已成功关闭，现在可以安全退出
                    LogManager.Info("ExitApplicationCommand", "所有文档已成功关闭，开始关闭应用程序");
                }
                
                // 获取当前应用程序生命周期
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    // 尝试关闭主窗口，这将触发应用程序的正常关闭流程
                    desktop.Shutdown();
                    LogManager.Info("ExitApplicationCommand", "应用程序正常退出");
                }
                else
                {
                    LogManager.Warning("ExitApplicationCommand", "无法获取桌面应用程序生命周期，使用Environment.Exit");
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("ExitApplicationCommand", $"退出应用程序时发生错误: {ex.Message}");
                // 强制退出
                Environment.Exit(-1);
            }
        }
    }
}
