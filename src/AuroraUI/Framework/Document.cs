using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using AuroraUI.Modules.UndoRedo;
using ReactiveUI;
using Dock.Model.ReactiveUI.Core;
using Splat;

namespace AuroraUI.Framework
{
    /// <summary>
    /// 文档抽象基类，提供文档的基础功能
    /// </summary>
    public abstract class Document : DockableBase, IDocument
    {
        private IUndoRedoManager? _undoRedoManager;
        private ICommand? _closeCommand;
        private string _displayName = string.Empty;
        private bool _canClose = true;
        private object? _iconSource;
        private string _toolTip = string.Empty;
        private bool _isSelected;
        private bool _shouldReopenOnStart = true;
        private ViewModelActivator? _activator;
        private Guid _layoutId = Guid.NewGuid();
        private string _contentId = string.Empty;
        
        /// <summary>
        /// 撤销重做管理器
        /// </summary>
        public IUndoRedoManager UndoRedoManager
        {
            get => _undoRedoManager ??= CreateUndoRedoManager();
        }
        
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName
        {
            get => _displayName;
            set
            {
                this.RaiseAndSetIfChanged(ref _displayName, value);
                // 同步设置Title属性，用于停靠面板显示
                Title = value;
            }
        }
        
        /// <summary>
        /// 是否可以关闭
        /// </summary>
        public new bool CanClose
        {
            get => _canClose;
            set 
            {
                this.RaiseAndSetIfChanged(ref _canClose, value);
                // 同步更新基类的CanClose属性，确保停靠系统正确识别
                base.CanClose = value;
            }
        }
        
        /// <summary>
        /// 布局项ID
        /// </summary>
        public new Guid Id
        {
            get => _layoutId;
            set => this.RaiseAndSetIfChanged(ref _layoutId, value);
        }
        
        /// <summary>
        /// 内容ID
        /// </summary>
        public string ContentId
        {
            get => _contentId;
            set => this.RaiseAndSetIfChanged(ref _contentId, value);
        }
        
        /// <summary>
        /// 图标源
        /// </summary>
        public Uri? IconSource
        {
            get => _iconSource as Uri;
            set => this.RaiseAndSetIfChanged(ref _iconSource, value);
        }
        
        /// <summary>
        /// 工具提示
        /// </summary>
        public string ToolTip
        {
            get => _toolTip;
            set => this.RaiseAndSetIfChanged(ref _toolTip, value ?? string.Empty);
        }
        
        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }
        
        /// <summary>
        /// 是否在启动时重新打开
        /// </summary>
        public bool ShouldReopenOnStart
        {
            get => _shouldReopenOnStart;
            set => this.RaiseAndSetIfChanged(ref _shouldReopenOnStart, value);
        }
        
        /// <summary>
        /// 激活器
        /// </summary>
        public ViewModelActivator Activator => _activator ??= new ViewModelActivator();
        
        /// <summary>
        /// 加载状态
        /// </summary>
        public virtual void LoadState(BinaryReader reader)
        {
            // 默认实现为空，子类可以重写
        }
        
        /// <summary>
        /// 保存状态
        /// </summary>
        public virtual void SaveState(BinaryWriter writer)
        {
            // 默认实现为空，子类可以重写
        }
        
        /// <summary>
        /// 关闭命令
        /// </summary>
        public ICommand CloseCommand
        {
            get => _closeCommand ??= ReactiveCommand.CreateFromTask(TryCloseAsync);
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        protected Document()
        {
            // 设置默认属性
            CanClose = true;  // 通过属性设置器同步更新基类属性
            base.CanFloat = true;
            base.CanPin = false;
            
            // 设置默认标题
            Title = _displayName;
        }
        
        /// <summary>
        /// 创建撤销重做管理器，子类可重写以提供自定义实现
        /// </summary>
        /// <returns>撤销重做管理器实例</returns>
        protected virtual IUndoRedoManager CreateUndoRedoManager()
        {
            return new UndoRedoManager();
        }
        
        /// <summary>
        /// 尝试关闭文档，子类可重写以添加保存确认等逻辑
        /// </summary>
        /// <returns>关闭任务，如果用户取消则抛出OperationCanceledException</returns>
        public virtual async Task TryCloseAsync()
        {
            // 默认实现直接关闭
            // 子类可以重写此方法来添加保存确认等逻辑
            await Task.CompletedTask;
        }
    }
}