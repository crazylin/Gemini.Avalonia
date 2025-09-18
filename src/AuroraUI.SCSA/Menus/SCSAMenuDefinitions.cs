using System.ComponentModel.Composition;
using AuroraUI.Framework.Menus;
using AuroraUI.Modules.MainMenu;
using SCSA.Commands;

namespace SCSA.Menus
{
    /// <summary>
    /// SCSA菜单定义
    /// </summary>
    public static class SCSAMenuDefinitions
    {
        /// <summary>
        /// 工具菜单中的SCSA设备组
        /// </summary>
        [Export]
        public static MenuItemGroupDefinition ToolsDeviceGroup = new MenuItemGroupDefinition(MenuDefinitions.ToolsMenu, 50);
    }

    /// <summary>
    /// 设备连接菜单项
    /// </summary>
    [Export(typeof(MenuItemDefinition))]
    public class DeviceConnectionMenuItem : CommandMenuItemDefinition<ShowDeviceConnectionCommandDefinition>
    {
        public DeviceConnectionMenuItem()
            : base(SCSAMenuDefinitions.ToolsDeviceGroup, 0)
        {
        }
    }
}
