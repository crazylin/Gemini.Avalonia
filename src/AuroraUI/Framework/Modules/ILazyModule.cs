using System.Threading.Tasks;

namespace AuroraUI.Framework.Modules
{
    /// <summary>
    /// 支持延迟加载的模块接口
    /// </summary>
    public interface ILazyModule : IModule
    {
        /// <summary>
        /// 模块元数据
        /// </summary>
        ModuleMetadata Metadata { get; }
        
        /// <summary>
        /// 检查是否应该加载此模块
        /// </summary>
        /// <returns>如果应该加载返回true</returns>
        bool ShouldLoad();
        
        /// <summary>
        /// 异步加载模块
        /// </summary>
        /// <returns>加载任务</returns>
        Task LoadAsync();
        
        /// <summary>
        /// 卸载模块
        /// </summary>
        /// <returns>卸载任务</returns>
        Task UnloadAsync();
        
        /// <summary>
        /// 检查模块是否已加载
        /// </summary>
        bool IsLoaded { get; }
    }
}
