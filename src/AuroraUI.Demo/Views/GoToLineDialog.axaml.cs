using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using AuroraUI.Demo.Controls;
using AuroraUI.Demo.ViewModels;

namespace AuroraUI.Demo.Views
{
    /// <summary>
    /// 跳转到行对话框
    /// </summary>
    public partial class GoToLineDialog : Window
    {
        private readonly EnhancedTextEditor? _textEditor;
        
        /// <summary>
        /// 跳转到行视图模型
        /// </summary>
        public GoToLineViewModel ViewModel { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public GoToLineDialog(EnhancedTextEditor? textEditor = null)
        {
            InitializeComponent();
            
            _textEditor = textEditor;
            ViewModel = new GoToLineViewModel();
            DataContext = ViewModel;
            
            // 计算总行数
            if (_textEditor != null)
            {
                var text = _textEditor.Text ?? string.Empty;
                var lineCount = text.Split('\n').Length;
                ViewModel.InfoMessage = $"当前文档共有 {lineCount} 行";
            }
            else
            {
                ViewModel.InfoMessage = "无法获取文档信息";
            }
            
            // 设置初始焦点
            var lineNumberTextBox = this.FindControl<TextBox>("LineNumberTextBox");
            if (lineNumberTextBox != null)
            {
                lineNumberTextBox.Focus();
                lineNumberTextBox.SelectAll();
                
                // 添加键盘事件处理
                lineNumberTextBox.KeyDown += OnLineNumberTextBox_KeyDown;
            }
        }
        
        /// <summary>
        /// 处理文本框键盘事件
        /// </summary>
        private void OnLineNumberTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GoTo_Click(sender, new RoutedEventArgs());
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                Cancel_Click(sender, new RoutedEventArgs());
                e.Handled = true;
            }
        }
        
        /// <summary>
        /// 跳转到指定行
        /// </summary>
        private void GoTo_Click(object? sender, RoutedEventArgs e)
        {
            if (_textEditor == null)
            {
                ViewModel.InfoMessage = "没有可用的文本编辑器";
                return;
            }
            
            if (!int.TryParse(ViewModel.LineNumber, out var lineNumber) || lineNumber < 1)
            {
                ViewModel.InfoMessage = "请输入有效的行号（大于0的整数）";
                return;
            }
            
            var text = _textEditor.Text ?? string.Empty;
            var lines = text.Split('\n');
            
            if (lineNumber > lines.Length)
            {
                ViewModel.InfoMessage = $"行号超出范围，文档只有 {lines.Length} 行";
                return;
            }
            
            try
            {
                // 计算目标行的字符索引
                var targetIndex = 0;
                for (int i = 0; i < lineNumber - 1; i++)
                {
                    targetIndex += lines[i].Length + 1; // +1 for the newline character
                }
                
                // 确保索引不超出文本长度
                targetIndex = Math.Min(targetIndex, text.Length);
                
                // 跳转到目标位置
                _textEditor.CaretIndex = targetIndex;
                _textEditor.Focus();
                
                // 选择整行（可选）
                var lineEnd = targetIndex + (lineNumber <= lines.Length ? lines[lineNumber - 1].Length : 0);
                _textEditor.SelectionStart = targetIndex;
                _textEditor.SelectionEnd = lineEnd;
                
                // DialogResult = true; // Avalonia窗口没有DialogResult属性
                Close();
            }
            catch (Exception ex)
            {
                ViewModel.InfoMessage = $"跳转失败: {ex.Message}";
            }
        }
        
        /// <summary>
        /// 取消对话框
        /// </summary>
        private void Cancel_Click(object? sender, RoutedEventArgs e)
        {
            // DialogResult = false; // Avalonia窗口没有DialogResult属性
            Close();
        }
    }
}
