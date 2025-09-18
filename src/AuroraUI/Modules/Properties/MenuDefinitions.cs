using System.ComponentModel.Composition;
using AuroraUI.Framework.Menus;
using AuroraUI.Modules.WindowManagement.Commands;

namespace AuroraUI.Modules.Properties
{
    /// <summary>
    /// 属性模块菜单定义
    /// </summary>
    public static class MenuDefinitions
    {
        /// <summary>
        /// 显示属性窗口菜单项
        /// </summary>
        [Export]
        public static readonly MenuItemDefinition ShowPropertiesMenuItem = new CommandMenuItemDefinition<ShowPropertiesCommandDefinition>(
            AuroraUI.Modules.WindowManagement.MenuDefinitions.WindowManagementMenuGroup, 2);
    }
}
