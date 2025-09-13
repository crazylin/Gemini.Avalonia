using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Gemini.Avalonia.Framework.Services;
using ReactiveUI;
using DockTool = Dock.Model.ReactiveUI.Controls.Tool;

namespace Gemini.Avalonia.Framework
{
    /// <summary>
    /// 工具抽象基类，提供工具窗口的基础功能
    /// </summary>
    public abstract class Tool : DockTool, ITool
    {
        private readonly Guid _id = Guid.NewGuid();
        private string _displayName = string.Empty;
        private string _toolTip = string.Empty;
        private bool _isSelected;
        private bool _isVisible = true;
        
        /// <summary>
        /// 激活器，用于管理视图模型的激活状态
        /// </summary>
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public new Guid Id => _id;
        
        /// <summary>
        /// 内容标识符
        /// </summary>
        public string ContentId => _id.ToString();
        
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
        /// 关闭命令
        /// </summary>
        public ICommand CloseCommand => ReactiveCommand.Create(() => { });
        
        /// <summary>
        /// 图标源
        /// </summary>
        public virtual Uri? IconSource => null;
        
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set => this.RaiseAndSetIfChanged(ref _isVisible, value);
        }
        
        /// <summary>
        /// 工具提示
        /// </summary>
        public string ToolTip
        {
            get => _toolTip;
            set => this.RaiseAndSetIfChanged(ref _toolTip, value);
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
        /// 是否应该在启动时重新打开
        /// </summary>
        public virtual bool ShouldReopenOnStart => false;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        protected Tool()
        {
            // 设置默认属性
            CanClose = true;
            CanFloat = true;
            CanPin = true;
        }
        
        /// <summary>
        /// 首选位置，子类必须实现
        /// </summary>
        public abstract PaneLocation PreferredLocation { get; }
        
        /// <summary>
        /// 首选宽度
        /// </summary>
        public virtual double PreferredWidth => 200.0;
        
        /// <summary>
        /// 首选高度
        /// </summary>
        public virtual double PreferredHeight => 200.0;
        
        /// <summary>
        /// 加载状态
        /// </summary>
        /// <param name="reader">二进制读取器</param>
        public virtual void LoadState(BinaryReader reader)
        {
            // 默认实现为空
        }
        
        /// <summary>
        /// 保存状态
        /// </summary>
        /// <param name="writer">二进制写入器</param>
        public virtual void SaveState(BinaryWriter writer)
        {
            // 默认实现为空
        }
        
        /// <summary>
        /// 初始化工具（在MEF注入完成后调用）
        /// </summary>
        public virtual void Initialize()
        {
            // 默认实现为空，子类可以重写此方法进行初始化
        }
    }
}