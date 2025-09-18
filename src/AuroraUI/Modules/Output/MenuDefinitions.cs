using System.ComponentModel.Composition;
using AuroraUI.Framework.Menus;
using AuroraUI.Modules.WindowManagement.Commands;

namespace AuroraUI.Modules.Output
{
    /// <summary>
    /// 输出模块菜单定义
    /// </summary>
    public static class MenuDefinitions
    {
        /// <summary>
        /// 显示输出窗口菜单项
        /// </summary>
        [Export]
        public static readonly MenuItemDefinition ShowOutputMenuItem = new CommandMenuItemDefinition<ShowOutputCommandDefinition>(
            AuroraUI.Modules.WindowManagement.MenuDefinitions.WindowManagementMenuGroup, 1);
    }
}
