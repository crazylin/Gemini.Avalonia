using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraUI.Framework.Modules
{
    /// <summary>
    /// 模块加载器接口，定义模块加载的核心操作
    /// </summary>
    public interface IModuleLoader
    {
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
        /// 加载模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>加载任务</returns>
        Task<bool> LoadModuleAsync(string moduleName);
        
        /// <summary>
        /// 卸载模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>卸载任务</returns>
        Task<bool> UnloadModuleAsync(string moduleName);
        
        /// <summary>
        /// 检查模块是否已加载
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>是否已加载</returns>
        bool IsModuleLoaded(string moduleName);
        
        /// <summary>
        /// 获取模块实例
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>模块实例</returns>
        T? GetModule<T>(string moduleName) where T : IModule;
    }
}
