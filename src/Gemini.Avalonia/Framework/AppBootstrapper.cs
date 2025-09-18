using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Gemini.Avalonia.Framework.Services;
using Gemini.Avalonia.Framework.Extensions;
using Gemini.Avalonia.Framework.Logging;
using Gemini.Avalonia.Framework.Modules;
using Gemini.Avalonia.Modules.MainMenu;
using Gemini.Avalonia.Services;
using Gemini.Avalonia.Modules.StatusBar;
using Gemini.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Gemini.Avalonia.Framework.Commands;
using Gemini.Avalonia.Framework.Performance;
using System.Diagnostics;

namespace Gemini.Avalonia.Framework
{
    /// <summary>
    /// 应用程序引导器，负责初始化框架和模块
    /// </summary>
    public class AppBootstrapper
    {
        private readonly List<IModule> _modules = new();
        private CompositionContainer? _mefContainer;
        private static MefServiceProvider? _serviceProvider;
        private List<Assembly>? _priorityAssemblies;
        private LanguageService? _languageService;
        private ModuleManager? _moduleManager;
        
        /// <summary>
        /// MEF容器，用于服务定位器模式
        /// </summary>
        public static CompositionContainer? Container { get; private set; }
        
        /// <summary>
        /// 服务提供者，用于服务定位器模式
        /// </summary>
        public static IServiceProvider? ServiceProvider => _serviceProvider;
        
        /// <summary>
        /// 优先程序集列表
        /// </summary>
        public static IList<Assembly>? PriorityAssemblies { get; private set; }
        
        /// <summary>
        /// Shell服务
        /// </summary>
        public IShell? Shell { get; private set; }
        
        /// <summary>
        /// 主窗口
        /// </summary>
        public ShellView? MainWindow { get; private set; }
        
        /// <summary>
        /// 模块管理器
        /// </summary>
        public IModuleManager? ModuleManager => _moduleManager;
        
        /// <summary>
        /// 添加模块
        /// </summary>
        /// <typeparam name="T">模块类型</typeparam>
        public AppBootstrapper AddModule<T>() where T : IModule, new()
        {
            var module = new T();
            _modules.Add(module);
            return this;
        }
        
        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="module">模块实例</param>
        public AppBootstrapper AddModule(IModule module)
        {
            if (module != null && !_modules.Contains(module))
            {
                _modules.Add(module);
            }
            return this;
        }
        
        /// <summary>
        /// 按需加载功能模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>加载任务</returns>
        public async Task<bool> LoadFeatureModuleAsync(string moduleName)
        {
            if (_moduleManager != null)
            {
                return await _moduleManager.LoadModuleAsync(moduleName);
            }
            return false;
        }
        
        /// <summary>
        /// 加载所有功能模块
        /// </summary>
        /// <returns>加载任务</returns>
        public async Task LoadAllFeatureModulesAsync()
        {
            if (_moduleManager != null)
            {
                await _moduleManager.LoadModulesByCategoryAsync(ModuleCategory.Feature);
            }
        }
        
        /// <summary>
        /// 初始化应用程序
        /// </summary>
        public AppBootstrapper Initialize()
        {
            PerformanceMonitor.StartTimer("AppBootstrapper.Initialize");
            
            // 第一步：初始化日志系统
            PerformanceMonitor.Measure("初始化日志系统", InitializeLogging);
            
            // 第二步：早期语言配置初始化（在任何MEF操作之前）
            PerformanceMonitor.Measure("早期语言配置初始化", InitializeLanguageConfigurationEarly);
            
            // 第三步：初始化语言服务（在程序集扫描之前）
            PerformanceMonitor.Measure("初始化语言服务", InitializeLanguageService);
            
            // 第四步：加载语言资源文件（在MEF容器初始化之前）
            PerformanceMonitor.Measure("加载语言资源文件", LoadLanguageResourcesEarly);
            
            // 第五步：初始化MEF容器和程序集扫描
            PerformanceMonitor.Measure("初始化MEF容器", InitializeMefContainer);
            
            // 第六步：设置全局容器引用
            Container = _mefContainer;
            
            // 第七步：初始化模块管理器
            PerformanceMonitor.Measure("初始化模块管理器", InitializeModuleManager);
            
            PerformanceMonitor.StopTimer("AppBootstrapper.Initialize");
            LogManager.Info("AppBootstrapper", "应用程序初始化完成");
            return this;
        }
        
        /// <summary>
        /// 启动应用程序
        /// </summary>
        public async Task<ShellView> StartAsync()
        {
            try
            {
                PerformanceMonitor.StartTimer("AppBootstrapper.StartAsync");
                LogManager.Info("AppBootstrapper", "开始启动应用程序");
                
                // 确保已初始化
                if (_mefContainer == null)
                {
                    Initialize();
                }
                
                // 获取Shell服务
                Shell = Container!.GetExportedValue<IShell>();
                
                // 使用模块管理器加载核心模块
                if (_moduleManager != null)
                {
                    await PerformanceMonitor.MeasureAsync("加载核心模块", 
                        () => _moduleManager.LoadCoreModulesAsync());
                }
                
                // 初始化遗留模块（向后兼容）
                await PerformanceMonitor.MeasureAsync("初始化遗留模块", InitializeLegacyModulesAsync);
                
                // 加载全局资源
                PerformanceMonitor.Measure("加载全局资源", LoadGlobalResources);
                
                // 初始化工具栏
                PerformanceMonitor.Measure("初始化工具栏", InitializeToolBars);
                
                // 初始化Shell
                PerformanceMonitor.Measure("初始化Shell", () =>
                {
                    if (Shell is ShellViewModel shellViewModel)
                    {
                        shellViewModel.Initialize();
                    }
                });
                
                // 创建主窗口
                PerformanceMonitor.Measure("创建主窗口", () =>
                {
                    MainWindow = new ShellView(Shell as ShellViewModel ?? throw new InvalidOperationException("Shell must be ShellViewModel"));
                });
                
                // 加载默认文档
                await PerformanceMonitor.MeasureAsync("加载默认文档", LoadDefaultDocuments);
                
                // 注册所有MEF导出的工具
                PerformanceMonitor.Measure("注册MEF工具", RegisterTools);
                
                // 加载UI模块
                if (_moduleManager != null)
                {
                    await PerformanceMonitor.MeasureAsync("加载UI模块", 
                        () => _moduleManager.LoadModulesByCategoryAsync(ModuleCategory.UI));
                }
                
                // 后初始化遗留模块
                await PerformanceMonitor.MeasureAsync("遗留模块后初始化", PostInitializeLegacyModulesAsync);
                
                PerformanceMonitor.StopTimer("AppBootstrapper.StartAsync");
                PerformanceMonitor.LogSummary();
                
                LogManager.Info("AppBootstrapper", "应用程序启动完成");
                return MainWindow;
            }
            catch (Exception ex)
            {
                LogManager.Error("AppBootstrapper", $"启动应用程序时出错: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// 停止应用程序
        /// </summary>
        public async Task StopAsync()
        {
            if (Shell != null)
            {
                await Shell.CloseAsync();
            }
            
            if (_mefContainer != null)
            {
                _mefContainer.Dispose();
                _mefContainer = null;
                Container = null;
                _serviceProvider = null;
            }
        }
        
        #region 私有初始化方法
        
        /// <summary>
        /// 初始化模块管理器
        /// </summary>
        private void InitializeModuleManager()
        {
            try
            {
                LogManager.Info("AppBootstrapper", "开始初始化模块管理器");
                
                _moduleManager = new ModuleManager();
                
                // 注册所有配置的模块
                var moduleConfigurations = ModuleConfiguration.GetAllModuleConfigurations();
                foreach (var config in moduleConfigurations)
                {
                    if (config.ModuleType != null)
                    {
                        _moduleManager.RegisterModule(config.ModuleType, config);
                    }
                }
                
                // 注册遗留模块实例（向后兼容）
                RegisterLegacyModules();
                
                LogManager.Info("AppBootstrapper", $"模块管理器初始化完成，已注册 {moduleConfigurations.Count + _modules.Count} 个模块");
            }
            catch (Exception ex)
            {
                LogManager.Error("AppBootstrapper", $"初始化模块管理器失败: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// 注册遗留模块实例（向后兼容）
        /// </summary>
        private void RegisterLegacyModules()
        {
            foreach (var module in _modules)
            {
                var metadata = new ModuleMetadata
                {
                    Name = $"Legacy_{module.GetType().Name}",
                    Description = $"Legacy module {module.GetType().Name}",
                    Category = ModuleCategory.Core,
                    Priority = 0,
                    AllowLazyLoading = false,
                    ModuleType = module.GetType(),
                    Instance = module,
                    IsLoaded = true,
                    IsInitialized = false // 将在后续步骤中初始化
                };
                _moduleManager?.RegisterModule(module.GetType(), metadata);
                LogManager.Debug("AppBootstrapper", $"注册遗留模块: {metadata.Name}");
            }
        }
        
        /// <summary>
        /// 初始化遗留模块（向后兼容）
        /// </summary>
        private async Task InitializeLegacyModulesAsync()
        {
            LogManager.Info("AppBootstrapper", "开始初始化遗留模块");
            
            // 预初始化
            foreach (var module in _modules)
            {
                try
                {
                    module.PreInitialize();
                    LogManager.Debug("AppBootstrapper", $"遗留模块预初始化完成: {module.GetType().Name}");
                }
                catch (Exception ex)
                {
                    LogManager.Error("AppBootstrapper", $"遗留模块预初始化失败 {module.GetType().Name}: {ex.Message}");
                }
            }
            
            // 初始化
            foreach (var module in _modules)
            {
                try
                {
                    module.Initialize();
                    LogManager.Debug("AppBootstrapper", $"遗留模块初始化完成: {module.GetType().Name}");
                }
                catch (Exception ex)
                {
                    LogManager.Error("AppBootstrapper", $"遗留模块初始化失败 {module.GetType().Name}: {ex.Message}");
                }
            }
            
            LogManager.Info("AppBootstrapper", "遗留模块初始化完成");
            await Task.CompletedTask;
        }
        
        /// <summary>
        /// 后初始化遗留模块（向后兼容）
        /// </summary>
        private async Task PostInitializeLegacyModulesAsync()
        {
            LogManager.Info("AppBootstrapper", "开始后初始化遗留模块");
            
            foreach (var module in _modules)
            {
                try
                {
                    await module.PostInitializeAsync();
                    LogManager.Debug("AppBootstrapper", $"遗留模块后初始化完成: {module.GetType().Name}");
                }
                catch (Exception ex)
                {
                    LogManager.Error("AppBootstrapper", $"遗留模块后初始化失败 {module.GetType().Name}: {ex.Message}");
                }
            }
            
            LogManager.Info("AppBootstrapper", "遗留模块后初始化完成");
        }
        
        /// <summary>
        /// 初始化日志系统
        /// </summary>
        private void InitializeLogging()
        {
            // 创建 Microsoft.Extensions.Logging 日志工厂
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .SetMinimumLevel(LogLevel.Debug)
                    .AddConsole()
                    .AddDebug();
                
                // 在发布模式下，只显示信息级别及以上的日志
#if RELEASE
                builder.SetMinimumLevel(LogLevel.Information);
#endif
            });
            
            // 使用新的日志工厂初始化LogManager
            LogManager.Initialize(loggerFactory);
            LogManager.Info("AppBootstrapper", "日志系统初始化完成 - 使用 Microsoft.Extensions.Logging");
        }
        
        /// <summary>
        /// 早期初始化语言配置（在MEF容器初始化之前）
        /// </summary>
        private void InitializeLanguageConfigurationEarly()
        {
            try
            {
                LogManager.Info("AppBootstrapper", "开始早期语言配置初始化");
                
                // 直接读取配置文件，不依赖MEF容器
                var configPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Gemini.Avalonia",
                    "settings.json");
                
                string savedLanguage = "跟随系统";
                
                if (File.Exists(configPath))
                {
                    try
                    {
                        var configJson = File.ReadAllText(configPath);
                        var configDoc = JsonDocument.Parse(configJson);
                        
                        if (configDoc.RootElement.TryGetProperty("Application.Language", out var languageElement))
                        {
                            savedLanguage = languageElement.GetString() ?? "跟随系统";
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.Warning("AppBootstrapper", $"读取配置文件失败，使用默认语言: {ex.Message}");
                    }
                }
                
                // 根据配置设置文化信息
                CultureInfo culture = savedLanguage switch
                {
                    "跟随系统" => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "zh" 
                        ? new CultureInfo("zh-CN") 
                        : new CultureInfo("en-US"),
                    "中文" or "Chinese" or "Language.Chinese" => new CultureInfo("zh-CN"),
                    "English" or "Language.English" => new CultureInfo("en-US"),
                    _ => new CultureInfo("zh-CN") // 默认使用中文而不是英文
                };
                
                // 设置当前线程的文化信息
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
                
                LogManager.Info("AppBootstrapper", $"早期语言配置完成: {culture.DisplayName} (保存的设置: {savedLanguage})");
            }
            catch (Exception ex)
            {
                LogManager.Error("AppBootstrapper", $"早期语言配置初始化失败: {ex.Message}");
                // 失败时使用默认英语
                var defaultCulture = new CultureInfo("en-US");
                CultureInfo.CurrentCulture = defaultCulture;
                CultureInfo.CurrentUICulture = defaultCulture;
            }
        }
        
        /// <summary>
        /// 初始化语言服务（在程序集扫描之前）
        /// </summary>
        private void InitializeLanguageService()
        {
            try
            {
                LogManager.Info("AppBootstrapper", "开始初始化语言服务");
                
                // 创建统一的语言服务（同时实现 ILanguageService 和 ILocalizationService）
                _languageService = new LanguageService();
                
                // 初始化语言服务，确保它使用早期设置的文化信息
                _languageService.Initialize();
                
                // 确保语言服务使用当前线程的文化信息
                if (_languageService is LanguageService langService)
                {
                    langService.SetCurrentCultureInternal(CultureInfo.CurrentUICulture);
                    LogManager.Info("AppBootstrapper", $"语言服务已设置为: {CultureInfo.CurrentUICulture.DisplayName}");
                }
                
                LogManager.Info("AppBootstrapper", "语言服务初始化完成");
            }
            catch (Exception ex)
            {
                LogManager.Error("AppBootstrapper", $"初始化语言服务失败: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// 初始化MEF容器
        /// </summary>
        private void InitializeMefContainer()
        {
            try
            {
                LogManager.Info("AppBootstrapper", "开始初始化MEF容器");
                
                // 创建临时MEF容器来注册语言服务，确保IoC容器可以访问
                var tempCatalog = new AggregateCatalog();
                var tempContainer = new CompositionContainer(tempCatalog);
                var tempBatch = new CompositionBatch();
                tempBatch.AddExportedValue<ILanguageService>(_languageService!);
                tempBatch.AddExportedValue<ILocalizationService>(_languageService!);
                tempContainer.Compose(tempBatch);
                
                // 临时设置IoC容器，确保MenuDefinition创建时可以访问语言服务
                IoC.SetContainer(tempContainer);
                LogManager.Info("AppBootstrapper", "临时IoC容器已设置，语言服务可用");
                
                // 扫描和收集程序集
                var assemblies = CollectAssemblies();
                
                // 设置优先程序集
                _priorityAssemblies = assemblies.Take(2).ToList();
                PriorityAssemblies = _priorityAssemblies;
                
                // 创建MEF目录和容器
                var catalog = new AggregateCatalog(assemblies.Select(x => new AssemblyCatalog(x)));
                _mefContainer = new CompositionContainer(catalog);
                
                // 注册核心服务到MEF容器
                var batch = new CompositionBatch();
                batch.AddExportedValue<ILanguageService>(_languageService!);
                batch.AddExportedValue<Framework.Services.ILocalizationService>(_languageService!);
                
                // 创建MEF服务提供者
                var mefServiceProvider = new MefServiceProvider(_mefContainer);
                _serviceProvider = mefServiceProvider;
                
                // 进行MEF组合
                _mefContainer.Compose(batch);
                
                // 设置IoC容器
                IoC.SetContainer(_mefContainer);
                
                // 验证核心服务
                ValidateCoreServices();
                
                LogManager.Info("AppBootstrapper", "MEF容器初始化完成");
            }
            catch (Exception ex)
            {
                LogManager.Error("AppBootstrapper", $"初始化MEF容器失败: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// 收集程序集
        /// </summary>
        private List<Assembly> CollectAssemblies()
        {
            var assemblies = new List<Assembly>();
            var entryAssembly = Assembly.GetEntryAssembly();
            var currentAssembly = Assembly.GetExecutingAssembly();
            
            // 添加入口程序集和当前程序集
            if (entryAssembly != null)
            {
                assemblies.Add(entryAssembly);
            }
            
            if (currentAssembly != entryAssembly)
            {
                assemblies.Add(currentAssembly);
            }
            
            // 扫描应用程序域中的相关程序集
            var allowedAssemblyPrefixes = new[] 
            {
                "Gemini.Avalonia",
                "Gemini.Avalonia.Demo",
                "Gemini.Avalonia.SimpleDemo"
            };
            
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assemblies.Contains(assembly) && !assembly.IsDynamic)
                {
                    try
                    {
                        var assemblyName = assembly.GetName().Name;
                        
                        // 只处理允许的程序集前缀
                        if (allowedAssemblyPrefixes.Any(prefix => assemblyName?.StartsWith(prefix) == true))
                        {
                            // 检查程序集是否包含MEF导出
                            var exportTypes = assembly.GetTypes().Where(t => 
                                t.GetCustomAttributes(typeof(ExportAttribute), false).Any() ||
                                t.GetCustomAttributes(typeof(InheritedExportAttribute), false).Any()).ToArray();
                            
                            if (exportTypes.Any())
                            {
                                assemblies.Add(assembly);
                                LogManager.Debug("AppBootstrapper", $"添加程序集: {assemblyName}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.Warning("AppBootstrapper", $"检查程序集 {assembly.FullName} 时出错: {ex.Message}");
                    }
                }
            }
            
            return assemblies;
        }
        
        /// <summary>
        /// 验证核心服务
        /// </summary>
        private void ValidateCoreServices()
        {
            try
            {
                var commandService = _mefContainer?.GetExportedValue<Commands.ICommandService>();
                if (commandService != null)
                {
                    LogManager.Info("AppBootstrapper", $"成功获取ICommandService: {commandService.GetType().FullName}");
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("AppBootstrapper", $"获取ICommandService失败: {ex.Message}");
            }
            
            try
            {
                var menu = _mefContainer?.GetExportedValue<IMenu>();
            }
            catch (Exception ex)
            {
                // 忽略菜单获取错误
            }
        }
        
        /// <summary>
        /// 初始化工具栏
        /// </summary>
        private void InitializeToolBars()
        {
            try
            {
                if (Shell?.ToolBars is Gemini.Avalonia.Modules.ToolBars.Models.ToolBarsModel toolBarsModel)
                {
                    toolBarsModel.InitializeToolBars();
                    LogManager.Info("AppBootstrapper", "工具栏初始化完成");
                }
            }
            catch (Exception ex)
            {
                LogManager.Warning("AppBootstrapper", $"初始化工具栏时出错: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 加载默认文档
        /// </summary>
        private async Task LoadDefaultDocuments()
        {
            foreach (var module in _modules)
            {
                foreach (var document in module.DefaultDocuments)
                {
                    await Shell!.OpenDocumentAsync(document);
                }
            }
        }
        
        /// <summary>
        /// 注册工具
        /// </summary>
        private void RegisterTools()
        {
            try
            {
                LogManager.Info("AppBootstrapper", "开始注册工具");
                var allTools = _mefContainer!.GetExportedValues<ITool>();
                LogManager.Info("AppBootstrapper", $"从MEF容器中找到 {allTools.Count()} 个工具");
                
                // 使用模块过滤服务
                var moduleFilterService = _mefContainer.GetExportedValue<IModuleFilterService>();
                var filteredTools = allTools.Where(tool => moduleFilterService.IsTypeFromEnabledModule(tool.GetType())).ToList();
                
                LogManager.Info("AppBootstrapper", $"过滤后的工具数量: {filteredTools.Count}");
                
                foreach (var tool in filteredTools)
                {
                    LogManager.Info("AppBootstrapper", $"正在注册工具: {tool.GetType().Name} - {tool.DisplayName}");
                    var result = Shell!.RegisterTool(tool);
                    LogManager.Info("AppBootstrapper", $"工具注册结果: {tool.DisplayName} - {(result ? "成功" : "失败(已存在)")}");
                }
                LogManager.Info("AppBootstrapper", "工具注册完成");
            }
            catch (Exception ex)
            {
                LogManager.Error("AppBootstrapper", $"注册工具时出错: {ex.Message}");
                LogManager.Error("AppBootstrapper", $"异常详情: {ex}");
            }
        }
        
        #endregion
        
        #region 资源加载方法
        
        /// <summary>
        /// 加载全局资源
        /// </summary>
        private void LoadGlobalResources()
        {
            LogManager.Info("AppBootstrapper", "开始加载全局资源");
            var app = Application.Current;
            if (app == null) 
            {
                LogManager.Warning("AppBootstrapper", "Application.Current为null，无法加载全局资源");
                return;
            }
            
            // 加载模块资源
            LogManager.Debug("AppBootstrapper", "开始加载模块资源");
            LoadModuleResources(app);
            
            // 加载语言资源文件
            LogManager.Debug("AppBootstrapper", "准备加载语言资源文件");
            LoadLanguageResources();
            
            // 注册ViewModel绑定
            LogManager.Debug("AppBootstrapper", "开始注册ViewModel绑定");
            RegisterViewModelBindings();
            
            LogManager.Info("AppBootstrapper", "全局资源加载完成");
        }
        
        /// <summary>
        /// 加载模块资源
        /// </summary>
        private void LoadModuleResources(Application app)
        {
            foreach (var module in _modules)
            {
                foreach (var resourceDictionary in module.GlobalResourceDictionaries)
                {
                    try
                    {
                        app.Styles.Add(resourceDictionary);
                    }
                    catch (Exception ex)
                    {
                        LogManager.Warning("AppBootstrapper", $"加载模块资源字典失败: {ex.Message}");
                    }
                }
            }
        }
        
        /// <summary>
        /// 根据LanguageService的当前语言设置加载对应的语言资源
        /// </summary>
        private void LoadLanguageResources()
        {
            try
            {
                LogManager.Info("AppBootstrapper", "开始加载语言资源");
                
                var app = Application.Current;
                if (app == null) 
                {
                    LogManager.Warning("AppBootstrapper", "Application.Current为null，退出");
                    return;
                }
                
                // 从LanguageService获取当前语言设置
                var currentLanguage = "zh-CN"; // 默认值
                try
                {
                    if (_languageService != null)
                    {
                        currentLanguage = _languageService.CurrentCulture.Name;
                        LogManager.Debug("AppBootstrapper", $"从LanguageService获取到的语言: {currentLanguage}");
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Warning("AppBootstrapper", $"获取LanguageService失败: {ex.Message}");
                }
                
                // 加载对应的语言资源文件
                var resourceUri = new Uri($"avares://Gemini.Avalonia/Resources/Languages/{currentLanguage}.xaml");
                LogManager.Debug("AppBootstrapper", $"语言资源URI: {resourceUri}");
                
                var resourceDictionary = AvaloniaXamlLoader.Load(resourceUri) as IResourceDictionary;
                if (resourceDictionary != null)
                {
                    app.Resources.MergedDictionaries.Add(resourceDictionary);
                    LogManager.Info("AppBootstrapper", $"语言资源加载完成: {currentLanguage}");
                }
                else
                {
                    LogManager.Warning("AppBootstrapper", "语言资源字典加载失败");
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("AppBootstrapper", $"加载语言资源失败: {ex.Message}");
                // 失败时加载默认中文资源
                LoadFallbackLanguageResources();
            }
        }
        
        /// <summary>
        /// 早期加载语言资源文件（在MEF容器初始化之前）
        /// </summary>
        protected virtual void LoadLanguageResourcesEarly()
        {
            try
            {
                LogManager.Info("AppBootstrapper", "开始早期加载语言资源");
                
                var app = Application.Current;
                if (app == null) 
                {
                    LogManager.Warning("AppBootstrapper", "Application.Current为null，无法加载语言资源");
                    return;
                }
                
                // 从LanguageService获取当前语言设置
                var currentLanguage = "zh-CN"; // 默认值
                try
                {
                    if (_languageService != null)
                    {
                        currentLanguage = _languageService.CurrentCulture.Name;
                        LogManager.Debug("AppBootstrapper", $"从LanguageService获取到的语言: {currentLanguage}");
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Warning("AppBootstrapper", $"获取LanguageService失败: {ex.Message}");
                }
                
                // 加载对应的语言资源文件
                var resourceUri = new Uri($"avares://Gemini.Avalonia/Resources/Languages/{currentLanguage}.xaml");
                LogManager.Debug("AppBootstrapper", $"早期语言资源URI: {resourceUri}");
                
                var resourceDictionary = AvaloniaXamlLoader.Load(resourceUri) as IResourceDictionary;
                if (resourceDictionary != null)
                {
                    app.Resources.MergedDictionaries.Add(resourceDictionary);
                    LogManager.Info("AppBootstrapper", $"早期语言资源加载完成: {currentLanguage}");
                }
                else
                {
                    LogManager.Warning("AppBootstrapper", "早期语言资源字典加载失败");
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("AppBootstrapper", $"早期加载语言资源失败: {ex.Message}");
                // 失败时加载默认中文资源
                LoadFallbackLanguageResourcesEarly();
            }
        }
        
        /// <summary>
        /// 早期加载备用语言资源
        /// </summary>
        private void LoadFallbackLanguageResourcesEarly()
        {
            try
            {
                LogManager.Debug("AppBootstrapper", "尝试早期加载默认中文资源");
                var app = Application.Current;
                if (app != null)
                {
                    var fallbackResourceUri = new Uri("avares://Gemini.Avalonia/Resources/Languages/zh-CN.xaml");
                    var fallbackResource = AvaloniaXamlLoader.Load(fallbackResourceUri) as IResourceDictionary;
                    if (fallbackResource != null)
                    {
                        app.Resources.MergedDictionaries.Add(fallbackResource);
                        LogManager.Debug("AppBootstrapper", "早期默认中文资源加载成功");
                    }
                }
            }
            catch (Exception fallbackEx)
            {
                LogManager.Error("AppBootstrapper", $"早期加载默认中文资源时发生异常: {fallbackEx.Message}");
            }
        }
        
        /// <summary>
        /// 加载备用语言资源
        /// </summary>
        private void LoadFallbackLanguageResources()
        {
            try
            {
                LogManager.Debug("AppBootstrapper", "尝试加载默认中文资源");
                var app = Application.Current;
                if (app != null)
                {
                    var fallbackResourceUri = new Uri("avares://Gemini.Avalonia/Resources/Languages/zh-CN.xaml");
                    var fallbackResource = AvaloniaXamlLoader.Load(fallbackResourceUri) as IResourceDictionary;
                    if (fallbackResource != null)
                    {
                        app.Resources.MergedDictionaries.Add(fallbackResource);
                        LogManager.Debug("AppBootstrapper", "默认中文资源加载成功");
                    }
                }
            }
            catch (Exception fallbackEx)
            {
                LogManager.Error("AppBootstrapper", $"加载默认中文资源时发生异常: {fallbackEx.Message}");
            }
        }
        
        /// <summary>
        /// 注册ViewModel和View的自动绑定
        /// </summary>
        private void RegisterViewModelBindings()
        {
            var app = Application.Current;
            if (app == null) return;
            
            try
            {
                // 获取所有已加载的程序集
                var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic && 
                               !string.IsNullOrEmpty(a.Location) &&
                               (a.FullName?.Contains("Gemini.Avalonia") == true ||
                                a.FullName?.Contains("Demo") == true))
                    .ToArray();
                
                // 使用自定义配置进行绑定
                var bindingOptions = new ViewModelViewBindingOptions
                {
                    EnableVerboseLogging = true,
                    ShowWarningsForMissingViews = true,
                    AssemblyFilter = assembly => 
                    {
                        var name = assembly.FullName;
                        return !assembly.IsDynamic && 
                               !string.IsNullOrEmpty(assembly.Location) &&
                               (name?.Contains("Gemini.Avalonia") == true ||
                                name?.Contains("Demo") == true ||
                                name?.Contains("Module") == true);
                    },
                    ViewModelFilter = type => 
                        (type.Name.EndsWith("ViewModel") || type.Name.EndsWith("Message")) && 
                        type.IsClass && 
                        !type.IsAbstract && 
                        type.IsPublic,
                    CustomNamingConvention = viewModelName => 
                    {
                        if (viewModelName.EndsWith("Message"))
                            return viewModelName + "View";
                        return viewModelName.Replace("ViewModel", "View");
                    }
                };
                
                // 注册自动绑定
                app.RegisterViewModelViewBindings(bindingOptions, assemblies);
                LogManager.Info("AppBootstrapper", "ViewModel绑定注册完成");
            }
            catch (Exception ex)
            {
                LogManager.Warning("AppBootstrapper", $"注册ViewModel绑定时出错: {ex.Message}");
            }
        }
        
        #endregion
    }
}