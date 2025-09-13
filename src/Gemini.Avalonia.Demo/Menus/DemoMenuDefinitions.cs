using System;
using System.ComponentModel.Composition;
using Avalonia.Input;
using Gemini.Avalonia.Framework.Commands;
using Gemini.Avalonia.Framework.Menus;
using Gemini.Avalonia.Modules.MainMenu;
using Gemini.Avalonia.Demo.Commands;

namespace Gemini.Avalonia.Demo.Menus
{
    /// <summary>
    /// Demo应用程序的菜单定义
    /// </summary>
    public static class MenuDefinitions
    {
        [Export]
        public static MenuDefinition DemoMenu = new MenuDefinition(Gemini.Avalonia.Modules.MainMenu.MenuDefinitions.MainMenuBar, 600, "Demo");
        
        [Export]
        public static MenuItemGroupDefinition DemoModuleGroup = new MenuItemGroupDefinition(DemoMenu, 0);
        
        [Export]
        public static MenuItemGroupDefinition DemoUtilityGroup = new MenuItemGroupDefinition(DemoMenu, 100);
    }
    
    /// <summary>
    /// Demo菜单项定义
    /// </summary>
    [Export(typeof(MenuItemDefinition))]
    public class LoadSampleDocumentModuleMenuItem : CommandMenuItemDefinition<LoadSampleDocumentModuleCommandDefinition>
    {
        public LoadSampleDocumentModuleMenuItem()
            : base(MenuDefinitions.DemoModuleGroup, 0)
        {
        }
    }
    
    [Export(typeof(MenuItemDefinition))]
    public class LoadAllDemoFeaturesMenuItem : CommandMenuItemDefinition<LoadAllDemoFeaturesCommandDefinition>
    {
        public LoadAllDemoFeaturesMenuItem()
            : base(MenuDefinitions.DemoModuleGroup, 1)
        {
        }
    }
    
    [Export(typeof(MenuItemDefinition))]
    public class DemoMenuSeparator : MenuItemDefinition
    {
        public DemoMenuSeparator()
            : base(MenuDefinitions.DemoUtilityGroup, 0)
        {
        }

        public override string Header => "-";
        public override Uri IconSource => null!;
        public override KeyGesture KeyGesture => null!;
        public override CommandDefinition CommandDefinition => null!;
    }
    
    [Export(typeof(MenuItemDefinition))]
    public class ShowModuleStatusMenuItem : CommandMenuItemDefinition<ShowModuleStatusCommandDefinition>
    {
        public ShowModuleStatusMenuItem()
            : base(MenuDefinitions.DemoUtilityGroup, 1)
        {
        }
    }
}
