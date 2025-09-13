using System.Collections.ObjectModel;
using Gemini.Avalonia.Framework.Menus;
using Gemini.Avalonia.Modules.MainMenu.Models;

namespace Gemini.Avalonia.Modules.MainMenu
{
    public interface IMenuBuilder
    {
        void BuildMenuBar(MenuBarDefinition menuBarDefinition, IMenu result);
    }
}
