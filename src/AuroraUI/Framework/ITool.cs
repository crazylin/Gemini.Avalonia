using AuroraUI.Framework.Services;

namespace AuroraUI.Framework
{
    /// <summary>
    /// 工具窗口接口，表示可以停靠的工具面板
    /// </summary>
    public interface ITool : ILayoutItem
    {
        /// <summary>
        /// 首选位置
        /// </summary>
        PaneLocation PreferredLocation { get; }
        
        /// <summary>
        /// 首选宽度
        /// </summary>
        double PreferredWidth { get; }
        
        /// <summary>
        /// 首选高度
        /// </summary>
        double PreferredHeight { get; }
        
        /// <summary>
        /// 是否默认折叠
        /// </summary>
        bool DefaultCollapsed { get; }
        
        /// <summary>
    /// 是否可见
    /// </summary>
    bool IsVisible { get; set; }
    
    /// <summary>
    /// 初始化工具（在MEF注入完成后调用）
    /// </summary>
    void Initialize();
}
}