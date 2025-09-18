using System;
using System.Threading.Tasks;
using AuroraUI.Framework.Logging;
using AuroraUI.Framework.Performance;

namespace AuroraUI.Framework.Modules.ModuleLoadingStrategies
{
    /// <summary>
    /// 延迟模块加载策略
    /// </summary>
    public class LazyModuleLoadingStrategy : IModuleLoadingStrategy
    {
        /// <summary>
        /// 检查是否可以加载指定模块
        /// </summary>
        /// <param name="metadata">模块元数据</param>
        /// <returns>是否可以加载</returns>
        public bool CanLoad(ModuleMetadata metadata)
        {
            return metadata.AllowLazyLoading && metadata.ModuleType != null;
        }
        
        /// <summary>
        /// 加载模块
        /// </summary>
        /// <param name="metadata">模块元数据</param>
        /// <returns>加载后的模块实例</returns>
        public async Task<IModule?> LoadAsync(ModuleMetadata metadata)
        {
            if (metadata.ModuleType == null)
            {
                LogManager.Error("LazyModuleLoadingStrategy", $"延迟模块 {metadata.Name} 的类型为空");
                return null;
            }
            
            try
            {
                var loadTimer = $"加载延迟模块_{metadata.Name}";
                PerformanceMonitor.StartTimer(loadTimer);
                
                LogManager.Info("LazyModuleLoadingStrategy", $"开始加载延迟模块: {metadata.Name}");
                
                // 创建模块实例
                IModule? moduleInstance = null;
                PerformanceMonitor.Measure($"创建延迟模块实例_{metadata.Name}", () => 
                {
                    moduleInstance = Activator.CreateInstance(metadata.ModuleType) as IModule;
                });
                
                if (moduleInstance == null)
                {
                    LogManager.Error("LazyModuleLoadingStrategy", $"无法创建延迟模块实例: {metadata.Name}");
                    return null;
                }
                
                // 如果是延迟模块，检查是否应该立即加载
                if (moduleInstance is ILazyModule lazyModule)
                {
                    if (!lazyModule.ShouldLoad())
                    {
                        LogManager.Debug("LazyModuleLoadingStrategy", $"延迟模块 {metadata.Name} 暂时不需要加载");
                        metadata.Instance = moduleInstance;
                        metadata.IsLoaded = true;
                        metadata.IsInitialized = false;
                        PerformanceMonitor.StopTimer(loadTimer);
                        return moduleInstance;
                    }
                    
                    // 异步加载延迟模块
                    await lazyModule.LoadAsync();
                }
                else
                {
                    // 普通模块，执行标准初始化流程
                    PerformanceMonitor.Measure($"预初始化_{metadata.Name}",
                        () => moduleInstance.PreInitialize());

                    PerformanceMonitor.Measure($"初始化_{metadata.Name}",
                        () => moduleInstance.Initialize());
                }
                
                // 标记模块状态
                metadata.Instance = moduleInstance;
                metadata.IsLoaded = true;
                metadata.IsInitialized = true;
                
                PerformanceMonitor.StopTimer(loadTimer);
                LogManager.Info("LazyModuleLoadingStrategy", $"延迟模块加载完成: {metadata.Name}");
                
                return moduleInstance;
            }
            catch (Exception ex)
            {
                LogManager.Error("LazyModuleLoadingStrategy", $"加载延迟模块 {metadata.Name} 失败: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// 卸载模块
        /// </summary>
        /// <param name="module">模块实例</param>
        /// <returns>卸载任务</returns>
        public async Task UnloadAsync(IModule module)
        {
            if (module is ILazyModule lazyModule)
            {
                try
                {
                    LogManager.Info("LazyModuleLoadingStrategy", $"开始卸载延迟模块: {module.GetType().Name}");
                    
                    await lazyModule.UnloadAsync();
                    
                    LogManager.Info("LazyModuleLoadingStrategy", $"延迟模块卸载完成: {module.GetType().Name}");
                }
                catch (Exception ex)
                {
                    LogManager.Error("LazyModuleLoadingStrategy", $"卸载延迟模块 {module.GetType().Name} 失败: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
