using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gemini.Avalonia.Framework.Logging;
using Gemini.Avalonia.Framework.Modules.ModuleLoadingStrategies;
using Gemini.Avalonia.Framework.Performance;

namespace Gemini.Avalonia.Framework.Modules
{
    /// <summary>
    /// 统一的模块加载服务，负责协调模块的注册、加载和卸载
    /// </summary>
    public class ModuleLoadingService : IModuleLoader
    {
        private readonly ConcurrentDictionary<string, ModuleMetadata> _registeredModules = new();
        private readonly ConcurrentDictionary<string, ModuleMetadata> _loadedModules = new();
        private readonly ModuleDependencyResolver _dependencyResolver = new();
        private readonly ModuleLoadingStrategyFactory _strategyFactory = new();
        private readonly ModuleLoadingEventMonitor _eventMonitor = new();
        private readonly SemaphoreSlim _loadingSemaphore = new(1, 1);
        
        /// <summary>
        /// 模块加载事件
        /// </summary>
        public event EventHandler<ModuleEventArgs>? ModuleLoaded;
        
        /// <summary>
        /// 模块卸载事件
        /// </summary>
        public event EventHandler<ModuleEventArgs>? ModuleUnloaded;
        
        /// <summary>
        /// 事件监控器
        /// </summary>
        public ModuleLoadingEventMonitor EventMonitor => _eventMonitor;
        
        /// <summary>
        /// 所有已注册的模块
        /// </summary>
        public IReadOnlyCollection<ModuleMetadata> RegisteredModules => 
            _registeredModules.Values.ToList().AsReadOnly();
        
        /// <summary>
        /// 所有已加载的模块
        /// </summary>
        public IReadOnlyCollection<ModuleMetadata> LoadedModules => 
            _loadedModules.Values.ToList().AsReadOnly();
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public ModuleLoadingService()
        {
            // 策略工厂会自动注册默认策略
            LogManager.Info("ModuleLoadingService", "模块加载服务已初始化");
        }
        
        /// <summary>
        /// 注册自定义的模块加载策略
        /// </summary>
        /// <param name="strategy">加载策略</param>
        public void RegisterLoadingStrategy(IModuleLoadingStrategy strategy)
        {
            _strategyFactory.RegisterStrategy(strategy);
        }
        
        /// <summary>
        /// 注册模块
        /// </summary>
        /// <param name="moduleType">模块类型</param>
        /// <param name="metadata">模块元数据</param>
        public void RegisterModule(Type moduleType, ModuleMetadata? metadata = null)
        {
            if (!typeof(IModule).IsAssignableFrom(moduleType))
            {
                throw new ArgumentException($"Type {moduleType.Name} must implement IModule interface");
            }
            
            metadata ??= CreateDefaultMetadata(moduleType);
            metadata.ModuleType = moduleType;
            
            if (string.IsNullOrEmpty(metadata.Name))
            {
                metadata.Name = moduleType.Name;
            }
            
            _registeredModules.TryAdd(metadata.Name, metadata);
            _dependencyResolver.RegisterModule(metadata);
            
            // 触发模块注册事件
            _ = Task.Run(async () => await _eventMonitor.TriggerEventAsync(
                ModuleLoadingEventType.ModuleRegistered, metadata));
            
            LogManager.Info("ModuleLoadingService", $"模块已注册: {metadata.Name} ({metadata.Category})");
        }
        
        /// <summary>
        /// 注册模块
        /// </summary>
        /// <typeparam name="T">模块类型</typeparam>
        /// <param name="metadata">模块元数据</param>
        public void RegisterModule<T>(ModuleMetadata? metadata = null) where T : IModule
        {
            RegisterModule(typeof(T), metadata);
        }
        
        /// <summary>
        /// 加载模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>加载任务</returns>
        public async Task<bool> LoadModuleAsync(string moduleName)
        {
            if (_loadedModules.ContainsKey(moduleName))
            {
                LogManager.Debug("ModuleLoadingService", $"模块 {moduleName} 已经加载");
                return true;
            }
            
            if (!_registeredModules.TryGetValue(moduleName, out var moduleMetadata))
            {
                LogManager.Warning("ModuleLoadingService", $"未找到模块: {moduleName}");
                return false;
            }
            
            await _loadingSemaphore.WaitAsync();
            try
            {
                return await LoadModuleInternalAsync(moduleMetadata);
            }
            finally
            {
                _loadingSemaphore.Release();
            }
        }
        
        /// <summary>
        /// 加载指定分类的模块
        /// </summary>
        /// <param name="category">模块分类</param>
        /// <returns>加载任务</returns>
        public async Task LoadModulesByCategoryAsync(ModuleCategory category)
        {
            var modulesToLoad = _registeredModules.Values
                .Where(m => m.Category == category && !m.IsLoaded)
                .ToList();
            
            if (modulesToLoad.Count == 0)
            {
                LogManager.Debug("ModuleLoadingService", $"没有需要加载的 {category} 类型模块");
                return;
            }
            
            LogManager.Info("ModuleLoadingService", $"开始加载 {modulesToLoad.Count} 个 {category} 类型模块");
            
            // 使用依赖解析器获取正确的加载顺序
            var orderedModules = _dependencyResolver.GetLoadOrder(modulesToLoad);
            
            foreach (var moduleMetadata in orderedModules)
            {
                await LoadModuleAsync(moduleMetadata.Name);
            }
            
            LogManager.Info("ModuleLoadingService", $"{category} 类型模块加载完成");
        }
        
        /// <summary>
        /// 卸载模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>卸载任务</returns>
        public async Task<bool> UnloadModuleAsync(string moduleName)
        {
            if (!_loadedModules.TryGetValue(moduleName, out var moduleMetadata))
            {
                LogManager.Warning("ModuleLoadingService", $"模块 {moduleName} 未加载");
                return false;
            }
            
            await _loadingSemaphore.WaitAsync();
            try
            {
                return await UnloadModuleInternalAsync(moduleMetadata);
            }
            finally
            {
                _loadingSemaphore.Release();
            }
        }
        
        /// <summary>
        /// 检查模块是否已加载
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>是否已加载</returns>
        public bool IsModuleLoaded(string moduleName)
        {
            return _loadedModules.ContainsKey(moduleName);
        }
        
        /// <summary>
        /// 获取模块实例
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>模块实例</returns>
        public T? GetModule<T>(string moduleName) where T : IModule
        {
            if (_loadedModules.TryGetValue(moduleName, out var moduleMetadata))
            {
                return (T?)moduleMetadata.Instance;
            }
            return default;
        }
        
        /// <summary>
        /// 内部加载模块实现
        /// </summary>
        /// <param name="moduleMetadata">模块元数据</param>
        /// <returns>加载成功返回true</returns>
        private async Task<bool> LoadModuleInternalAsync(ModuleMetadata moduleMetadata)
        {
            if (moduleMetadata.IsLoaded)
            {
                return true;
            }
            
            try
            {
                // 触发模块开始加载事件
                await _eventMonitor.TriggerEventAsync(ModuleLoadingEventType.ModuleLoadStarted, moduleMetadata);
                
                // 检查依赖是否满足
                var loadedModuleNames = _loadedModules.Keys.ToHashSet();
                if (!_dependencyResolver.AreDependenciesLoaded(moduleMetadata.Name, loadedModuleNames))
                {
                    // 先加载依赖模块
                    var dependencies = _dependencyResolver.GetDirectDependencies(moduleMetadata.Name);
                    foreach (var dependency in dependencies)
                    {
                        if (!await LoadModuleAsync(dependency))
                        {
                            var error = new InvalidOperationException($"依赖模块 {dependency} 加载失败");
                            await _eventMonitor.TriggerEventAsync(ModuleLoadingEventType.ModuleLoadFailed, moduleMetadata, error);
                            LogManager.Error("ModuleLoadingService", 
                                $"加载模块 {moduleMetadata.Name} 失败：依赖模块 {dependency} 加载失败");
                            return false;
                        }
                    }
                }
                
                // 选择合适的加载策略
                var strategy = _strategyFactory.GetStrategy(moduleMetadata);
                if (strategy == null)
                {
                    var error = new InvalidOperationException($"没有找到适合模块 {moduleMetadata.Name} 的加载策略");
                    await _eventMonitor.TriggerEventAsync(ModuleLoadingEventType.ModuleLoadFailed, moduleMetadata, error);
                    LogManager.Error("ModuleLoadingService", 
                        $"没有找到适合模块 {moduleMetadata.Name} 的加载策略");
                    return false;
                }
                
                // 使用策略加载模块
                var moduleInstance = await strategy.LoadAsync(moduleMetadata);
                if (moduleInstance == null)
                {
                    var error = new InvalidOperationException($"模块 {moduleMetadata.Name} 加载失败");
                    await _eventMonitor.TriggerEventAsync(ModuleLoadingEventType.ModuleLoadFailed, moduleMetadata, error);
                    LogManager.Error("ModuleLoadingService", $"模块 {moduleMetadata.Name} 加载失败");
                    return false;
                }
                
                // 更新状态
                _loadedModules.TryAdd(moduleMetadata.Name, moduleMetadata);
                
                // 触发模块加载成功事件
                await _eventMonitor.TriggerEventAsync(ModuleLoadingEventType.ModuleLoadSucceeded, moduleMetadata);
                
                // 触发兼容性事件
                ModuleLoaded?.Invoke(this, new ModuleEventArgs(moduleMetadata));
                
                return true;
            }
            catch (Exception ex)
            {
                // 触发模块加载失败事件
                await _eventMonitor.TriggerEventAsync(ModuleLoadingEventType.ModuleLoadFailed, moduleMetadata, ex);
                
                LogManager.Error("ModuleLoadingService", $"加载模块 {moduleMetadata.Name} 失败: {ex.Message}");
                moduleMetadata.IsLoaded = false;
                moduleMetadata.IsInitialized = false;
                moduleMetadata.Instance = null;
                return false;
            }
        }
        
        /// <summary>
        /// 内部卸载模块实现
        /// </summary>
        /// <param name="moduleMetadata">模块元数据</param>
        /// <returns>卸载成功返回true</returns>
        private async Task<bool> UnloadModuleInternalAsync(ModuleMetadata moduleMetadata)
        {
            try
            {
                // 触发模块开始卸载事件
                await _eventMonitor.TriggerEventAsync(ModuleLoadingEventType.ModuleUnloadStarted, moduleMetadata);
                
                if (moduleMetadata.Instance != null)
                {
                    // 选择合适的卸载策略
                    var strategy = _strategyFactory.GetStrategy(moduleMetadata);
                    if (strategy != null)
                    {
                        await strategy.UnloadAsync(moduleMetadata.Instance);
                    }
                }
                
                // 更新状态
                moduleMetadata.IsLoaded = false;
                moduleMetadata.IsInitialized = false;
                moduleMetadata.Instance = null;
                
                _loadedModules.TryRemove(moduleMetadata.Name, out _);
                
                // 触发模块卸载成功事件
                await _eventMonitor.TriggerEventAsync(ModuleLoadingEventType.ModuleUnloadSucceeded, moduleMetadata);
                
                // 触发兼容性事件
                ModuleUnloaded?.Invoke(this, new ModuleEventArgs(moduleMetadata));
                
                LogManager.Info("ModuleLoadingService", $"模块已卸载: {moduleMetadata.Name}");
                return true;
            }
            catch (Exception ex)
            {
                // 触发模块卸载失败事件
                await _eventMonitor.TriggerEventAsync(ModuleLoadingEventType.ModuleUnloadFailed, moduleMetadata, ex);
                
                LogManager.Error("ModuleLoadingService", $"卸载模块 {moduleMetadata.Name} 失败: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// 创建默认模块元数据
        /// </summary>
        /// <param name="moduleType">模块类型</param>
        /// <returns>模块元数据</returns>
        private static ModuleMetadata CreateDefaultMetadata(Type moduleType)
        {
            return new ModuleMetadata
            {
                Name = moduleType.Name,
                Description = $"Module {moduleType.Name}",
                Category = DetermineModuleCategory(moduleType),
                Priority = DetermineModulePriority(moduleType),
                ModuleType = moduleType
            };
        }
        
        /// <summary>
        /// 确定模块分类
        /// </summary>
        /// <param name="moduleType">模块类型</param>
        /// <returns>模块分类</returns>
        private static ModuleCategory DetermineModuleCategory(Type moduleType)
        {
            var name = moduleType.Name.ToLower();
            var namespaceName = moduleType.Namespace?.ToLower() ?? "";
            
            // 根据模块名称和命名空间确定分类
            if (name.Contains("core") || name.Contains("main") || 
                namespaceName.Contains("mainmenu") || namespaceName.Contains("toolbar"))
            {
                return ModuleCategory.Core;
            }
            
            if (namespaceName.Contains("theme") || namespaceName.Contains("windowmanagement"))
            {
                return ModuleCategory.UI;
            }
            
            return ModuleCategory.Feature;
        }
        
        /// <summary>
        /// 确定模块优先级
        /// </summary>
        /// <param name="moduleType">模块类型</param>
        /// <returns>优先级</returns>
        private static int DetermineModulePriority(Type moduleType)
        {
            var name = moduleType.Name.ToLower();
            var namespaceName = moduleType.Namespace?.ToLower() ?? "";
            
            // 根据模块类型确定优先级
            if (namespaceName.Contains("mainmenu")) return 10;
            if (namespaceName.Contains("toolbar")) return 20;
            if (namespaceName.Contains("statusbar")) return 30;
            if (namespaceName.Contains("theme")) return 40;
            if (namespaceName.Contains("windowmanagement")) return 50;
            
            return 100; // 默认优先级
        }
    }
}
