using Gemini.Avalonia.Framework.Commands;
using Gemini.Avalonia.Framework.ToolBars;
using Gemini.Avalonia.Modules.ToolBars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Gemini.Avalonia.Modules.ToolBars.Models
{
    public class ToggleButtonToolBarItem: ButtonToolBarItem
    {
        public ToggleButtonToolBarItem(ToolBarItemDefinition toolBarItem, Command command, IToolBar parent) : base(toolBarItem, command, parent)
        {
        }
    }
}
