using Gemini.Avalonia.Modules.UndoRedo;

namespace Gemini.Avalonia.Framework
{
    /// <summary>
    /// 文档接口，表示可以在主文档区域显示的内容
    /// </summary>
    public interface IDocument : ILayoutItem
    {
        /// <summary>
        /// 撤销重做管理器
        /// </summary>
        IUndoRedoManager UndoRedoManager { get; }
    }
}