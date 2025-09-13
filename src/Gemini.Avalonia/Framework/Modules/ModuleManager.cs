using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gemini.Avalonia.Framework.Logging;
using Gemini.Avalonia.Framework.Performance;

namespace Gemini.Avalonia.Framework.Modules
{
    /// <summary>
    /// 模块管理器实现
    /// </summary>
    public class ModuleManager : IModuleManager
    {
        private readonly ConcurrentDictionary<string, ModuleMetadata> _registeredModules = new();
        private readonly ConcurrentDictionary<string, ModuleMetadata> _loadedModules = new();
        private readonly object _lock = new();
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        
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
        /// 模块加载事件
        /// </summary>
        public event EventHandler<ModuleEventArgs>? ModuleLoaded;
        
        /// <summary>
        /// 模块卸载事件
        /// </summary>
        public event EventHandler<ModuleEventArgs>? ModuleUnloaded;
        
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
            LogManager.Info("ModuleManager", $"模块已注册: {metadata.Name} ({metadata.Category})");
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
        /// 加载核心模块
        /// </summary>
        /// <returns>加载任务</returns>
        public async Task LoadCoreModulesAsync()
        {
            var coreModules = _registeredModules.Values
                .Where(m => m.Category == ModuleCategory.Core)
                .OrderBy(m => m.Priority)
                .ToList();
            
            LogManager.Info("ModuleManager", $"开始加载 {coreModules.Count} 个核心模块");
            
            foreach (var moduleMetadata in coreModules)
            {
                await LoadModuleInternalAsync(moduleMetadata);
            }
            
            LogManager.Info("ModuleManager", "核心模块加载完成");
        }
        
        /// <summary>
        /// 按需加载模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>加载任务</returns>
        public async Task<bool> LoadModuleAsync(string moduleName)
        {
            if (_loadedModules.ContainsKey(moduleName))
            {
                LogManager.Debug("ModuleManager", $"模块 {moduleName} 已经加载");
                return true;
            }
            
            if (!_registeredModules.TryGetValue(moduleName, out var moduleMetadata))
            {
                LogManager.Warning("ModuleManager", $"未找到模块: {moduleName}");
                return false;
            }
            
            // 先加载依赖模块
            foreach (var dependency in moduleMetadata.Dependencies)
            {
                if (!await LoadModuleAsync(dependency))
                {
                    LogManager.Error("ModuleManager", $"加载模块 {moduleName} 失败：依赖模块 {dependency} 加载失败");
                    return false;
                }
            }
            
            return await LoadModuleInternalAsync(moduleMetadata);
        }
        
        /// <summary>
        /// 加载指定分类的模块
        /// </summary>
        /// <param name="category">模块分类</param>
        /// <returns>加载任务</returns>
        public async Task LoadModulesByCategoryAsync(ModuleCategory category)
        {
            var modules = _registeredModules.Values
                .Where(m => m.Category == category && !m.IsLoaded)
                .OrderBy(m => m.Priority)
                .ToList();
            
            LogManager.Info("ModuleManager", $"开始加载 {modules.Count} 个 {category} 类型模块");
            
            foreach (var moduleMetadata in modules)
            {
                await LoadModuleInternalAsync(moduleMetadata);
            }
            
            LogManager.Info("ModuleManager", $"{category} 类型模块加载完成");
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
                LogManager.Warning("ModuleManager", $"模块 {moduleName} 未加载");
                return false;
            }
            
            try
            {
                // 如果是延迟加载模块，调用其卸载方法
                if (moduleMetadata.Instance is ILazyModule lazyModule)
                {
                    await lazyModule.UnloadAsync();
                }
                
                moduleMetadata.IsLoaded = false;
                moduleMetadata.IsInitialized = false;
                moduleMetadata.Instance = null;
                
                _loadedModules.TryRemove(moduleName, out _);
                
                ModuleUnloaded?.Invoke(this, new ModuleEventArgs(moduleMetadata));
                LogManager.Info("ModuleManager", $"模块已卸载: {moduleName}");
                
                return true;
            }
            catch (Exception ex)
            {
                LogManager.Error("ModuleManager", $"卸载模块 {moduleName} 失败: {ex.Message}");
                return false;
            }
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
        /// 检查模块是否已加载
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>是否已加载</returns>
        public bool IsModuleLoaded(string moduleName)
        {
            return _loadedModules.ContainsKey(moduleName);
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
            
            // 注意：这里不能使用lock，因为我们需要await
            bool acquired = false;
            try
            {
                // 使用信号量来确保线程安全
                await _semaphore.WaitAsync();
                acquired = true;
                
                // 双重检查锁定
                if (moduleMetadata.IsLoaded)
                {
                    return true;
                }
                
                try
                {
                    var loadTimer = $"加载模块_{moduleMetadata.Name}";
                    PerformanceMonitor.StartTimer(loadTimer);
                    
                    LogManager.Info("ModuleManager", $"开始加载模块: {moduleMetadata.Name}");
                    
                    // 创建模块实例
                    if (moduleMetadata.ModuleType == null)
                    {
                        LogManager.Error("ModuleManager", $"模块 {moduleMetadata.Name} 的类型为空");
                        return false;
                    }
                    
                    IModule? moduleInstance = null;
                    PerformanceMonitor.Measure($"创建模块实例_{moduleMetadata.Name}", () => 
                    {
                        moduleInstance = Activator.CreateInstance(moduleMetadata.ModuleType) as IModule;
                    });
                    
                    if (moduleInstance == null)
                    {
                        LogManager.Error("ModuleManager", $"无法创建模块实例: {moduleMetadata.Name}");
                        return false;
                    }
                    
                    moduleMetadata.Instance = moduleInstance;
                    moduleMetadata.IsLoaded = true;
                    
                    // 如果是延迟加载模块，调用其加载方法
                    if (moduleInstance is ILazyModule lazyModule)
                    {
                        if (!lazyModule.ShouldLoad())
                        {
                            LogManager.Debug("ModuleManager", $"模块 {moduleMetadata.Name} 暂时不需要加载");
                            PerformanceMonitor.StopTimer(loadTimer);
                            return true;
                        }
                        
                        // 异步加载延迟模块
                        await lazyModule.LoadAsync();
                    }
                    else
                    {
                        // 预初始化
                        PerformanceMonitor.Measure($"预初始化_{moduleMetadata.Name}", 
                            () => moduleInstance.PreInitialize());
                        
                        // 初始化
                        PerformanceMonitor.Measure($"初始化_{moduleMetadata.Name}", 
                            () => moduleInstance.Initialize());
                    }
                    
                    moduleMetadata.IsInitialized = true;
                    
                    _loadedModules.TryAdd(moduleMetadata.Name, moduleMetadata);
                    
                    PerformanceMonitor.StopTimer(loadTimer);
                    LogManager.Info("ModuleManager", $"模块加载完成: {moduleMetadata.Name}");
                    ModuleLoaded?.Invoke(this, new ModuleEventArgs(moduleMetadata));
                    
                    return true;
                }
                catch (Exception ex)
                {
                    LogManager.Error("ModuleManager", $"加载模块 {moduleMetadata.Name} 失败: {ex.Message}");
                    moduleMetadata.IsLoaded = false;
                    moduleMetadata.IsInitialized = false;
                    moduleMetadata.Instance = null;
                    return false;
                }
            }
            finally
            {
                if (acquired)
                {
                    _semaphore.Release();
                }
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
