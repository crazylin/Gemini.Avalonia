using System.Collections.ObjectModel;
using ReactiveUI;

namespace Gemini.Avalonia.Modules.StatusBar
{
    /// <summary>
    /// 状态栏接口
    /// </summary>
    public interface IStatusBar : IReactiveObject
    {
        /// <summary>
        /// 状态栏项集合
        /// </summary>
        ObservableCollection<IStatusBarItem> Items { get; }
        
        /// <summary>
        /// 主要状态文本
        /// </summary>
        string Text { get; set; }
        
        /// <summary>
        /// 是否可见
        /// </summary>
        bool IsVisible { get; set; }
        
        /// <summary>
        /// 设置状态文本
        /// </summary>
        /// <param name="text">状态文本</param>
        void SetStatus(string text);
        
        /// <summary>
        /// 设置临时状态文本
        /// </summary>
        /// <param name="text">状态文本</param>
        void SetTemporaryStatus(string text);
    }
}