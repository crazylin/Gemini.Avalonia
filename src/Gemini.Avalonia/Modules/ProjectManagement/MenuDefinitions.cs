using System.ComponentModel.Composition;
using Gemini.Avalonia.Framework.Menus;
using Gemini.Avalonia.Modules.WindowManagement.Commands;

namespace Gemini.Avalonia.Modules.ProjectManagement
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
            Gemini.Avalonia.Modules.WindowManagement.MenuDefinitions.WindowManagementMenuGroup, 0);
    }
}
