using System.Collections.Generic;
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Framework.Modules;

namespace Gemini.Avalonia.Modules.Settings
{
    /// <summary>
    /// 设置模块 - 支持延迟加载
    /// </summary>
    public class Module : LazyModuleBase
    {
        /// <summary>
        /// 创建模块元数据
        /// </summary>
        /// <returns>模块元数据</returns>
        protected override ModuleMetadata CreateMetadata()
        {
            return new ModuleMetadata
            {
                Name = "SettingsModule",
                Description = "设置模块，提供应用程序配置和设置功能",
                Category = ModuleCategory.Feature,
                Priority = 130,
                AllowLazyLoading = true,
                ModuleType = GetType(),
                Dependencies = new List<string> { "MainMenuModule" }
            };
        }
        
        /// <summary>
        /// 检查是否应该加载此模块
        /// </summary>
        /// <returns>如果应该加载返回true</returns>
        public override bool ShouldLoad()
        {
            // 设置模块在用户需要访问设置时加载
            return true;
        }
        
        /// <summary>
        /// 初始化模块
        /// </summary>
        public override void Initialize()
        {
            if (!IsLoaded) return;
            
            base.Initialize();
            
            // 设置模块的命令和菜单项通过MEF自动注册
            // 不需要手动注册任何服务
        }
    }
}