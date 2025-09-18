using System;
using System.Collections.Generic;
using System.Linq;
using AuroraUI.Framework.Modules.ModuleLoadingStrategies;
using AuroraUI.Framework.Logging;

namespace AuroraUI.Framework.Modules
{
    /// <summary>
    /// 模块加载策略工厂，负责管理和选择合适的模块加载策略
    /// </summary>
    public class ModuleLoadingStrategyFactory
    {
        private readonly List<IModuleLoadingStrategy> _strategies = new();
        
        /// <summary>
        /// 构造函数，注册默认的加载策略
        /// </summary>
        public ModuleLoadingStrategyFactory()
        {
            RegisterDefaultStrategies();
        }
        
        /// <summary>
        /// 注册加载策略
        /// </summary>
        /// <param name="strategy">加载策略</param>
        public void RegisterStrategy(IModuleLoadingStrategy strategy)
        {
            if (strategy != null && !_strategies.Contains(strategy))
            {
                _strategies.Add(strategy);
                LogManager.Debug("ModuleLoadingStrategyFactory", $"已注册模块加载策略: {strategy.GetType().Name}");
            }
        }
        
        /// <summary>
        /// 获取适合指定模块的加载策略
        /// </summary>
        /// <param name="metadata">模块元数据</param>
        /// <returns>加载策略，如果没有找到则返回null</returns>
        public IModuleLoadingStrategy? GetStrategy(ModuleMetadata metadata)
        {
            var strategy = _strategies.FirstOrDefault(s => s.CanLoad(metadata));
            
            if (strategy == null)
            {
                LogManager.Warning("ModuleLoadingStrategyFactory", 
                    $"没有找到适合模块 {metadata.Name} (类型: {metadata.ModuleType?.Name}) 的加载策略");
            }
            else
            {
                LogManager.Debug("ModuleLoadingStrategyFactory", 
                    $"为模块 {metadata.Name} 选择了加载策略: {strategy.GetType().Name}");
            }
            
            return strategy;
        }
        
        /// <summary>
        /// 获取所有已注册的策略
        /// </summary>
        /// <returns>策略列表</returns>
        public IReadOnlyList<IModuleLoadingStrategy> GetAllStrategies()
        {
            return _strategies.AsReadOnly();
        }
        
        /// <summary>
        /// 移除指定的加载策略
        /// </summary>
        /// <param name="strategy">要移除的策略</param>
        /// <returns>是否成功移除</returns>
        public bool UnregisterStrategy(IModuleLoadingStrategy strategy)
        {
            var removed = _strategies.Remove(strategy);
            if (removed)
            {
                LogManager.Debug("ModuleLoadingStrategyFactory", $"已移除模块加载策略: {strategy.GetType().Name}");
            }
            return removed;
        }
        
        /// <summary>
        /// 移除指定类型的加载策略
        /// </summary>
        /// <typeparam name="T">策略类型</typeparam>
        /// <returns>移除的策略数量</returns>
        public int UnregisterStrategy<T>() where T : IModuleLoadingStrategy
        {
            var strategiesToRemove = _strategies.OfType<T>().ToList();
            int removedCount = 0;
            
            foreach (var strategy in strategiesToRemove)
            {
                if (_strategies.Remove(strategy))
                {
                    removedCount++;
                    LogManager.Debug("ModuleLoadingStrategyFactory", $"已移除模块加载策略: {strategy.GetType().Name}");
                }
            }
            
            return removedCount;
        }
        
        /// <summary>
        /// 清除所有策略
        /// </summary>
        public void ClearStrategies()
        {
            var count = _strategies.Count;
            _strategies.Clear();
            LogManager.Info("ModuleLoadingStrategyFactory", $"已清除所有 {count} 个模块加载策略");
        }
        
        /// <summary>
        /// 检查是否有可以处理指定模块的策略
        /// </summary>
        /// <param name="metadata">模块元数据</param>
        /// <returns>是否有合适的策略</returns>
        public bool HasSuitableStrategy(ModuleMetadata metadata)
        {
            return _strategies.Any(s => s.CanLoad(metadata));
        }
        
        /// <summary>
        /// 注册默认的加载策略
        /// </summary>
        private void RegisterDefaultStrategies()
        {
            // 注册延迟模块加载策略（优先级较高）
            RegisterStrategy(new LazyModuleLoadingStrategy());
            
            // 注册标准模块加载策略（默认策略）
            RegisterStrategy(new StandardModuleLoadingStrategy());
            
            LogManager.Info("ModuleLoadingStrategyFactory", "默认模块加载策略注册完成");
        }
    }
}
