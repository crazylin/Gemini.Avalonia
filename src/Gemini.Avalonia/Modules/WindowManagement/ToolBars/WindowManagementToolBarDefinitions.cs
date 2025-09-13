using System.ComponentModel.Composition;
using Gemini.Avalonia.Framework.ToolBars;
using Gemini.Avalonia.Modules.WindowManagement.Commands;

namespace Gemini.Avalonia.Modules.WindowManagement.ToolBars
{
    /// <summary>
    /// 窗口管理工具栏定义
    /// </summary>
    internal static class WindowManagementToolBarDefinitions
    {
        /// <summary>
        /// 窗口管理工具栏
        /// </summary>
        [Export(typeof(ToolBarDefinition))]
        public static ToolBarDefinition WindowManagementToolBar = new ToolBarDefinition(100, "Window Management");
        
        /// <summary>
        /// 窗口管理工具栏组
        /// </summary>
        [Export]
        public static ToolBarItemGroupDefinition WindowManagementToolBarGroup = 
            new ToolBarItemGroupDefinition(WindowManagementToolBar, 0);
        
        /// <summary>
        /// 显示项目管理器工具栏项
        /// </summary>
        [Export]
        public static CommandToolBarItemDefinition<ShowProjectExplorerCommandDefinition> ShowProjectExplorerToolBarItem = 
            new CommandToolBarItemDefinition<ShowProjectExplorerCommandDefinition>(
                WindowManagementToolBarGroup, 1, ToolBarItemType.Button, ToolBarItemDisplay.IconOnly);
        
        /// <summary>
        /// 显示输出窗口工具栏项
        /// </summary>
        [Export]
        public static CommandToolBarItemDefinition<ShowOutputCommandDefinition> ShowOutputToolBarItem = 
            new CommandToolBarItemDefinition<ShowOutputCommandDefinition>(
                WindowManagementToolBarGroup, 2, ToolBarItemType.Button, ToolBarItemDisplay.IconOnly);
        
        /// <summary>
        /// 显示属性窗口工具栏项
        /// </summary>
        [Export]
        public static CommandToolBarItemDefinition<ShowPropertiesCommandDefinition> ShowPropertiesToolBarItem = 
            new CommandToolBarItemDefinition<ShowPropertiesCommandDefinition>(
                WindowManagementToolBarGroup, 3, ToolBarItemType.Button, ToolBarItemDisplay.IconOnly);
    }
}