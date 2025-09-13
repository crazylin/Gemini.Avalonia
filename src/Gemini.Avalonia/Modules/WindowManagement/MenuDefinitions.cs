using System.ComponentModel.Composition;
using Gemini.Avalonia.Framework.Menus;
using Gemini.Avalonia.Modules.MainMenu;
using Gemini.Avalonia.Modules.WindowManagement.Commands;

namespace Gemini.Avalonia.Modules.WindowManagement
{
    /// <summary>
    /// 窗口管理菜单定义
    /// </summary>
    public static class MenuDefinitions
    {
        /// <summary>
        /// 窗口管理菜单组
        /// </summary>
        [Export]
        public static readonly MenuItemGroupDefinition WindowManagementMenuGroup = new MenuItemGroupDefinition(Gemini.Avalonia.Modules.MainMenu.MenuDefinitions.WindowMenu, 0);
        
        /// <summary>
        /// 显示项目管理器菜单项
        /// </summary>
        [Export]
        public static readonly MenuItemDefinition ShowProjectExplorerMenuItem = new CommandMenuItemDefinition<ShowProjectExplorerCommandDefinition>(
            WindowManagementMenuGroup, 0);
            
        /// <summary>
        /// 显示输出窗口菜单项
        /// </summary>
        [Export]
        public static readonly MenuItemDefinition ShowOutputMenuItem = new CommandMenuItemDefinition<ShowOutputCommandDefinition>(
            WindowManagementMenuGroup, 1);
            
        /// <summary>
        /// 显示属性窗口菜单项
        /// </summary>
        [Export]
        public static readonly MenuItemDefinition ShowPropertiesMenuItem = new CommandMenuItemDefinition<ShowPropertiesCommandDefinition>(
            WindowManagementMenuGroup, 2);
    }
}