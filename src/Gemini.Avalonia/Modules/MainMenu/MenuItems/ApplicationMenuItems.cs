using System.ComponentModel.Composition;
using Gemini.Avalonia.Framework.Menus;
using Gemini.Avalonia.Modules.MainMenu.Commands;

namespace Gemini.Avalonia.Modules.MainMenu.MenuItems
{
    /// <summary>
    /// 应用程序菜单项定义
    /// </summary>
    [Export(typeof(MenuItemDefinition))]
    public class ExitMenuItem : CommandMenuItemDefinition<ExitApplicationCommandDefinition>
    {
        public ExitMenuItem()
            : base(MenuDefinitions.FileExitOpenMenuGroup, 0) // 在退出菜单组中排在第一位
        {
        }
    }
}
