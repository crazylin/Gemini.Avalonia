using System.ComponentModel.Composition;
using Gemini.Avalonia.Framework.ToolBars;
using Gemini.Avalonia.Modules.WindowManagement.Commands;
using Gemini.Avalonia.Modules.WindowManagement.ToolBars;

namespace Gemini.Avalonia.Modules.ToolBars
{
    /// <summary>
    /// 工具栏项定义
    /// </summary>
    internal static class ToolBarItemDefinitions
    {
        // 标准工具栏组
        [Export(typeof(ToolBarItemGroupDefinition))]
        public static ToolBarItemGroupDefinition StandardToolBarGroup = new ToolBarItemGroupDefinition(ToolBarDefinitions.StandardToolBar, 0);
        
        // 窗口管理工具栏组
        [Export(typeof(ToolBarItemGroupDefinition))]
        public static ToolBarItemGroupDefinition WindowToolBarGroup = new ToolBarItemGroupDefinition(WindowManagementToolBarDefinitions.WindowManagementToolBar, 0);
        
        // 项目管理器按钮
        [Export(typeof(ToolBarItemDefinition))]
        public static CommandToolBarItemDefinition<ShowProjectExplorerCommandDefinition> ShowProjectExplorerToolBarItem = 
            new CommandToolBarItemDefinition<ShowProjectExplorerCommandDefinition>(WindowToolBarGroup, 0, ToolBarItemType.Button, ToolBarItemDisplay.IconAndText);
        
        // 输出窗口按钮
        [Export(typeof(ToolBarItemDefinition))]
        public static CommandToolBarItemDefinition<ShowOutputCommandDefinition> ShowOutputToolBarItem = 
            new CommandToolBarItemDefinition<ShowOutputCommandDefinition>(WindowToolBarGroup, 1, ToolBarItemType.Button, ToolBarItemDisplay.IconAndText);
        
        // 属性窗口按钮
        [Export(typeof(ToolBarItemDefinition))]
        public static CommandToolBarItemDefinition<ShowPropertiesCommandDefinition> ShowPropertiesToolBarItem = 
            new CommandToolBarItemDefinition<ShowPropertiesCommandDefinition>(WindowToolBarGroup, 2, ToolBarItemType.Button, ToolBarItemDisplay.IconAndText);
    }
}