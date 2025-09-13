using System;
using System.Collections.Generic;
using System.Linq;

namespace Gemini.Avalonia.Modules.UndoRedo
{
    /// <summary>
    /// 撤销重做管理器的默认实现
    /// </summary>
    public class UndoRedoManager : IUndoRedoManager
    {
        private readonly Stack<IUndoableAction> _undoStack = new();
        private readonly Stack<IUndoableAction> _redoStack = new();
        
        /// <summary>
        /// 撤销重做栈变更事件
        /// </summary>
        public event EventHandler? UndoRedoStackChanged;
        
        /// <summary>
        /// 撤销栈
        /// </summary>
        public IEnumerable<IUndoableAction> UndoStack => _undoStack.AsEnumerable();
        
        /// <summary>
        /// 重做栈
        /// </summary>
        public IEnumerable<IUndoableAction> RedoStack => _redoStack.AsEnumerable();
        
        /// <summary>
        /// 是否可以撤销
        /// </summary>
        public bool CanUndo => _undoStack.Count > 0;
        
        /// <summary>
        /// 是否可以重做
        /// </summary>
        public bool CanRedo => _redoStack.Count > 0;
        
        /// <summary>
        /// 执行动作
        /// </summary>
        /// <param name="action">可撤销动作</param>
        public void ExecuteAction(IUndoableAction action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
                
            // 执行动作
            action.Execute();
            
            // 添加到撤销栈
            _undoStack.Push(action);
            
            // 清空重做栈
            _redoStack.Clear();
            
            // 触发事件
            OnUndoRedoStackChanged();
        }
        
        /// <summary>
        /// 撤销
        /// </summary>
        public void Undo()
        {
            if (!CanUndo)
                return;
                
            var action = _undoStack.Pop();
            action.Undo();
            
            _redoStack.Push(action);
            
            OnUndoRedoStackChanged();
        }
        
        /// <summary>
        /// 重做
        /// </summary>
        public void Redo()
        {
            if (!CanRedo)
                return;
                
            var action = _redoStack.Pop();
            action.Execute();
            
            _undoStack.Push(action);
            
            OnUndoRedoStackChanged();
        }
        
        /// <summary>
        /// 清空栈
        /// </summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            
            OnUndoRedoStackChanged();
        }
        
        /// <summary>
        /// 触发撤销重做栈变更事件
        /// </summary>
        protected virtual void OnUndoRedoStackChanged()
        {
            UndoRedoStackChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}