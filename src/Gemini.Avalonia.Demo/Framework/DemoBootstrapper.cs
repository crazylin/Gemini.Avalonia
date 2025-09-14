using System;
using System.Linq;
using System.Threading.Tasks;
using Gemini.Avalonia.Demo.Framework;
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Framework.Modules;
using Gemini.Avalonia.Framework.Performance;
using Gemini.Avalonia.Framework.Logging;
using Gemini.Avalonia.Views;
using Gemini.Avalonia.Demo.ViewModels;
using Gemini.Avalonia.Framework.Extensions;
using Gemini.Avalonia.Services;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Controls;

namespace Gemini.Avalonia.Demo.Framework
{
    /// <summary>
    /// Demo应用程序的引导器，继承自核心AppBootstrapper
    /// </summary>
    public class DemoBootstrapper : AppBootstrapper
    {
        /// <summary>
        /// 初始化Demo应用程序
        /// </summary>
        public new DemoBootstrapper Initialize()
        {
            PerformanceMonitor.StartTimer("DemoBootstrapper.Initialize");
            
            // 首先调用基类的初始化（这会初始化LogManager）
            base.Initialize();
            
            // 现在可以安全使用LogManager
            LogManager.Info("DemoBootstrapper", "开始初始化Demo应用程序");
            
            // 注册Demo特定的模块配置
            InitializeDemoModules();
            
            // 加载Demo项目的语言资源
            LoadDemoLanguageResources();
            
            PerformanceMonitor.StopTimer("DemoBootstrapper.Initialize");
            LogManager.Info("DemoBootstrapper", "Demo应用程序初始化完成");
            
            return this;
        }
        
        /// <summary>
        /// 启动Demo应用程序
        /// </summary>
        /// <returns>主窗口</returns>
        public new async Task<ShellView> StartAsync()
        {
            PerformanceMonitor.StartTimer("DemoBootstrapper.StartAsync");
            LogManager.Info("DemoBootstrapper", "开始启动Demo应用程序");
            
            // 启动基础框架
            var mainWindow = await base.StartAsync();
            
            // 在基础框架启动完成后注册Demo视图绑定
            RegisterDemoViewBindings();
            
            // Demo特定的启动逻辑
            await InitializeDemoFeaturesAsync();
            
            PerformanceMonitor.StopTimer("DemoBootstrapper.StartAsync");
            PerformanceMonitor.LogSummary();
            
            LogManager.Info("DemoBootstrapper", "Demo应用程序启动完成");
            return mainWindow;
        }
        
        /// <summary>
        /// 初始化Demo模块
        /// </summary>
        private void InitializeDemoModules()
        {
            PerformanceMonitor.Measure("初始化Demo模块配置", () =>
            {
                if (ModuleManager != null)
                {
                    // 注册Demo特定的模块配置
                    var demoModules = DemoModuleConfiguration.GetDemoModuleConfigurations();
                    foreach (var moduleConfig in demoModules)
                    {
                        if (moduleConfig.ModuleType != null && moduleConfig.Name == "SampleDocumentModule")
                        {
                            // 只注册Demo特有的模块，核心模块已经在基类中注册
                            ModuleManager.RegisterModule(moduleConfig.ModuleType, moduleConfig);
                            LogManager.Info("DemoBootstrapper", $"已注册Demo模块: {moduleConfig.Name}");
                        }
                    }
                }
            });
        }
        
        /// <summary>
        /// 初始化Demo特有功能
        /// </summary>
        /// <returns>异步任务</returns>
        private async Task InitializeDemoFeaturesAsync()
        {
            await PerformanceMonitor.MeasureAsync("初始化Demo功能", async () =>
            {
                // 自动加载Demo文档模块
                if (ModuleManager != null)
                {
                    await ModuleManager.LoadModuleAsync("SampleDocumentModule");
                    LogManager.Info("DemoBootstrapper", "SampleDocumentModule已加载");
                }
                
                // 可以在这里添加其他Demo特有的初始化逻辑
                await InitializeDemoUIAsync();
            });
        }
        
        /// <summary>
        /// 初始化Demo UI特性
        /// </summary>
        /// <returns>异步任务</returns>
        private async Task InitializeDemoUIAsync()
        {
            await Task.Run(() =>
            {
                // Demo UI初始化逻辑
                LogManager.Info("DemoBootstrapper", "Demo UI特性初始化完成");
            });
        }
        
        /// <summary>
        /// 验证Demo特有的视图绑定（现在依赖自动绑定系统）
        /// </summary>
        private void RegisterDemoViewBindings()
        {
            try
            {
                LogManager.Info("DemoBootstrapper", "验证Demo视图绑定状态");
                
                if (Application.Current != null)
                {
                    var totalTemplates = Application.Current.DataTemplates.Count;
                    var sampleDocumentTemplates = Application.Current.DataTemplates
                        .Count(dt => dt.Match(typeof(SampleDocumentViewModel)));
                    
                    LogManager.Info("DemoBootstrapper", $"当前应用程序中有 {totalTemplates} 个DataTemplate");
                    LogManager.Info("DemoBootstrapper", $"其中 {sampleDocumentTemplates} 个匹配SampleDocumentViewModel");
                    
                    if (sampleDocumentTemplates > 0)
                    {
                        LogManager.Info("DemoBootstrapper", "✅ SampleDocumentViewModel的自动绑定已就绪");
                    }
                    else
                    {
                        LogManager.Warning("DemoBootstrapper", "⚠️ 未找到SampleDocumentViewModel的DataTemplate，自动绑定可能失败");
                    }
                }
                else
                {
                    LogManager.Warning("DemoBootstrapper", "Application.Current为null");
                }
                
                LogManager.Info("DemoBootstrapper", "Demo视图绑定验证完成，现在依赖自动绑定系统");
            }
            catch (Exception ex)
            {
                LogManager.Warning("DemoBootstrapper", $"验证Demo视图绑定时出错: {ex.Message}");
                LogManager.Debug("DemoBootstrapper", $"异常详情: {ex}");
            }
        }
        
        /// <summary>
        /// 获取Demo应用的模块管理器
        /// </summary>
        /// <returns>模块管理器</returns>
        public IModuleManager? GetDemoModuleManager()
        {
            return ModuleManager;
        }
        
        /// <summary>
        /// 按需加载Demo功能模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>加载结果</returns>
        public async Task<bool> LoadDemoFeatureAsync(string moduleName)
        {
            if (ModuleManager != null)
            {
                LogManager.Info("DemoBootstrapper", $"按需加载Demo功能: {moduleName}");
                return await ModuleManager.LoadModuleAsync(moduleName);
            }
            return false;
        }
        
        /// <summary>
        /// 加载所有Demo功能模块
        /// </summary>
        /// <returns>异步任务</returns>
        public async Task LoadAllDemoFeaturesAsync()
        {
            if (ModuleManager != null)
            {
                LogManager.Info("DemoBootstrapper", "开始加载所有Demo功能模块");
                await ModuleManager.LoadModulesByCategoryAsync(ModuleCategory.Feature);
                LogManager.Info("DemoBootstrapper", "所有Demo功能模块加载完成");
            }
        }
        
        /// <summary>
        /// 加载Demo项目的语言资源
        /// </summary>
        private void LoadDemoLanguageResources()
        {
            try
            {
                LogManager.Info("DemoBootstrapper", "开始加载Demo项目语言资源");
                
                var app = Application.Current;
                if (app == null)
                {
                    LogManager.Warning("DemoBootstrapper", "Application.Current为null，无法加载Demo语言资源");
                    return;
                }
                
                // 获取当前语言设置
                var currentLanguage = "zh-CN"; // 默认中文
                try
                {
                    var languageService = IoC.Get<ILanguageService>();
                    if (languageService != null)
                    {
                        currentLanguage = languageService.CurrentCulture.Name;
                        LogManager.Debug("DemoBootstrapper", $"从LanguageService获取语言: {currentLanguage}");
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Warning("DemoBootstrapper", $"获取LanguageService失败，使用默认语言: {ex.Message}");
                }
                
                // 加载Demo项目的语言资源文件
                var demoResourceUri = new Uri($"avares://Gemini.Avalonia.Demo/Resources/Languages/{currentLanguage}.xaml");
                LogManager.Debug("DemoBootstrapper", $"Demo语言资源URI: {demoResourceUri}");
                
                var demoResourceDictionary = AvaloniaXamlLoader.Load(demoResourceUri) as IResourceDictionary;
                if (demoResourceDictionary != null)
                {
                    app.Resources.MergedDictionaries.Add(demoResourceDictionary);
                    LogManager.Info("DemoBootstrapper", $"Demo语言资源加载完成: {currentLanguage}");
                }
                else
                {
                    LogManager.Warning("DemoBootstrapper", "Demo语言资源字典加载失败");
                }
            }
            catch (Exception ex)
            {
                LogManager.Warning("DemoBootstrapper", $"Demo语言资源加载失败: {ex.Message}");
                
                // 加载默认中文资源作为备用
                try
                {
                    var fallbackUri = new Uri("avares://Gemini.Avalonia.Demo/Resources/Languages/zh-CN.xaml");
                    var fallbackResource = AvaloniaXamlLoader.Load(fallbackUri) as IResourceDictionary;
                    if (fallbackResource != null)
                    {
                        Application.Current?.Resources.MergedDictionaries.Add(fallbackResource);
                        LogManager.Info("DemoBootstrapper", "Demo默认中文语言资源加载成功");
                    }
                }
                catch (Exception fallbackEx)
                {
                    LogManager.Error("DemoBootstrapper", $"Demo默认语言资源加载失败: {fallbackEx.Message}");
                }
            }
        }
    }
}
