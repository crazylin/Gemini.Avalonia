using ReactiveUI;

namespace Gemini.Avalonia.Modules.StatusBar
{
    /// <summary>
    /// 状态栏项视图模型实现
    /// </summary>
    public class StatusBarItemViewModel : ReactiveObject, IStatusBarItem
    {
        private string _text = string.Empty;
        private bool _isVisible = true;
        private double _width = double.NaN; // NaN表示自动宽度
        
        /// <summary>
        /// 状态栏项文本
        /// </summary>
        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }
        
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set => this.RaiseAndSetIfChanged(ref _isVisible, value);
        }
        
        /// <summary>
        /// 宽度（NaN表示自动宽度）
        /// </summary>
        public double Width
        {
            get => _width;
            set => this.RaiseAndSetIfChanged(ref _width, value);
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public StatusBarItemViewModel()
        {
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="text">状态栏项文本</param>
        public StatusBarItemViewModel(string text)
        {
            Text = text;
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="text">状态栏项文本</param>
        /// <param name="width">宽度</param>
        public StatusBarItemViewModel(string text, double width)
        {
            Text = text;
            Width = width;
        }
        
        /// <summary>
        /// 设置固定宽度
        /// </summary>
        /// <param name="width">宽度值</param>
        public void SetFixedWidth(double width)
        {
            Width = width;
        }
        
        /// <summary>
        /// 设置自动宽度
        /// </summary>
        public void SetAutoWidth()
        {
            Width = double.NaN;
        }
    }
}