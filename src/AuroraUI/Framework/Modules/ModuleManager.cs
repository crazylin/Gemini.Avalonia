using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuroraUI.Framework.Logging;

namespace AuroraUI.Framework.Modules
{
    /// <summary>
    /// 模块管理器实现（简化版本，主要职责是提供便利的API）
    /// 实际的模块加载逻辑已经委托给 ModuleLoadingService
    /// </summary>
    public class ModuleManager : IModuleManager
    {
        private readonly ModuleLoadingService _moduleLoadingService;
        
        /// <summary>
        /// 所有已注册的模块
        /// </summary>
        public IReadOnlyCollection<ModuleMetadata> RegisteredModules => 
            _moduleLoadingService.RegisteredModules;
        
        /// <summary>
        /// 所有已加载的模块
        /// </summary>
        public IReadOnlyCollection<ModuleMetadata> LoadedModules => 
            _moduleLoadingService.LoadedModules;
        
        /// <summary>
        /// 模块加载事件
        /// </summary>
        public event EventHandler<ModuleEventArgs>? ModuleLoaded
        {
            add => _moduleLoadingService.ModuleLoaded += value;
            remove => _moduleLoadingService.ModuleLoaded -= value;
        }
        
        /// <summary>
        /// 模块卸载事件
        /// </summary>
        public event EventHandler<ModuleEventArgs>? ModuleUnloaded
        {
            add => _moduleLoadingService.ModuleUnloaded += value;
            remove => _moduleLoadingService.ModuleUnloaded -= value;
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public ModuleManager()
        {
            _moduleLoadingService = new ModuleLoadingService();
            LogManager.Info("ModuleManager", "模块管理器已初始化");
        }
        
        /// <summary>
        /// 注册模块
        /// </summary>
        /// <param name="moduleType">模块类型</param>
        /// <param name="metadata">模块元数据</param>
        public void RegisterModule(Type moduleType, ModuleMetadata? metadata = null)
        {
            _moduleLoadingService.RegisterModule(moduleType, metadata);
        }
        
        /// <summary>
        /// 注册模块
        /// </summary>
        /// <typeparam name="T">模块类型</typeparam>
        /// <param name="metadata">模块元数据</param>
        public void RegisterModule<T>(ModuleMetadata? metadata = null) where T : IModule
        {
            _moduleLoadingService.RegisterModule<T>(metadata);
        }
        
        /// <summary>
        /// 加载核心模块
        /// </summary>
        /// <returns>加载任务</returns>
        public async Task LoadCoreModulesAsync()
        {
            LogManager.Info("ModuleManager", "开始加载核心模块");
            await _moduleLoadingService.LoadModulesByCategoryAsync(ModuleCategory.Core);
            LogManager.Info("ModuleManager", "核心模块加载完成");
        }
        
        /// <summary>
        /// 按需加载模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>加载任务</returns>
        public async Task<bool> LoadModuleAsync(string moduleName)
        {
            return await _moduleLoadingService.LoadModuleAsync(moduleName);
        }
        
        /// <summary>
        /// 加载指定分类的模块
        /// </summary>
        /// <param name="category">模块分类</param>
        /// <returns>加载任务</returns>
        public async Task LoadModulesByCategoryAsync(ModuleCategory category)
        {
            await _moduleLoadingService.LoadModulesByCategoryAsync(category);
        }
        
        /// <summary>
        /// 卸载模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>卸载任务</returns>
        public async Task<bool> UnloadModuleAsync(string moduleName)
        {
            return await _moduleLoadingService.UnloadModuleAsync(moduleName);
        }
        
        /// <summary>
        /// 获取模块实例
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>模块实例</returns>
        public T? GetModule<T>(string moduleName) where T : IModule
        {
            return _moduleLoadingService.GetModule<T>(moduleName);
        }
        
        /// <summary>
        /// 检查模块是否已加载
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>是否已加载</returns>
        public bool IsModuleLoaded(string moduleName)
        {
            return _moduleLoadingService.IsModuleLoaded(moduleName);
        }
    }
}
