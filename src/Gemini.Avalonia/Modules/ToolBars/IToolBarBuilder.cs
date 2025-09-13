using Gemini.Avalonia.Framework.ToolBars;
using System.Collections.ObjectModel;


namespace Gemini.Avalonia.Modules.ToolBars
{
    public interface IToolBarBuilder
    {
        void BuildToolBars(IToolBars result);
        void BuildToolBar(ToolBarDefinition toolBarDefinition, IToolBar result);
    }
}