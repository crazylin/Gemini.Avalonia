using System.ComponentModel.Composition;
using AuroraUI.Framework.Menus;
using AuroraUI.Modules.MainMenu;

namespace AuroraUI.Modules.WindowManagement
{
    /// <summary>
    /// 窗口管理菜单定义
    /// </summary>
    public static class MenuDefinitions
    {
        /// <summary>
        /// 窗口管理菜单组 - 供其他模块的窗口菜单项使用
        /// </summary>
        [Export]
        public static readonly MenuItemGroupDefinition WindowManagementMenuGroup = new MenuItemGroupDefinition(AuroraUI.Modules.MainMenu.MenuDefinitions.WindowMenu, 0);
    }
}