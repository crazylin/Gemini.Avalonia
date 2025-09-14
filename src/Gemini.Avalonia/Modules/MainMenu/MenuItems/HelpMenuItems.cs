using System.ComponentModel.Composition;
using Gemini.Avalonia.Framework.Menus;
using Gemini.Avalonia.Modules.MainMenu.Commands;

namespace Gemini.Avalonia.Modules.MainMenu.MenuItems
{
    /// <summary>
    /// 关于菜单项定义
    /// </summary>
    [Export(typeof(MenuItemDefinition))]
    public class AboutMenuItem : CommandMenuItemDefinition<AboutApplicationCommandDefinition>
    {
        public AboutMenuItem()
            : base(MenuDefinitions.HelpAboutMenuGroup, 0)
        {
        }
    }

    /// <summary>
    /// 模块列表菜单项定义
    /// </summary>
    [Export(typeof(MenuItemDefinition))]
    public class ModuleListMenuItem : CommandMenuItemDefinition<ModuleListCommandDefinition>
    {
        public ModuleListMenuItem()
            : base(MenuDefinitions.HelpAboutMenuGroup, 1)
        {
        }
    }
}
