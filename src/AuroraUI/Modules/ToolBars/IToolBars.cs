

using AuroraUI.Modules.ToolBars.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AuroraUI.Modules.ToolBars
{
    /// <summary>
    /// 工具栏集合接口
    /// </summary>
    public interface IToolBars
    {
        /// <summary>
        /// 工具栏集合
        /// </summary>
        ObservableCollection<IToolBar> Items { get; }
        
        /// <summary>
        /// 工具栏是否可见
        /// </summary>
        bool Visible { get; set; }
    }
}