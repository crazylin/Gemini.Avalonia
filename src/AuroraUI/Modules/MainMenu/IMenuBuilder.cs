using System.Collections.ObjectModel;
using AuroraUI.Framework.Menus;
using AuroraUI.Modules.MainMenu.Models;

namespace AuroraUI.Modules.MainMenu
{
    public interface IMenuBuilder
    {
        void BuildMenuBar(MenuBarDefinition menuBarDefinition, IMenu result);
    }
}
