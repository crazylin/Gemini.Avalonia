using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using AuroraUI.Framework;
using AuroraUI.Framework.Modules;
using AuroraUI.Framework.Logging;

namespace AuroraUI.Modules.Theme
{
    /// <summary>
    /// 主题模块 - 支持延迟加载
    /// </summary>
    [Export(typeof(IModule))]
    public class Module : LazyModuleBase
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        
        /// <summary>
        /// 全局资源字典集合 - 包含框架必需的 Dock 样式
        /// </summary>
        public override IEnumerable<IStyle> GlobalResourceDictionaries
        {
            get
            {
                var styles = new List<IStyle>();
                
                try
                {
                    // 添加 DockFluentTheme
                    Logger.Debug("加载 DockFluentTheme 到主题模块");
                    var dockTheme = new Dock.Avalonia.Themes.Fluent.DockFluentTheme();
                    styles.Add(dockTheme);
                    
                    // 添加 DocumentStyles.axaml - 通过 AvaloniaXamlLoader 加载
                    Logger.Debug("加载 DocumentStyles 到主题模块");
                    var documentStylesUri = new Uri("avares://AuroraUI/Modules/Theme/Resources/DocumentStyles.axaml");
                    var documentStyles = Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(documentStylesUri) as IStyle;
                    if (documentStyles != null)
                    {
                        styles.Add(documentStyles);
                    }
                    else
                    {
                        Logger.Warning("DocumentStyles.axaml 加载失败或不是有效的样式");
                    }
                    
                    Logger.Info("主题模块全局资源加载完成");
                }
                catch (Exception ex)
                {
                    Logger.Error($"加载主题模块全局资源失败: {ex.Message}", ex);
                }
                
                return styles;
            }
        }
        
        /// <summary>
        /// 创建模块元数据
        /// </summary>
        /// <returns>模块元数据</returns>
        protected override ModuleMetadata CreateMetadata()
        {
            return new ModuleMetadata
            {
                Name = "ThemeModule",
                Description = "主题模块，提供应用程序主题切换功能",
                Category = ModuleCategory.UI,
                Priority = 40,
                AllowLazyLoading = true,
                ModuleType = GetType()
            };
        }
        
        /// <summary>
        /// 检查是否应该加载此模块
        /// </summary>
        /// <returns>如果应该加载返回true</returns>
        public override bool ShouldLoad()
        {
            // 主题模块通常在界面显示时就应该加载
            return true;
        }
        
        /// <summary>
        /// 初始化模块
        /// </summary>
        public override void Initialize()
        {
            if (!IsLoaded) return;
            
            Logger.Info("主题模块初始化开始");
            base.Initialize();
            
            // 主题服务将在Shell中初始化，这里不需要重复初始化
            Logger.Info("主题模块初始化完成");
        }
    }
}