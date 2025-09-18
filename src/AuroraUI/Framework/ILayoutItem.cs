using System;
using System.IO;
using System.Windows.Input;
using ReactiveUI;

namespace AuroraUI.Framework
{
    /// <summary>
    /// 布局项接口，所有可在Shell中显示的项目的基础接口
    /// </summary>
    public interface ILayoutItem : IReactiveObject, IActivatableViewModel
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        Guid Id { get; }
        
        /// <summary>
        /// 内容标识符，用于布局持久化
        /// </summary>
        string ContentId { get; }
        
        /// <summary>
        /// 显示名称
        /// </summary>
        string DisplayName { get; set; }
        
        /// <summary>
        /// 关闭命令
        /// </summary>
        ICommand CloseCommand { get; }
        
        /// <summary>
        /// 图标源
        /// </summary>
        Uri? IconSource { get; }
        
        /// <summary>
        /// 工具提示
        /// </summary>
        string ToolTip { get; set; }
        
        /// <summary>
        /// 是否被选中
        /// </summary>
        bool IsSelected { get; set; }
        
        /// <summary>
        /// 是否应该在启动时重新打开
        /// </summary>
        bool ShouldReopenOnStart { get; }
        
        /// <summary>
        /// 加载状态
        /// </summary>
        /// <param name="reader">二进制读取器</param>
        void LoadState(BinaryReader reader);
        
        /// <summary>
        /// 保存状态
        /// </summary>
        /// <param name="writer">二进制写入器</param>
        void SaveState(BinaryWriter writer);
    }
}