using Gemini.Avalonia.Framework.ToolBars;
using System.ComponentModel.Composition;


namespace Gemini.Avalonia.Modules.ToolBars
{
    internal static class ToolBarDefinitions
    {
        [Export(typeof(ToolBarDefinition))]
        public static ToolBarDefinition StandardToolBar = new ToolBarDefinition(0, "标准工具栏按钮");
    }
}