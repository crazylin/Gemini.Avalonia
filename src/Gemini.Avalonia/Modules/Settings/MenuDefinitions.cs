using System.ComponentModel.Composition;
using Gemini.Avalonia.Framework.Menus;
using Gemini.Avalonia.Modules.MainMenu;
using Gemini.Avalonia.Modules.Settings.Commands;

namespace Gemini.Avalonia.Modules.Settings
{
    public static class MenuDefinitions
    {
        [Export]
        public static readonly MenuItemDefinition OpenOptionsMenuItem = new CommandMenuItemDefinition<OpenSettingsCommandDefinition>(
            Gemini.Avalonia.Modules.MainMenu.MenuDefinitions.ToolsOptionsMenuGroup, 0);
    }
}