using System;
using System.Threading.Tasks;
using Gemini.Avalonia.Framework.Logging;

namespace Gemini.Avalonia.Framework.Modules
{
    /// <summary>
    /// 支持延迟加载的模块基类
    /// </summary>
    public abstract class LazyModuleBase : ModuleBase, ILazyModule
    {
        private bool _isLoaded = false;
        private readonly object _lockObject = new();
        
        /// <summary>
        /// 模块元数据
        /// </summary>
        public virtual ModuleMetadata Metadata { get; protected set; }
        
        /// <summary>
        /// 检查模块是否已加载
        /// </summary>
        public bool IsLoaded => _isLoaded;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        protected LazyModuleBase()
        {
            Metadata = CreateMetadata();
        }
        
        /// <summary>
        /// 创建模块元数据，子类可重写以提供自定义元数据
        /// </summary>
        /// <returns>模块元数据</returns>
        protected virtual ModuleMetadata CreateMetadata()
        {
            return new ModuleMetadata
            {
                Name = GetType().Name,
                Description = $"Lazy module {GetType().Name}",
                Category = ModuleCategory.Feature,
                Priority = 100,
                AllowLazyLoading = true,
                ModuleType = GetType()
            };
        }
        
        /// <summary>
        /// 检查是否应该加载此模块，子类可重写以提供自定义逻辑
        /// </summary>
        /// <returns>如果应该加载返回true</returns>
        public virtual bool ShouldLoad()
        {
            return true; // 默认总是应该加载
        }
        
        /// <summary>
        /// 异步加载模块
        /// </summary>
        /// <returns>加载任务</returns>
        public virtual async Task LoadAsync()
        {
            if (_isLoaded)
            {
                return;
            }
            
            lock (_lockObject)
            {
                if (_isLoaded)
                {
                    return;
                }
                
                try
                {
                    LogManager.Info(GetType().Name, "开始延迟加载模块");
                    
                    // 先设置加载状态，这样Initialize()中的IsLoaded检查就会通过
                    _isLoaded = true;
                    Metadata.IsLoaded = true;
                    Metadata.IsInitialized = true;
                    
                    // 调用模块的预初始化
                    PreInitialize();
                    
                    // 调用模块的初始化
                    Initialize();
                    
                    LogManager.Info(GetType().Name, "模块延迟加载完成");
                }
                catch (Exception ex)
                {
                    LogManager.Error(GetType().Name, $"延迟加载模块失败: {ex.Message}");
                    throw;
                }
            }
            
            // 异步后初始化
            await PostInitializeAsync();
        }
        
        /// <summary>
        /// 卸载模块
        /// </summary>
        /// <returns>卸载任务</returns>
        public virtual async Task UnloadAsync()
        {
            if (!_isLoaded)
            {
                return;
            }
            
            try
            {
                LogManager.Info(GetType().Name, "开始卸载模块");
                
                // 执行模块特定的清理工作
                await OnUnloadingAsync();
                
                // 清理资源
                CleanupResources();
                
                _isLoaded = false;
                Metadata.IsLoaded = false;
                Metadata.IsInitialized = false;
                
                LogManager.Info(GetType().Name, "模块卸载完成");
            }
            catch (Exception ex)
            {
                LogManager.Error(GetType().Name, $"卸载模块失败: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// 模块卸载时的清理逻辑，子类可重写
        /// </summary>
        /// <returns>清理任务</returns>
        protected virtual async Task OnUnloadingAsync()
        {
            await Task.CompletedTask;
        }
        
        /// <summary>
        /// 清理资源，子类可重写
        /// </summary>
        protected virtual void CleanupResources()
        {
            // 默认实现为空，子类可重写以提供特定的清理逻辑
        }
        
        /// <summary>
        /// 确保模块已加载
        /// </summary>
        protected async Task EnsureLoadedAsync()
        {
            if (!_isLoaded)
            {
                await LoadAsync();
            }
        }
        
        /// <summary>
        /// 重写初始化方法，确保在延迟加载时调用
        /// </summary>
        public override void Initialize()
        {
            if (_isLoaded)
            {
                base.Initialize();
            }
        }
        
        /// <summary>
        /// 重写预初始化方法，确保在延迟加载时调用
        /// </summary>
        public override void PreInitialize()
        {
            if (_isLoaded)
            {
                base.PreInitialize();
            }
        }
        
        /// <summary>
        /// 重写后初始化方法，确保在延迟加载时调用
        /// </summary>
        /// <returns>异步任务</returns>
        public override async Task PostInitializeAsync()
        {
            if (_isLoaded)
            {
                await base.PostInitializeAsync();
            }
        }
    }
}
