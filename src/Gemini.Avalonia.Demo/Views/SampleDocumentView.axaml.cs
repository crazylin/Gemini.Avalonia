using Avalonia.Controls;
using Avalonia.Input;
using Gemini.Avalonia.Demo.ViewModels;

namespace Gemini.Avalonia.Demo.Views
{
    /// <summary>
    /// 示例文档视图
    /// </summary>
    public partial class SampleDocumentView : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SampleDocumentView()
        {
            InitializeComponent();
            
            // 添加键盘快捷键
            AddHandler(KeyDownEvent, OnKeyDown, handledEventsToo: true);
        }
        
        /// <summary>
        /// 处理键盘按键事件
        /// </summary>
        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (DataContext is not SampleDocumentViewModel viewModel)
                return;
                
            // Ctrl+S 保存
            if (e.Key == Key.S && e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                if (viewModel.SaveCommand.CanExecute(null))
                {
                    viewModel.SaveCommand.Execute(null);
                    e.Handled = true;
                }
            }
            // Ctrl+Shift+S 另存为
            else if (e.Key == Key.S && e.KeyModifiers.HasFlag(KeyModifiers.Control | KeyModifiers.Shift))
            {
                if (viewModel.SaveAsCommand.CanExecute(null))
                {
                    viewModel.SaveAsCommand.Execute(null);
                    e.Handled = true;
                }
            }
        }
    }
}