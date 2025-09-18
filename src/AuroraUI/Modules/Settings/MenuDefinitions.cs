using System.ComponentModel.Composition;
using AuroraUI.Framework.Menus;
using AuroraUI.Modules.MainMenu;
using AuroraUI.Modules.Settings.Commands;

namespace AuroraUI.Modules.Settings
{
    public static class MenuDefinitions
    {
        [Export]
        public static readonly MenuItemDefinition OpenOptionsMenuItem = new CommandMenuItemDefinition<OpenSettingsCommandDefinition>(
            AuroraUI.Modules.MainMenu.MenuDefinitions.ToolsOptionsMenuGroup, 0);
    }
}