using AuroraUI.Framework.Commands;
using AuroraUI.Framework.ToolBars;
using AuroraUI.Modules.ToolBars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AuroraUI.Modules.ToolBars.Models
{
    public class ToggleButtonToolBarItem: ButtonToolBarItem
    {
        public ToggleButtonToolBarItem(ToolBarItemDefinition toolBarItem, Command command, IToolBar parent) : base(toolBarItem, command, parent)
        {
        }
    }
}
