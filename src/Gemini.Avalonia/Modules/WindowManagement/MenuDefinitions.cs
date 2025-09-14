using System.ComponentModel.Composition;
using Gemini.Avalonia.Framework.Menus;
using Gemini.Avalonia.Modules.MainMenu;

namespace Gemini.Avalonia.Modules.WindowManagement
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
        public static readonly MenuItemGroupDefinition WindowManagementMenuGroup = new MenuItemGroupDefinition(Gemini.Avalonia.Modules.MainMenu.MenuDefinitions.WindowMenu, 0);
    }
}