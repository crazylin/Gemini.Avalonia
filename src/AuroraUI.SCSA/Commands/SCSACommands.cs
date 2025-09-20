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

    public override async Task Run(Command command)
    {
        try
        {
            var logger = AuroraUI.Framework.Logging.LogManager.GetLogger("SCSA.Commands");
            logger.Info("打开设备连接对话框");

            // 创建设备连接对话框
            var connectionManager = AuroraUI.Framework.IoC.Get<Services.IConnectionManager>();
            var deviceManager = AuroraUI.Framework.IoC.Get<Services.IDeviceManager>();
            var viewModel = new ViewModels.DeviceConnectionViewModel(connectionManager, deviceManager);
            var dialog = new Views.DeviceConnectionDialog(viewModel);

            // 使用主窗口作为父窗口显示模态对话框
            if (_shell.MainWindow != null)
            {
                await dialog.ShowDialog(_shell.MainWindow);
                
                // 如果选择了设备，设备管理器已经自动连接
                if (dialog.DialogResult && dialog.SelectedDevice != null)
                {
                    try
                    {
                        // 设备已经通过DeviceManager自动连接，无需额外操作
                        logger.Info($"设备已通过设备管理器连接: {dialog.SelectedDevice.DisplayName}");
                        
                        logger.Info($"设备 {dialog.SelectedDevice.DeviceId} 已设置到参数配置工具");
                    }
                    catch (Exception toolEx)
                    {
                        logger.Error($"设置参数配置工具设备失败: {toolEx.Message}");
                    }
                }
            }
            else
            {
                dialog.Show();
            }
        }
        catch (Exception ex)
        {
            // 记录错误
            AuroraUI.Framework.Logging.LogManager.GetLogger("SCSA.Commands").Error($"显示设备连接对话框失败: {ex.Message}");
        }
    }
}
