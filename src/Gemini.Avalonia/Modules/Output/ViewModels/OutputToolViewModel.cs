using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Windows.Input;
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Framework.Services;
using Gemini.Avalonia.Services;
using ReactiveUI;

namespace Gemini.Avalonia.Modules.Output.ViewModels
{
    /// <summary>
    /// 输出工具视图模型
    /// </summary>
    [Export(typeof(ITool))]
    [Export(typeof(OutputToolViewModel))]
    public class OutputToolViewModel : Tool
    {
        private string _selectedCategory = "常规";
        private int _maxLines = 1000;
        
        /// <summary>
        /// 本地化服务
        /// </summary>
        [Import]
        public ILocalizationService LocalizationService { get; set; } = null!;
        
        /// <summary>
        /// 首选位置
        /// </summary>
        public override PaneLocation PreferredLocation => PaneLocation.Bottom;
        
        /// <summary>
        /// 首选宽度
        /// </summary>
        public override double PreferredWidth => 600;
        
        /// <summary>
        /// 首选高度
        /// </summary>
        public override double PreferredHeight => 200;
        
        /// <summary>
        /// 输出类别
        /// </summary>
        public ObservableCollection<string> Categories { get; }
        
        /// <summary>
        /// 选中的类别
        /// </summary>
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedCategory, value);
                UpdateFilteredMessages();
            }
        }
        
        /// <summary>
        /// 所有消息
        /// </summary>
        public ObservableCollection<OutputMessageViewModel> AllMessages { get; }
        
        /// <summary>
        /// 过滤后的消息
        /// </summary>
        public ObservableCollection<OutputMessageViewModel> FilteredMessages { get; }
        
        /// <summary>
        /// 最大行数
        /// </summary>
        public int MaxLines
        {
            get => _maxLines;
            set
            {
                this.RaiseAndSetIfChanged(ref _maxLines, value);
                TrimMessages();
            }
        }
        
        /// <summary>
        /// 清空命令
        /// </summary>
        public ICommand ClearCommand { get; }
        
        /// <summary>
        /// 复制命令
        /// </summary>
        public ICommand CopyCommand { get; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public OutputToolViewModel()
        {
            // 初始化集合
            Categories = new ObservableCollection<string> { "常规", "构建", "调试", "错误" };
            AllMessages = new ObservableCollection<OutputMessageViewModel>();
            FilteredMessages = new ObservableCollection<OutputMessageViewModel>();
            
            // 创建命令
            ClearCommand = ReactiveCommand.Create(Clear);
            CopyCommand = ReactiveCommand.Create(CopyToClipboard);
            
            // 添加示例消息
            InitializeSampleMessages();
        }
        
        /// <summary>
        /// 初始化显示名称（在MEF注入完成后调用）
        /// </summary>
        public override void Initialize()
        {
            DisplayName = LocalizationService?.GetString("Output.Title");
            ToolTip = LocalizationService?.GetString("Output.ToolTip");
        }
        
        /// <summary>
        /// 初始化示例消息
        /// </summary>
        private void InitializeSampleMessages()
        {
            WriteLine("欢迎使用 Gemini.Avalonia 框架!", "常规", OutputMessageType.Information);
            WriteLine("框架初始化完成。", "常规", OutputMessageType.Information);
            WriteLine("正在加载模块...", "构建", OutputMessageType.Information);
            WriteLine("模块加载完成。", "构建", OutputMessageType.Success);
            WriteLine("这是一个警告消息示例。", "调试", OutputMessageType.Warning);
            WriteLine("这是一个错误消息示例。", "错误", OutputMessageType.Error);
        }
        
        /// <summary>
        /// 写入一行消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="category">类别</param>
        /// <param name="type">消息类型</param>
        public void WriteLine(string message, string category = "常规", OutputMessageType type = OutputMessageType.Information)
        {
            var outputMessage = new OutputMessageViewModel
            {
                Timestamp = DateTime.Now,
                Message = message,
                Category = category,
                Type = type
            };
            
            AllMessages.Add(outputMessage);
            
            // 添加类别（如果不存在）
            if (!Categories.Contains(category))
            {
                Categories.Add(category);
            }
            
            // 限制消息数量
            TrimMessages();
            
            // 更新过滤后的消息
            UpdateFilteredMessages();
        }
        
        /// <summary>
        /// 清空消息
        /// </summary>
        private void Clear()
        {
            AllMessages.Clear();
            FilteredMessages.Clear();
        }
        
        /// <summary>
        /// 复制到剪贴板
        /// </summary>
        private void CopyToClipboard()
        {
            try
            {
                var text = string.Join(Environment.NewLine, 
                    FilteredMessages.Select(m => $"[{m.Timestamp:HH:mm:ss}] {m.Message}"));
                    
                // 这里应该使用Avalonia的剪贴板API
                // Application.Current?.Clipboard?.SetTextAsync(text);
                System.Diagnostics.Debug.WriteLine($"复制到剪贴板: {text.Length} 个字符");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"复制失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 限制消息数量
        /// </summary>
        private void TrimMessages()
        {
            while (AllMessages.Count > MaxLines)
            {
                AllMessages.RemoveAt(0);
            }
        }
        
        /// <summary>
        /// 更新过滤后的消息
        /// </summary>
        private void UpdateFilteredMessages()
        {
            FilteredMessages.Clear();
            
            var filtered = AllMessages.Where(m => m.Category == SelectedCategory);
            
            foreach (var message in filtered)
            {
                FilteredMessages.Add(message);
            }
        }
        
        /// <summary>
        /// 加载状态
        /// </summary>
        public override void LoadState(BinaryReader reader)
        {
            try
            {
                SelectedCategory = reader.ReadString();
                MaxLines = reader.ReadInt32();
                
                // 清空现有消息
                AllMessages.Clear();
                
                // 读取消息数量
                var messageCount = reader.ReadInt32();
                
                for (int i = 0; i < messageCount; i++)
                {
                    var message = new OutputMessageViewModel
                    {
                        Timestamp = DateTime.FromBinary(reader.ReadInt64()),
                        Message = reader.ReadString(),
                        Category = reader.ReadString(),
                        Type = (OutputMessageType)reader.ReadInt32()
                    };
                    
                    AllMessages.Add(message);
                }
                
                UpdateFilteredMessages();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载输出工具状态失败: {ex.Message}");
                InitializeSampleMessages();
            }
        }
        
        /// <summary>
        /// 保存状态
        /// </summary>
        public override void SaveState(BinaryWriter writer)
        {
            try
            {
                writer.Write(SelectedCategory);
                writer.Write(MaxLines);
                
                // 写入消息数量
                writer.Write(AllMessages.Count);
                
                // 写入每个消息
                foreach (var message in AllMessages)
                {
                    writer.Write(message.Timestamp.ToBinary());
                    writer.Write(message.Message);
                    writer.Write(message.Category);
                    writer.Write((int)message.Type);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存输出工具状态失败: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// 输出消息视图模型
    /// </summary>
    public class OutputMessageViewModel : ReactiveObject
    {
        private DateTime _timestamp;
        private string _message = string.Empty;
        private string _category = string.Empty;
        private OutputMessageType _type;
        
        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp
        {
            get => _timestamp;
            set => this.RaiseAndSetIfChanged(ref _timestamp, value);
        }
        
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }
        
        /// <summary>
        /// 类别
        /// </summary>
        public string Category
        {
            get => _category;
            set => this.RaiseAndSetIfChanged(ref _category, value);
        }
        
        /// <summary>
        /// 消息类型
        /// </summary>
        public OutputMessageType Type
        {
            get => _type;
            set => this.RaiseAndSetIfChanged(ref _type, value);
        }
        
        /// <summary>
        /// 格式化的显示文本
        /// </summary>
        public string DisplayText => $"[{Timestamp:HH:mm:ss}] {Message}";
    }
    
    /// <summary>
    /// 输出消息类型
    /// </summary>
    public enum OutputMessageType
    {
        /// <summary>
        /// 信息
        /// </summary>
        Information,
        
        /// <summary>
        /// 成功
        /// </summary>
        Success,
        
        /// <summary>
        /// 警告
        /// </summary>
        Warning,
        
        /// <summary>
        /// 错误
        /// </summary>
        Error
    }
}