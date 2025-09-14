using System.ComponentModel.Composition;
using Gemini.Avalonia.Framework.Menus;
using Gemini.Avalonia.Modules.WindowManagement.Commands;

namespace Gemini.Avalonia.Modules.Output
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
            Gemini.Avalonia.Modules.WindowManagement.MenuDefinitions.WindowManagementMenuGroup, 1);
    }
}
