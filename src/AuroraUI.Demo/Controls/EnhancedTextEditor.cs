using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Media;
using AuroraUI.Demo.Commands;
using AuroraUI.Demo.ViewModels;
using AuroraUI.Demo.Views;
using AuroraUI.Modules.UndoRedo;

namespace AuroraUI.Demo.Controls
{
    /// <summary>
    /// 增强的文本编辑器控件，支持撤销/重做和完整的编辑功能
    /// </summary>
    public class EnhancedTextEditor : TextBox, ITextEditor
    {
        #region 依赖属性

        public static readonly StyledProperty<SampleDocumentViewModel?> DocumentProperty =
            AvaloniaProperty.Register<EnhancedTextEditor, SampleDocumentViewModel?>(nameof(Document));

        public SampleDocumentViewModel? Document
        {
            get => GetValue(DocumentProperty);
            set => SetValue(DocumentProperty, value);
        }

        #endregion

        #region 字段

        private readonly Stack<TextChange> _undoStack = new();
        private readonly Stack<TextChange> _redoStack = new();
        private bool _isUpdatingText = false;
        private string _lastText = string.Empty;
        private int _maxUndoStackSize = 100;

        #endregion

        #region 构造函数

        public EnhancedTextEditor()
        {
            AcceptsReturn = true;
            AcceptsTab = true;
            TextWrapping = TextWrapping.Wrap;
            FontFamily = "Consolas,Monaco,Courier New,monospace";
            FontSize = 14;
            Padding = new Thickness(12);
            MinHeight = 300;
            
            // 监听文本变化
            PropertyChanged += OnPropertyChanged;
            
            // 设置上下文菜单
            SetupContextMenu();
            
            _lastText = Text ?? string.Empty;
        }

        #endregion

        #region ITextEditor 实现

        public bool HasSelection => SelectionStart != SelectionEnd;

        public new bool CanUndo => _undoStack.Count > 0;

        public new bool CanRedo => _redoStack.Count > 0;

        public new bool CanPaste => true; // 简化实现，实际应检查剪贴板

        public new void Undo()
        {
            if (!CanUndo) return;

            var change = _undoStack.Pop();
            _redoStack.Push(new TextChange(change.NewText, change.OldText, change.StartIndex, change.NewLength));
            
            _isUpdatingText = true;
            try
            {
                Text = change.OldText;
                CaretIndex = change.StartIndex;
            }
            finally
            {
                _isUpdatingText = false;
            }
            
            UpdateDocument();
        }

        public new void Redo()
        {
            if (!CanRedo) return;

            var change = _redoStack.Pop();
            _undoStack.Push(new TextChange(change.NewText, change.OldText, change.StartIndex, change.NewLength));
            
            _isUpdatingText = true;
            try
            {
                Text = change.OldText;
                CaretIndex = change.StartIndex + change.NewLength;
            }
            finally
            {
                _isUpdatingText = false;
            }
            
            UpdateDocument();
        }

        public new async Task Cut()
        {
            if (!HasSelection) return;
            
            await Copy();
            var selectedText = Text?.Substring(SelectionStart, SelectionEnd - SelectionStart) ?? string.Empty;
            var newText = (Text ?? string.Empty).Remove(SelectionStart, SelectionEnd - SelectionStart);
            
            RecordTextChange(Text ?? string.Empty, newText, SelectionStart, 0);
            Text = newText;
            CaretIndex = SelectionStart;
        }

        public new async Task Copy()
        {
            if (!HasSelection) return;
            
            var selectedText = Text?.Substring(SelectionStart, SelectionEnd - SelectionStart) ?? string.Empty;
            
            try
            {
                var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
                if (clipboard != null)
                {
                    await clipboard.SetTextAsync(selectedText);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"复制到剪贴板失败: {ex.Message}");
            }
        }

        public new async Task Paste()
        {
            try
            {
                var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
                if (clipboard != null)
                {
                    var clipboardText = await clipboard.GetTextAsync();
                    if (!string.IsNullOrEmpty(clipboardText))
                    {
                        var oldText = Text ?? string.Empty;
                        var startIndex = SelectionStart;
                        var length = SelectionEnd - SelectionStart;
                        
                        var newText = oldText.Remove(startIndex, length).Insert(startIndex, clipboardText);
                        
                        RecordTextChange(oldText, newText, startIndex, clipboardText.Length);
                        Text = newText;
                        CaretIndex = startIndex + clipboardText.Length;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"从剪贴板粘贴失败: {ex.Message}");
            }
        }

        public void ShowFindDialog()
        {
            try
            {
                var parentWindow = TopLevel.GetTopLevel(this) as Window;
                var dialog = new Views.FindReplaceDialog(this);
                if (parentWindow != null)
                {
                    dialog.ShowDialog(parentWindow);
                }
                else
                {
                    dialog.Show();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"显示查找对话框失败: {ex.Message}");
            }
        }

        public void ShowReplaceDialog()
        {
            try
            {
                var parentWindow = TopLevel.GetTopLevel(this) as Window;
                var dialog = new Views.FindReplaceDialog(this);
                if (parentWindow != null)
                {
                    dialog.ShowDialog(parentWindow);
                }
                else
                {
                    dialog.Show();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"显示替换对话框失败: {ex.Message}");
            }
        }

        public void ShowGoToLineDialog()
        {
            try
            {
                var parentWindow = TopLevel.GetTopLevel(this) as Window;
                var dialog = new Views.GoToLineDialog(this);
                if (parentWindow != null)
                {
                    dialog.ShowDialog(parentWindow);
                }
                else
                {
                    dialog.Show();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"显示跳转到行对话框失败: {ex.Message}");
            }
        }

        #endregion

        #region 私有方法

        private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == TextProperty && !_isUpdatingText)
            {
                var newText = Text ?? string.Empty;
                if (newText != _lastText)
                {
                    // 简化的文本变化检测，实际应该更精确
                    var startIndex = Math.Min(CaretIndex, newText.Length);
                    var addedLength = newText.Length - _lastText.Length;
                    
                    RecordTextChange(_lastText, newText, startIndex, Math.Max(0, addedLength));
                    _lastText = newText;
                    
                    UpdateDocument();
                }
            }
            else if (e.Property == DocumentProperty)
            {
                UpdateFromDocument();
            }
        }

        private void RecordTextChange(string oldText, string newText, int startIndex, int newLength)
        {
            if (_isUpdatingText) return;
            
            var change = new TextChange(oldText, newText, startIndex, newLength);
            _undoStack.Push(change);
            
            // 清空重做栈
            _redoStack.Clear();
            
            // 限制撤销栈大小
            if (_undoStack.Count > _maxUndoStackSize)
            {
                var items = _undoStack.Take(_maxUndoStackSize).ToArray();
                _undoStack.Clear();
                for (int i = items.Length - 1; i >= 0; i--)
                {
                    _undoStack.Push(items[i]);
                }
            }
        }

        private void UpdateDocument()
        {
            if (Document != null && !_isUpdatingText)
            {
                Document.Content = Text ?? string.Empty;
            }
        }

        private void UpdateFromDocument()
        {
            if (Document != null)
            {
                _isUpdatingText = true;
                try
                {
                    Text = Document.Content;
                    _lastText = Text ?? string.Empty;
                }
                finally
                {
                    _isUpdatingText = false;
                }
            }
        }

        private void SetupContextMenu()
        {
            var contextMenu = new ContextMenu();
            
            var undoItem = new MenuItem { Header = "撤销", InputGesture = new KeyGesture(Key.Z, KeyModifiers.Control) };
            undoItem.Click += (s, e) => { if (CanUndo) Undo(); };
            contextMenu.Items.Add(undoItem);
            
            var redoItem = new MenuItem { Header = "重做", InputGesture = new KeyGesture(Key.Y, KeyModifiers.Control) };
            redoItem.Click += (s, e) => { if (CanRedo) Redo(); };
            contextMenu.Items.Add(redoItem);
            
            contextMenu.Items.Add(new Separator());
            
            var cutItem = new MenuItem { Header = "剪切", InputGesture = new KeyGesture(Key.X, KeyModifiers.Control) };
            cutItem.Click += async (s, e) => await Cut();
            contextMenu.Items.Add(cutItem);
            
            var copyItem = new MenuItem { Header = "复制", InputGesture = new KeyGesture(Key.C, KeyModifiers.Control) };
            copyItem.Click += async (s, e) => await Copy();
            contextMenu.Items.Add(copyItem);
            
            var pasteItem = new MenuItem { Header = "粘贴", InputGesture = new KeyGesture(Key.V, KeyModifiers.Control) };
            pasteItem.Click += async (s, e) => await Paste();
            contextMenu.Items.Add(pasteItem);
            
            contextMenu.Items.Add(new Separator());
            
            var selectAllItem = new MenuItem { Header = "全选", InputGesture = new KeyGesture(Key.A, KeyModifiers.Control) };
            selectAllItem.Click += (s, e) => SelectAll();
            contextMenu.Items.Add(selectAllItem);
            
            // 动态更新菜单项状态
            contextMenu.Opening += (s, e) =>
            {
                undoItem.IsEnabled = CanUndo;
                redoItem.IsEnabled = CanRedo;
                cutItem.IsEnabled = HasSelection;
                copyItem.IsEnabled = HasSelection;
                pasteItem.IsEnabled = CanPaste;
                selectAllItem.IsEnabled = !string.IsNullOrEmpty(Text);
            };
            
            ContextMenu = contextMenu;
        }

        #endregion

        #region 键盘处理

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // 处理快捷键
            if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                switch (e.Key)
                {
                    case Key.Z:
                        if (CanUndo)
                        {
                            Undo();
                            e.Handled = true;
                        }
                        break;
                    case Key.Y:
                        if (CanRedo)
                        {
                            Redo();
                            e.Handled = true;
                        }
                        break;
                    case Key.X:
                        if (HasSelection)
                        {
                            _ = Cut();
                            e.Handled = true;
                        }
                        break;
                    case Key.C:
                        if (HasSelection)
                        {
                            _ = Copy();
                            e.Handled = true;
                        }
                        break;
                    case Key.V:
                        _ = Paste();
                        e.Handled = true;
                        break;
                    case Key.A:
                        SelectAll();
                        e.Handled = true;
                        break;
                }
            }

            if (!e.Handled)
            {
                base.OnKeyDown(e);
            }
        }

        #endregion
    }

    /// <summary>
    /// 文本变化记录，用于撤销/重做
    /// </summary>
    public class TextChange
    {
        public string OldText { get; }
        public string NewText { get; }
        public int StartIndex { get; }
        public int NewLength { get; }

        public TextChange(string oldText, string newText, int startIndex, int newLength)
        {
            OldText = oldText;
            NewText = newText;
            StartIndex = startIndex;
            NewLength = newLength;
        }
    }
}
