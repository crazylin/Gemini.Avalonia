using AuroraUI.Framework.ToolBars;
using System.ComponentModel.Composition;


namespace AuroraUI.Modules.ToolBars
{
    internal static class ToolBarDefinitions
    {
        [Export(typeof(ToolBarDefinition))]
        public static ToolBarDefinition StandardToolBar = new ToolBarDefinition(0, "标准工具栏按钮");
    }
}