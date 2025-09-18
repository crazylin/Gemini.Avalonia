using System.ComponentModel.Composition;
using AuroraUI.Framework.ToolBars;

namespace SCSA.Commands;

/// <summary>
/// SCSA工具栏定义
/// </summary>
public static class SCSAToolBarDefinitions
{
    /// <summary>
    /// SCSA工具栏
    /// </summary>
    [Export]
    public static ToolBarDefinition SCSAToolBar = new ToolBarDefinition(100, "SCSA");

    /// <summary>
    /// 设备管理组
    /// </summary>
    [Export]
    public static ToolBarItemGroupDefinition DeviceGroup = new ToolBarItemGroupDefinition(SCSAToolBar, 0);
}

/// <summary>
/// 设备连接工具栏按钮定义
/// </summary>
[Export(typeof(ToolBarItemDefinition))]
public class DeviceConnectionToolBarItemDefinition : CommandToolBarItemDefinition<ShowDeviceConnectionCommandDefinition>
{
    public DeviceConnectionToolBarItemDefinition() : base(SCSAToolBarDefinitions.DeviceGroup, 0, ToolBarItemType.Button, ToolBarItemDisplay.IconOnly)
    {
    }
}
