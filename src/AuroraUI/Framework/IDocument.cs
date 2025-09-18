using System.Threading.Tasks;
using AuroraUI.Modules.UndoRedo;

namespace AuroraUI.Framework
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
        
        /// <summary>
        /// 尝试关闭文档，实现类可以添加保存确认等逻辑
        /// </summary>
        /// <returns>关闭任务，如果用户取消则抛出OperationCanceledException</returns>
        Task TryCloseAsync();
    }
}