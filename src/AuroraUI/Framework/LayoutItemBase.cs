using System;
using System.ComponentModel;
using System.IO;
using System.Reactive.Disposables;
using System.Windows.Input;
using ReactiveUI;

namespace AuroraUI.Framework
{
    /// <summary>
    /// 布局项基类，提供ILayoutItem接口的基础实现
    /// </summary>
    public abstract class LayoutItemBase : ReactiveObject, ILayoutItem, IActivatableViewModel
    {
        private readonly Guid _id = Guid.NewGuid();
        private string _displayName = string.Empty;
        private string _toolTip = string.Empty;
        private bool _isSelected;
        
        /// <summary>
        /// 激活器，用于管理视图模型的激活状态
        /// </summary>
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        
        /// <summary>
        /// 抽象的关闭命令，由子类实现
        /// </summary>
        public abstract ICommand CloseCommand { get; }
        
        /// <summary>
        /// 唯一标识符
        /// </summary>
        [Browsable(false)]
        public Guid Id => _id;
        
        /// <summary>
        /// 内容标识符，用于布局持久化
        /// </summary>
        [Browsable(false)]
        public string ContentId => _id.ToString();
        
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName
        {
            get => _displayName;
            set => this.RaiseAndSetIfChanged(ref _displayName, value);
        }
        
        /// <summary>
        /// 图标源，默认为null，子类可重写
        /// </summary>
        [Browsable(false)]
        public virtual Uri? IconSource => null;
        
        /// <summary>
        /// 工具提示
        /// </summary>
        [Browsable(false)]
        public string ToolTip
        {
            get => _toolTip;
            set => this.RaiseAndSetIfChanged(ref _toolTip, value);
        }
        
        /// <summary>
        /// 是否被选中
        /// </summary>
        [Browsable(false)]
        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }
        
        /// <summary>
        /// 是否应该在启动时重新打开，默认为false
        /// </summary>
        [Browsable(false)]
        public virtual bool ShouldReopenOnStart => false;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        protected LayoutItemBase()
        {
            // 当视图模型被激活时的处理逻辑
            this.WhenActivated(disposables =>
            {
                // 在这里可以添加激活时的逻辑
                Disposable.Create(() =>
                {
                    // 在这里可以添加停用时的清理逻辑
                }).DisposeWith(disposables);
            });
        }
        
        /// <summary>
        /// 加载状态，默认实现为空，子类可重写
        /// </summary>
        /// <param name="reader">二进制读取器</param>
        public virtual void LoadState(BinaryReader reader)
        {
            // 默认实现为空
        }
        
        /// <summary>
        /// 保存状态，默认实现为空，子类可重写
        /// </summary>
        /// <param name="writer">二进制写入器</param>
        public virtual void SaveState(BinaryWriter writer)
        {
            // 默认实现为空
        }
    }
}