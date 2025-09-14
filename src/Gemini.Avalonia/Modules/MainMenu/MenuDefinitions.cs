using Gemini.Avalonia.Framework.Menus;
using System.ComponentModel.Composition;

namespace Gemini.Avalonia.Modules.MainMenu
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuBarDefinition MainMenuBar = new MenuBarDefinition();

        [Export]
        public static readonly MenuDefinition FileMenu = new MenuDefinition(MainMenuBar, 0, "Menu.File");

        [Export]
        public static MenuItemGroupDefinition FileNewOpenMenuGroup = new MenuItemGroupDefinition(FileMenu, 0);

        [Export]
        public static MenuItemGroupDefinition FileCloseMenuGroup = new MenuItemGroupDefinition(FileMenu, 3);

        [Export]
        public static MenuItemGroupDefinition FileSaveMenuGroup = new MenuItemGroupDefinition(FileMenu, 6);

        [Export]
        public static MenuItemGroupDefinition FileExitOpenMenuGroup = new MenuItemGroupDefinition(FileMenu, 1000); // 最高优先级，确保在最底部

        [Export]
        public static readonly MenuDefinition EditMenu = new MenuDefinition(MainMenuBar, 1, "Menu.Edit");

        [Export]
        public static MenuItemGroupDefinition EditUndoRedoMenuGroup = new MenuItemGroupDefinition(EditMenu, 0);

        [Export]
        public static readonly MenuDefinition ViewMenu = new MenuDefinition(MainMenuBar, 2, "Menu.View");

        [Export]
        public static MenuItemGroupDefinition ViewToolsMenuGroup = new MenuItemGroupDefinition(ViewMenu, 0);

        [Export]
        public static MenuItemGroupDefinition ViewPropertiesMenuGroup = new MenuItemGroupDefinition(ViewMenu, 100);

        [Export]
        public static readonly MenuDefinition ToolsMenu = new MenuDefinition(MainMenuBar, 3, "Menu.Tools");

        [Export]
        public static MenuItemGroupDefinition ToolsOptionsMenuGroup = new MenuItemGroupDefinition(ToolsMenu, 100);

        [Export]
        public static readonly MenuDefinition WindowMenu = new MenuDefinition(MainMenuBar, 4, "Menu.Window");

        [Export]
        public static readonly MenuDefinition HelpMenu = new MenuDefinition(MainMenuBar, 5, "Menu.Help");

        [Export]
        public static MenuItemGroupDefinition HelpAboutMenuGroup = new MenuItemGroupDefinition(HelpMenu, 100);

    }
}
