using System.Collections.ObjectModel;
using AuroraUI.Modules.ToolBars.Models;

namespace AuroraUI.Modules.ToolBars
{
    public interface IToolBar
    {
        /// <summary>
        /// 工具栏名称
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// 是否可见
        /// </summary>
        bool IsVisible { get; set; }
        
        /// <summary>
        /// 工具栏项集合
        /// </summary>
        ObservableCollection<ToolBarItemBase> Items { get; }
        
        /// <summary>
        /// 添加工具栏项
        /// </summary>
        /// <param name="item">工具栏项</param>
        void Add(ToolBarItemBase item);
    }
}
