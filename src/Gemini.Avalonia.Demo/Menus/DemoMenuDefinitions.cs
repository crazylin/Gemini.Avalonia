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
        // 使用核心框架的文件和编辑菜单，添加新的菜单项组
        [Export]
        public static MenuItemGroupDefinition FileSaveGroup = new MenuItemGroupDefinition(Gemini.Avalonia.Modules.MainMenu.MenuDefinitions.FileMenu, 100);

        [Export]
        public static MenuItemGroupDefinition EditUndoRedoGroup = new MenuItemGroupDefinition(Gemini.Avalonia.Modules.MainMenu.MenuDefinitions.EditMenu, 100);
        
        [Export]
        public static MenuItemGroupDefinition EditClipboardGroup = new MenuItemGroupDefinition(Gemini.Avalonia.Modules.MainMenu.MenuDefinitions.EditMenu, 200);
        
        [Export]
        public static MenuItemGroupDefinition EditSelectGroup = new MenuItemGroupDefinition(Gemini.Avalonia.Modules.MainMenu.MenuDefinitions.EditMenu, 300);
        
        [Export]
        public static MenuItemGroupDefinition EditFindGroup = new MenuItemGroupDefinition(Gemini.Avalonia.Modules.MainMenu.MenuDefinitions.EditMenu, 400);
    }
    

 


    #region 文件菜单项

    [Export(typeof(MenuItemDefinition))]
    public class SaveMenuItem : CommandMenuItemDefinition<SaveDocumentCommandDefinition>
    {
        public SaveMenuItem()
            : base(MenuDefinitions.FileSaveGroup, 0)
        {
        }
    }

    [Export(typeof(MenuItemDefinition))]
    public class SaveAsMenuItem : CommandMenuItemDefinition<SaveAsDocumentCommandDefinition>
    {
        public SaveAsMenuItem()
            : base(MenuDefinitions.FileSaveGroup, 1)
        {
        }
    }

    #endregion

    #region 编辑菜单项

    // 撤销/重做组
    [Export(typeof(MenuItemDefinition))]
    public class UndoMenuItem : CommandMenuItemDefinition<UndoCommandDefinition>
    {
        public UndoMenuItem()
            : base(MenuDefinitions.EditUndoRedoGroup, 0)
        {
        }
    }

    [Export(typeof(MenuItemDefinition))]
    public class RedoMenuItem : CommandMenuItemDefinition<RedoCommandDefinition>
    {
        public RedoMenuItem()
            : base(MenuDefinitions.EditUndoRedoGroup, 1)
        {
        }
    }

    // 剪贴板组
    [Export(typeof(MenuItemDefinition))]
    public class CutMenuItem : CommandMenuItemDefinition<CutCommandDefinition>
    {
        public CutMenuItem()
            : base(MenuDefinitions.EditClipboardGroup, 0)
        {
        }
    }

    [Export(typeof(MenuItemDefinition))]
    public class CopyMenuItem : CommandMenuItemDefinition<CopyCommandDefinition>
    {
        public CopyMenuItem()
            : base(MenuDefinitions.EditClipboardGroup, 1)
        {
        }
    }

    [Export(typeof(MenuItemDefinition))]
    public class PasteMenuItem : CommandMenuItemDefinition<PasteCommandDefinition>
    {
        public PasteMenuItem()
            : base(MenuDefinitions.EditClipboardGroup, 2)
        {
        }
    }

    // 选择组
    [Export(typeof(MenuItemDefinition))]
    public class SelectAllMenuItem : CommandMenuItemDefinition<SelectAllCommandDefinition>
    {
        public SelectAllMenuItem()
            : base(MenuDefinitions.EditSelectGroup, 0)
        {
        }
    }

    // 查找组
    [Export(typeof(MenuItemDefinition))]
    public class FindMenuItem : CommandMenuItemDefinition<FindCommandDefinition>
    {
        public FindMenuItem()
            : base(MenuDefinitions.EditFindGroup, 0)
        {
        }
    }

    [Export(typeof(MenuItemDefinition))]
    public class ReplaceMenuItem : CommandMenuItemDefinition<ReplaceCommandDefinition>
    {
        public ReplaceMenuItem()
            : base(MenuDefinitions.EditFindGroup, 1)
        {
        }
    }

    [Export(typeof(MenuItemDefinition))]
    public class GoToLineMenuItem : CommandMenuItemDefinition<GoToLineCommandDefinition>
    {
        public GoToLineMenuItem()
            : base(MenuDefinitions.EditFindGroup, 2)
        {
        }
    }

    #endregion
}
