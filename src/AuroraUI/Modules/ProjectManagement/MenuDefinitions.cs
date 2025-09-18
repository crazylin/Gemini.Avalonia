using System.ComponentModel.Composition;
using AuroraUI.Framework.Menus;
using AuroraUI.Modules.WindowManagement.Commands;

namespace AuroraUI.Modules.ProjectManagement
{
    /// <summary>
    /// 项目管理模块菜单定义
    /// </summary>
    public static class MenuDefinitions
    {
        /// <summary>
        /// 显示项目管理器菜单项
        /// </summary>
        [Export]
        public static readonly MenuItemDefinition ShowProjectExplorerMenuItem = new CommandMenuItemDefinition<ShowProjectExplorerCommandDefinition>(
            AuroraUI.Modules.WindowManagement.MenuDefinitions.WindowManagementMenuGroup, 0);
    }
}
