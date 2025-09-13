namespace Gemini.Avalonia.Modules.UndoRedo
{
    /// <summary>
    /// 可撤销动作接口
    /// </summary>
    public interface IUndoableAction
    {
        /// <summary>
        /// 动作名称
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// 执行动作
        /// </summary>
        void Execute();
        
        /// <summary>
        /// 撤销动作
        /// </summary>
        void Undo();
    }
}