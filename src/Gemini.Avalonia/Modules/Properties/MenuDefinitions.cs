using System.ComponentModel.Composition;
using Gemini.Avalonia.Framework.Menus;
using Gemini.Avalonia.Modules.WindowManagement.Commands;

namespace Gemini.Avalonia.Modules.Properties
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
            Gemini.Avalonia.Modules.WindowManagement.MenuDefinitions.WindowManagementMenuGroup, 2);
    }
}
