using System.Threading.Tasks;

namespace AuroraUI.Framework.Modules
{
    /// <summary>
    /// 模块加载策略接口
    /// </summary>
    public interface IModuleLoadingStrategy
    {
        /// <summary>
        /// 检查是否可以加载指定模块
        /// </summary>
        /// <param name="metadata">模块元数据</param>
        /// <returns>是否可以加载</returns>
        bool CanLoad(ModuleMetadata metadata);
        
        /// <summary>
        /// 加载模块
        /// </summary>
        /// <param name="metadata">模块元数据</param>
        /// <returns>加载后的模块实例</returns>
        Task<IModule?> LoadAsync(ModuleMetadata metadata);
        
        /// <summary>
        /// 卸载模块
        /// </summary>
        /// <param name="module">模块实例</param>
        /// <returns>卸载任务</returns>
        Task UnloadAsync(IModule module);
    }
}
