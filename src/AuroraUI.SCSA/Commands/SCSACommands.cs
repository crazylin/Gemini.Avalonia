using System.ComponentModel.Composition;
using AuroraUI.Framework.Commands;
using AuroraUI.Framework.Services;

namespace SCSA.Commands;

/// <summary>
/// 设备连接命令定义
/// </summary>
[Export(typeof(CommandDefinitionBase))]
[CommandDefinition]
public class ShowDeviceConnectionCommandDefinition : CommandDefinition
{
    public const string CommandName = "SCSA.ShowDeviceConnection";

    public override string Name => CommandName;
    public override string Text => "设备连接";
    public override string ToolTip => "打开设备连接管理窗口";
    public override Uri IconSource => new("avares://SCSA/Assets/Icons/device-connection.svg");
}

/// <summary>
/// 设备连接命令处理器
/// </summary>
[Export(typeof(ICommandHandler))]
public class ShowDeviceConnectionCommandHandler : CommandHandlerBase<ShowDeviceConnectionCommandDefinition>
{
    private readonly IShell _shell;

    [ImportingConstructor]
    public ShowDeviceConnectionCommandHandler(IShell shell)
    {
        _shell = shell;
    }

    public override Task Run(Command command)
    {
        try
        {
            // 创建设备连接对话框
            var viewModel = new ViewModels.DeviceConnectionViewModel();
            var dialog = new Views.DeviceConnectionDialog(viewModel);

            // 显示对话框
            dialog.Show();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            // 记录错误
            AuroraUI.Framework.Logging.LogManager.GetLogger("SCSA.Commands").Error($"显示设备连接对话框失败: {ex.Message}");
            return Task.CompletedTask;
        }
    }
}
