using AuroraUI.Framework.ToolBars;
using System.Collections.ObjectModel;


namespace AuroraUI.Modules.ToolBars
{
    public interface IToolBarBuilder
    {
        void BuildToolBars(IToolBars result);
        void BuildToolBar(ToolBarDefinition toolBarDefinition, IToolBar result);
    }
}