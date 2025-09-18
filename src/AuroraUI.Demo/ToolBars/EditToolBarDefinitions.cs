using System.ComponentModel.Composition;
using AuroraUI.Framework.ToolBars;
using AuroraUI.Demo.Commands;

namespace AuroraUI.Demo.ToolBars
{
    /// <summary>
    /// 编辑工具栏定义
    /// </summary>
    public static class EditToolBarDefinitions
    {
        /// <summary>
        /// 编辑工具栏
        /// </summary>
        [Export]
        public static ToolBarDefinition EditToolBar = new ToolBarDefinition(1, "EditToolBar");

        /// <summary>
        /// 撤销重做组
        /// </summary>
        [Export]
        public static ToolBarItemGroupDefinition UndoRedoGroup = new ToolBarItemGroupDefinition(EditToolBar, 0);

        /// <summary>
        /// 剪贴板组
        /// </summary>
        [Export]
        public static ToolBarItemGroupDefinition ClipboardGroup = new ToolBarItemGroupDefinition(EditToolBar, 100);

        /// <summary>
        /// 查找组
        /// </summary>
        [Export]
        public static ToolBarItemGroupDefinition FindGroup = new ToolBarItemGroupDefinition(EditToolBar, 200);

        /// <summary>
        /// 文件操作组
        /// </summary>
        [Export]
        public static ToolBarItemGroupDefinition FileGroup = new ToolBarItemGroupDefinition(EditToolBar, 300);
    }

    #region 工具栏项定义

    // 撤销重做组
    [Export(typeof(ToolBarItemDefinition))]
    public class UndoToolBarItem : CommandToolBarItemDefinition<UndoCommandDefinition>
    {
        public UndoToolBarItem()
            : base(EditToolBarDefinitions.UndoRedoGroup, 0, ToolBarItemType.Button, ToolBarItemDisplay.IconOnly)
        {
        }
    }

    [Export(typeof(ToolBarItemDefinition))]
    public class RedoToolBarItem : CommandToolBarItemDefinition<RedoCommandDefinition>
    {
        public RedoToolBarItem()
            : base(EditToolBarDefinitions.UndoRedoGroup, 1, ToolBarItemType.Button, ToolBarItemDisplay.IconOnly)
        {
        }
    }


    // 剪贴板组
    [Export(typeof(ToolBarItemDefinition))]
    public class CutToolBarItem : CommandToolBarItemDefinition<CutCommandDefinition>
    {
        public CutToolBarItem()
            : base(EditToolBarDefinitions.ClipboardGroup, 0, ToolBarItemType.Button, ToolBarItemDisplay.IconOnly)
        {
        }
    }

    [Export(typeof(ToolBarItemDefinition))]
    public class CopyToolBarItem : CommandToolBarItemDefinition<CopyCommandDefinition>
    {
        public CopyToolBarItem()
            : base(EditToolBarDefinitions.ClipboardGroup, 1, ToolBarItemType.Button, ToolBarItemDisplay.IconOnly)
        {
        }
    }

    [Export(typeof(ToolBarItemDefinition))]
    public class PasteToolBarItem : CommandToolBarItemDefinition<PasteCommandDefinition>
    {
        public PasteToolBarItem()
            : base(EditToolBarDefinitions.ClipboardGroup, 2, ToolBarItemType.Button, ToolBarItemDisplay.IconOnly)
        {
        }
    }


    // 查找组
    [Export(typeof(ToolBarItemDefinition))]
    public class FindToolBarItem : CommandToolBarItemDefinition<FindCommandDefinition>
    {
        public FindToolBarItem()
            : base(EditToolBarDefinitions.FindGroup, 0, ToolBarItemType.Button, ToolBarItemDisplay.IconOnly)
        {
        }
    }

    [Export(typeof(ToolBarItemDefinition))]
    public class ReplaceToolBarItem : CommandToolBarItemDefinition<ReplaceCommandDefinition>
    {
        public ReplaceToolBarItem()
            : base(EditToolBarDefinitions.FindGroup, 1, ToolBarItemType.Button, ToolBarItemDisplay.IconOnly)
        {
        }
    }

    [Export(typeof(ToolBarItemDefinition))]
    public class GoToLineToolBarItem : CommandToolBarItemDefinition<GoToLineCommandDefinition>
    {
        public GoToLineToolBarItem()
            : base(EditToolBarDefinitions.FindGroup, 2, ToolBarItemType.Button, ToolBarItemDisplay.IconOnly)
        {
        }
    }

    // 文件操作组
    [Export(typeof(ToolBarItemDefinition))]
    public class SaveToolBarItem : CommandToolBarItemDefinition<SaveDocumentCommandDefinition>
    {
        public SaveToolBarItem()
            : base(EditToolBarDefinitions.FileGroup, 0, ToolBarItemType.Button, ToolBarItemDisplay.IconOnly)
        {
        }
    }

    [Export(typeof(ToolBarItemDefinition))]
    public class SaveAsToolBarItem : CommandToolBarItemDefinition<SaveAsDocumentCommandDefinition>
    {
        public SaveAsToolBarItem()
            : base(EditToolBarDefinitions.FileGroup, 1, ToolBarItemType.Button, ToolBarItemDisplay.IconOnly)
        {
        }
    }

    #endregion
}
