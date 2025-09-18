using System;
using System.Collections.Generic;

namespace AuroraUI.Modules.UndoRedo
{
    /// <summary>
    /// 撤销重做管理器接口
    /// </summary>
    public interface IUndoRedoManager
    {
        /// <summary>
        /// 撤销栈变更事件
        /// </summary>
        event EventHandler UndoRedoStackChanged;
        
        /// <summary>
        /// 撤销栈
        /// </summary>
        IEnumerable<IUndoableAction> UndoStack { get; }
        
        /// <summary>
        /// 重做栈
        /// </summary>
        IEnumerable<IUndoableAction> RedoStack { get; }
        
        /// <summary>
        /// 是否可以撤销
        /// </summary>
        bool CanUndo { get; }
        
        /// <summary>
        /// 是否可以重做
        /// </summary>
        bool CanRedo { get; }
        
        /// <summary>
        /// 执行动作
        /// </summary>
        /// <param name="action">可撤销动作</param>
        void ExecuteAction(IUndoableAction action);
        
        /// <summary>
        /// 撤销
        /// </summary>
        void Undo();
        
        /// <summary>
        /// 重做
        /// </summary>
        void Redo();
        
        /// <summary>
        /// 清空栈
        /// </summary>
        void Clear();
    }
}