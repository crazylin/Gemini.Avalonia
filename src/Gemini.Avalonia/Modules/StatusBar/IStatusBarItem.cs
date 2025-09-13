using ReactiveUI;

namespace Gemini.Avalonia.Modules.StatusBar
{
    /// <summary>
    /// 状态栏项接口
    /// </summary>
    public interface IStatusBarItem : IReactiveObject
    {
        /// <summary>
        /// 显示文本
        /// </summary>
        string Text { get; set; }
        
        /// <summary>
        /// 是否可见
        /// </summary>
        bool IsVisible { get; set; }
        
        /// <summary>
        /// 宽度
        /// </summary>
        double Width { get; set; }
    }
}