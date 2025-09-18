using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using ReactiveUI;

namespace AuroraUI.Modules.StatusBar
{
    /// <summary>
    /// 状态栏视图模型实现
    /// </summary>
    [Export(typeof(IStatusBar))]
    public class StatusBarViewModel : ReactiveObject, IStatusBar
    {
        private string _text = "就绪";
        private bool _isVisible = true;
        
        /// <summary>
        /// 状态栏项集合
        /// </summary>
        public ObservableCollection<IStatusBarItem> Items { get; } = new();
        
        /// <summary>
        /// 主要状态文本
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
        /// 构造函数
        /// </summary>
        public StatusBarViewModel()
        {
            InitializeDefaultItems();
        }
        
        /// <summary>
        /// 初始化默认状态栏项
        /// </summary>
        private void InitializeDefaultItems()
        {
            // 可以在这里添加一些默认的状态栏项
            // 例如：行列信息、选择信息等
        }
        
        /// <summary>
        /// 添加状态栏项
        /// </summary>
        /// <param name="item">状态栏项</param>
        public void AddItem(IStatusBarItem item)
        {
            if (item != null && !Items.Contains(item))
            {
                Items.Add(item);
            }
        }
        
        /// <summary>
        /// 移除状态栏项
        /// </summary>
        /// <param name="item">状态栏项</param>
        public void RemoveItem(IStatusBarItem item)
        {
            if (item != null && Items.Contains(item))
            {
                Items.Remove(item);
            }
        }
        
        /// <summary>
        /// 清空所有状态栏项
        /// </summary>
        public void Clear()
        {
            Items.Clear();
        }
        
        /// <summary>
        /// 设置状态文本
        /// </summary>
        /// <param name="text">状态文本</param>
        public void SetStatus(string text)
        {
            Text = text;
        }
        
        /// <summary>
        /// 设置临时状态文本
        /// </summary>
        /// <param name="text">状态文本</param>
        public void SetTemporaryStatus(string text)
        {
            Text = text;
            // 可以在这里添加定时器来自动清除临时状态
        }
        
        /// <summary>
        /// 设置临时状态文本（可以在一定时间后恢复）
        /// </summary>
        /// <param name="text">临时状态文本</param>
        /// <param name="duration">持续时间（毫秒）</param>
        public async void SetTemporaryStatus(string text, int duration = 3000)
        {
            var originalText = Text;
            Text = text;
            
            await System.Threading.Tasks.Task.Delay(duration);
            
            // 如果状态文本没有被其他操作改变，则恢复原始文本
            if (Text == text)
            {
                Text = originalText;
            }
        }
    }
}