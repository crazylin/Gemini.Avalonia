using System;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AuroraUI.Demo.Controls;
using AuroraUI.Demo.ViewModels;

namespace AuroraUI.Demo.Views
{
    /// <summary>
    /// 查找和替换对话框
    /// </summary>
    public partial class FindReplaceDialog : Window
    {
        private readonly EnhancedTextEditor? _textEditor;
        private int _lastFoundIndex = -1;
        
        /// <summary>
        /// 查找和替换视图模型
        /// </summary>
        public FindReplaceViewModel ViewModel { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public FindReplaceDialog(EnhancedTextEditor? textEditor = null)
        {
            InitializeComponent();
            
            _textEditor = textEditor;
            ViewModel = new FindReplaceViewModel();
            DataContext = ViewModel;
            
            // 设置初始焦点
            var findTextBox = this.FindControl<TextBox>("FindTextBox");
            if (findTextBox != null)
            {
                findTextBox.Focus();
                
                // 如果编辑器有选中文本，设置为查找内容
                if (_textEditor != null && _textEditor.HasSelection)
                {
                    var selectedText = _textEditor.Text?.Substring(_textEditor.SelectionStart, 
                        _textEditor.SelectionEnd - _textEditor.SelectionStart) ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(selectedText) && selectedText.Length < 100)
                    {
                        ViewModel.FindText = selectedText;
                        findTextBox.SelectAll();
                    }
                }
            }
        }
        
        /// <summary>
        /// 查找下一个
        /// </summary>
        private void FindNext_Click(object? sender, RoutedEventArgs e)
        {
            if (_textEditor == null || string.IsNullOrEmpty(ViewModel.FindText))
            {
                ViewModel.StatusMessage = "请输入要查找的内容";
                return;
            }
            
            var text = _textEditor.Text ?? string.Empty;
            var findText = ViewModel.FindText;
            
            // 构建搜索选项
            var options = RegexOptions.None;
            if (!ViewModel.MatchCase)
                options |= RegexOptions.IgnoreCase;
            
            try
            {
                string pattern;
                if (ViewModel.MatchWholeWord)
                {
                    pattern = $@"\b{Regex.Escape(findText)}\b";
                }
                else
                {
                    pattern = Regex.Escape(findText);
                }
                
                var regex = new Regex(pattern, options);
                var startIndex = Math.Max(_lastFoundIndex + 1, _textEditor.CaretIndex);
                
                // 从当前位置开始查找
                var match = regex.Match(text, startIndex);
                
                // 如果没找到，从头开始查找
                if (!match.Success && startIndex > 0)
                {
                    match = regex.Match(text, 0);
                }
                
                if (match.Success)
                {
                    _textEditor.CaretIndex = match.Index;
                    _textEditor.SelectionStart = match.Index;
                    _textEditor.SelectionEnd = match.Index + match.Length;
                    _lastFoundIndex = match.Index;
                    
                    ViewModel.StatusMessage = $"找到第 {CountMatches(text, pattern, options, match.Index + 1)} 个匹配项";
                }
                else
                {
                    ViewModel.StatusMessage = "未找到匹配项";
                    _lastFoundIndex = -1;
                }
            }
            catch (Exception ex)
            {
                ViewModel.StatusMessage = $"查找错误: {ex.Message}";
            }
        }
        
        /// <summary>
        /// 替换当前匹配项
        /// </summary>
        private void Replace_Click(object? sender, RoutedEventArgs e)
        {
            if (_textEditor == null || string.IsNullOrEmpty(ViewModel.FindText))
            {
                ViewModel.StatusMessage = "请先查找要替换的内容";
                return;
            }
            
            if (!_textEditor.HasSelection)
            {
                FindNext_Click(sender, e);
                return;
            }
            
            var selectedText = _textEditor.Text?.Substring(_textEditor.SelectionStart, 
                _textEditor.SelectionEnd - _textEditor.SelectionStart) ?? string.Empty;
            
            var findText = ViewModel.FindText;
            var replaceText = ViewModel.ReplaceText ?? string.Empty;
            
            // 检查选中的文本是否匹配查找内容
            bool isMatch;
            if (ViewModel.MatchCase)
                isMatch = selectedText == findText;
            else
                isMatch = string.Equals(selectedText, findText, StringComparison.OrdinalIgnoreCase);
            
            if (ViewModel.MatchWholeWord)
            {
                // 简化的全字匹配检查
                var text = _textEditor.Text ?? string.Empty;
                var start = _textEditor.SelectionStart;
                var end = _textEditor.SelectionEnd;
                
                bool startOk = start == 0 || !char.IsLetterOrDigit(text[start - 1]);
                bool endOk = end == text.Length || !char.IsLetterOrDigit(text[end]);
                
                isMatch = isMatch && startOk && endOk;
            }
            
            if (isMatch)
            {
                var newText = (_textEditor.Text ?? string.Empty).Remove(_textEditor.SelectionStart, 
                    _textEditor.SelectionEnd - _textEditor.SelectionStart)
                    .Insert(_textEditor.SelectionStart, replaceText);
                
                _textEditor.Text = newText;
                _textEditor.CaretIndex = _textEditor.SelectionStart + replaceText.Length;
                
                ViewModel.StatusMessage = "已替换 1 个匹配项";
                
                // 查找下一个
                FindNext_Click(sender, e);
            }
            else
            {
                ViewModel.StatusMessage = "选中的文本与查找内容不匹配";
                FindNext_Click(sender, e);
            }
        }
        
        /// <summary>
        /// 全部替换
        /// </summary>
        private void ReplaceAll_Click(object? sender, RoutedEventArgs e)
        {
            if (_textEditor == null || string.IsNullOrEmpty(ViewModel.FindText))
            {
                ViewModel.StatusMessage = "请输入要查找的内容";
                return;
            }
            
            var text = _textEditor.Text ?? string.Empty;
            var findText = ViewModel.FindText;
            var replaceText = ViewModel.ReplaceText ?? string.Empty;
            
            var options = RegexOptions.None;
            if (!ViewModel.MatchCase)
                options |= RegexOptions.IgnoreCase;
            
            try
            {
                string pattern;
                if (ViewModel.MatchWholeWord)
                {
                    pattern = $@"\b{Regex.Escape(findText)}\b";
                }
                else
                {
                    pattern = Regex.Escape(findText);
                }
                
                var regex = new Regex(pattern, options);
                var matches = regex.Matches(text);
                
                if (matches.Count > 0)
                {
                    var newText = regex.Replace(text, replaceText);
                    _textEditor.Text = newText;
                    
                    ViewModel.StatusMessage = $"已替换 {matches.Count} 个匹配项";
                    _lastFoundIndex = -1;
                }
                else
                {
                    ViewModel.StatusMessage = "未找到匹配项";
                }
            }
            catch (Exception ex)
            {
                ViewModel.StatusMessage = $"替换错误: {ex.Message}";
            }
        }
        
        /// <summary>
        /// 关闭对话框
        /// </summary>
        private void Close_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
        
        /// <summary>
        /// 计算匹配项数量
        /// </summary>
        private int CountMatches(string text, string pattern, RegexOptions options, int beforeIndex)
        {
            try
            {
                var regex = new Regex(pattern, options);
                var matches = regex.Matches(text.Substring(0, beforeIndex));
                return matches.Count;
            }
            catch
            {
                return 0;
            }
        }
    }
}
