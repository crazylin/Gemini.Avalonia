using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Gemini.Avalonia.Demo.ViewModels;
using Gemini.Avalonia.Demo.Controls;

namespace Gemini.Avalonia.Demo.Views
{
    /// <summary>
    /// 示例文档视图
    /// </summary>
    public partial class SampleDocumentView : UserControl
    {
        /// <summary>
        /// 文本编辑器控件
        /// </summary>
        public EnhancedTextEditor EditorControl => this.FindControl<EnhancedTextEditor>("TextEditor")!;
        
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
                
            // 文档相关快捷键
            if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                switch (e.Key)
                {
                    case Key.S when !e.KeyModifiers.HasFlag(KeyModifiers.Shift):
                        // Ctrl+S 保存
                        if (viewModel.SaveCommand.CanExecute(null))
                        {
                            viewModel.SaveCommand.Execute(null);
                            e.Handled = true;
                        }
                        break;
                    
                    case Key.S when e.KeyModifiers.HasFlag(KeyModifiers.Shift):
                        // Ctrl+Shift+S 另存为
                        if (viewModel.SaveAsCommand.CanExecute(null))
                        {
                            viewModel.SaveAsCommand.Execute(null);
                            e.Handled = true;
                        }
                        break;
                }
            }
            
            // 其他快捷键由EnhancedTextEditor处理
        }
    }
}