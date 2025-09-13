using System.ComponentModel.Composition;
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Framework.Modules;
using Gemini.Avalonia.Framework.Logging;

namespace Gemini.Avalonia.Modules.Theme
{
    /// <summary>
    /// 主题模块 - 支持延迟加载
    /// </summary>
    [Export(typeof(IModule))]
    public class Module : LazyModuleBase
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        
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