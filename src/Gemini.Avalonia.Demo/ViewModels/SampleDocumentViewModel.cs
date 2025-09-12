using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Modules.UndoRedo;
using ReactiveUI;

namespace Gemini.Avalonia.Demo.ViewModels
{
    /// <summary>
    /// 示例文档视图模型
    /// </summary>
    public class SampleDocumentViewModel : Document
    {
        private string _content = string.Empty;
        private bool _isDirty;
        private string _filePath = string.Empty;
        
        /// <summary>
        /// 文档内容
        /// </summary>
        public string Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    var oldContent = _content;
                    this.RaiseAndSetIfChanged(ref _content, value);
                    
                    // 创建撤销重做动作
                    if (!string.IsNullOrEmpty(oldContent) || !string.IsNullOrEmpty(value))
                    {
                        var action = new TextChangeAction(this, oldContent, value);
                        // 注意：这里不应该直接执行，因为内容已经改变了
                        // UndoRedoManager.ExecuteAction(action);
                    }
                    
                    IsDirty = true;
                }
            }
        }
        
        /// <summary>
        /// 是否有未保存的更改
        /// </summary>
        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                this.RaiseAndSetIfChanged(ref _isDirty, value);
                UpdateDisplayName();
            }
        }
        
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath
        {
            get => _filePath;
            set
            {
                this.RaiseAndSetIfChanged(ref _filePath, value);
                UpdateDisplayName();
            }
        }
        
        /// <summary>
        /// 保存命令
        /// </summary>
        public ICommand SaveCommand { get; }
        
        /// <summary>
        /// 另存为命令
        /// </summary>
        public ICommand SaveAsCommand { get; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public SampleDocumentViewModel()
        {
            DisplayName = "未命名文档";
            
            // 创建命令
            SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync, this.WhenAnyValue(x => x.IsDirty));
            SaveAsCommand = ReactiveCommand.CreateFromTask(SaveAsAsync);
            
            // 监听内容变化
            this.WhenAnyValue(x => x.Content)
                .Subscribe(_ => IsDirty = true);
        }
        
        /// <summary>
        /// 保存文档
        /// </summary>
        private async Task SaveAsync()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                await SaveAsAsync();
            }
            else
            {
                try
                {
                    await File.WriteAllTextAsync(FilePath, Content);
                    IsDirty = false;
                }
                catch (Exception ex)
                {
                    // 这里应该显示错误消息
                    System.Diagnostics.Debug.WriteLine($"保存文件失败: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// 另存为文档
        /// </summary>
        private async Task SaveAsAsync()
        {
            // 这里应该显示保存文件对话框
            // 为了演示，我们使用一个临时路径
            var tempPath = Path.Combine(Path.GetTempPath(), $"sample_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            
            try
            {
                await File.WriteAllTextAsync(tempPath, Content);
                FilePath = tempPath;
                IsDirty = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存文件失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 更新显示名称
        /// </summary>
        private void UpdateDisplayName()
        {
            var name = string.IsNullOrEmpty(FilePath) ? "未命名文档" : Path.GetFileName(FilePath);
            DisplayName = IsDirty ? $"{name}*" : name;
        }
        
        /// <summary>
        /// 尝试关闭文档
        /// </summary>
        protected override async Task TryCloseAsync()
        {
            if (IsDirty)
            {
                // 这里应该显示保存确认对话框
                // 为了演示，我们直接保存
                await SaveAsync();
            }
            
            await base.TryCloseAsync();
        }
        
        /// <summary>
        /// 加载状态
        /// </summary>
        public override void LoadState(BinaryReader reader)
        {
            try
            {
                FilePath = reader.ReadString();
                Content = reader.ReadString();
                IsDirty = reader.ReadBoolean();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载文档状态失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 保存状态
        /// </summary>
        public override void SaveState(BinaryWriter writer)
        {
            try
            {
                writer.Write(FilePath ?? string.Empty);
                writer.Write(Content ?? string.Empty);
                writer.Write(IsDirty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存文档状态失败: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// 文本更改动作，用于撤销重做
    /// </summary>
    public class TextChangeAction : IUndoableAction
    {
        private readonly SampleDocumentViewModel _document;
        private readonly string _oldText;
        private readonly string _newText;
        
        public string Name => "文本更改";
        
        public TextChangeAction(SampleDocumentViewModel document, string oldText, string newText)
        {
            _document = document;
            _oldText = oldText;
            _newText = newText;
        }
        
        public void Execute()
        {
            _document.Content = _newText;
        }
        
        public void Undo()
        {
            _document.Content = _oldText;
        }
    }
}