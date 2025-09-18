using System;
using System.Threading.Tasks;
using AuroraUI.Framework.Logging;
using AuroraUI.Framework.Performance;

namespace AuroraUI.Framework.Modules.ModuleLoadingStrategies
{
    /// <summary>
    /// 标准模块加载策略
    /// </summary>
    public class StandardModuleLoadingStrategy : IModuleLoadingStrategy
    {
        /// <summary>
        /// 检查是否可以加载指定模块
        /// </summary>
        /// <param name="metadata">模块元数据</param>
        /// <returns>是否可以加载</returns>
        public bool CanLoad(ModuleMetadata metadata)
        {
            return metadata.ModuleType != null && 
                   typeof(IModule).IsAssignableFrom(metadata.ModuleType) &&
                   !typeof(ILazyModule).IsAssignableFrom(metadata.ModuleType);
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
                LogManager.Error("StandardModuleLoadingStrategy", $"模块 {metadata.Name} 的类型为空");
                return null;
            }
            
            try
            {
                var loadTimer = $"加载标准模块_{metadata.Name}";
                PerformanceMonitor.StartTimer(loadTimer);
                
                LogManager.Info("StandardModuleLoadingStrategy", $"开始加载标准模块: {metadata.Name}");
                
                // 创建模块实例
                IModule? moduleInstance = null;
                PerformanceMonitor.Measure($"创建模块实例_{metadata.Name}", () => 
                {
                    moduleInstance = Activator.CreateInstance(metadata.ModuleType) as IModule;
                });
                
                if (moduleInstance == null)
                {
                    LogManager.Error("StandardModuleLoadingStrategy", $"无法创建模块实例: {metadata.Name}");
                    return null;
                }
                
                // 预初始化
                PerformanceMonitor.Measure($"预初始化_{metadata.Name}", 
                    () => moduleInstance.PreInitialize());
                
                // 初始化
                PerformanceMonitor.Measure($"初始化_{metadata.Name}", 
                    () => moduleInstance.Initialize());
                
                // 标记模块状态
                metadata.Instance = moduleInstance;
                metadata.IsLoaded = true;
                metadata.IsInitialized = true;
                
                PerformanceMonitor.StopTimer(loadTimer);
                LogManager.Info("StandardModuleLoadingStrategy", $"标准模块加载完成: {metadata.Name}");
                
                return moduleInstance;
            }
            catch (Exception ex)
            {
                LogManager.Error("StandardModuleLoadingStrategy", $"加载标准模块 {metadata.Name} 失败: {ex.Message}");
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
            try
            {
                LogManager.Info("StandardModuleLoadingStrategy", $"开始卸载标准模块: {module.GetType().Name}");
                
                // 标准模块没有特殊的卸载逻辑，这里可以扩展
                await Task.CompletedTask;
                
                LogManager.Info("StandardModuleLoadingStrategy", $"标准模块卸载完成: {module.GetType().Name}");
            }
            catch (Exception ex)
            {
                LogManager.Error("StandardModuleLoadingStrategy", $"卸载标准模块 {module.GetType().Name} 失败: {ex.Message}");
                throw;
            }
        }
    }
}
