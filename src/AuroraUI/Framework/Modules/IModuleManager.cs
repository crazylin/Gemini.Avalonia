using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraUI.Framework.Modules
{
    /// <summary>
    /// 模块管理器接口
    /// </summary>
    public interface IModuleManager
    {
        /// <summary>
        /// 所有已注册的模块
        /// </summary>
        IReadOnlyCollection<ModuleMetadata> RegisteredModules { get; }
        
        /// <summary>
        /// 所有已加载的模块
        /// </summary>
        IReadOnlyCollection<ModuleMetadata> LoadedModules { get; }
        
        /// <summary>
        /// 注册模块
        /// </summary>
        /// <param name="moduleType">模块类型</param>
        /// <param name="metadata">模块元数据</param>
        void RegisterModule(Type moduleType, ModuleMetadata? metadata = null);
        
        /// <summary>
        /// 注册模块
        /// </summary>
        /// <typeparam name="T">模块类型</typeparam>
        /// <param name="metadata">模块元数据</param>
        void RegisterModule<T>(ModuleMetadata? metadata = null) where T : IModule;
        
        /// <summary>
        /// 加载核心模块
        /// </summary>
        /// <returns>加载任务</returns>
        Task LoadCoreModulesAsync();
        
        /// <summary>
        /// 按需加载模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>加载任务</returns>
        Task<bool> LoadModuleAsync(string moduleName);
        
        /// <summary>
        /// 加载指定分类的模块
        /// </summary>
        /// <param name="category">模块分类</param>
        /// <returns>加载任务</returns>
        Task LoadModulesByCategoryAsync(ModuleCategory category);
        
        /// <summary>
        /// 卸载模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>卸载任务</returns>
        Task<bool> UnloadModuleAsync(string moduleName);
        
        /// <summary>
        /// 获取模块实例
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>模块实例</returns>
        T? GetModule<T>(string moduleName) where T : IModule;
        
        /// <summary>
        /// 检查模块是否已加载
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>是否已加载</returns>
        bool IsModuleLoaded(string moduleName);
        
        /// <summary>
        /// 模块加载事件
        /// </summary>
        event EventHandler<ModuleEventArgs>? ModuleLoaded;
        
        /// <summary>
        /// 模块卸载事件
        /// </summary>
        event EventHandler<ModuleEventArgs>? ModuleUnloaded;
    }
    
    /// <summary>
    /// 模块事件参数
    /// </summary>
    public class ModuleEventArgs : EventArgs
    {
        public ModuleMetadata ModuleMetadata { get; }
        
        public ModuleEventArgs(ModuleMetadata moduleMetadata)
        {
            ModuleMetadata = moduleMetadata;
        }
    }
}
