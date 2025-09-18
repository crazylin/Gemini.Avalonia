using System.Collections.Generic;
using System.Linq;
using AuroraUI.Demo.Modules;
using AuroraUI.Framework.Modules;

namespace AuroraUI.Demo.Framework
{
    /// <summary>
    /// Demo应用程序的模块配置
    /// </summary>
    public static class DemoModuleConfiguration
    {
        /// <summary>
        /// 获取Demo应用程序的所有模块配置
        /// </summary>
        /// <returns>模块配置列表</returns>
        public static List<ModuleMetadata> GetDemoModuleConfigurations()
        {
            // 获取核心框架的模块配置
            var coreModules = ModuleConfiguration.GetAllModuleConfigurations();
            
            // 添加Demo专用的模块配置
            var demoModules = new List<ModuleMetadata>
            {
                new ModuleMetadata
                {
                    Name = "SampleDocumentModule",
                    Description = "示例文档模块，演示文档管理功能",
                    Category = ModuleCategory.Feature,
                    Priority = 200,
                    AllowLazyLoading = true,
                    ModuleType = typeof(SampleDocumentModule),
                    Dependencies = new List<string> { "MainMenuModule" }
                }
            };
            
            // 合并核心模块和Demo模块
            var allModules = new List<ModuleMetadata>();
            allModules.AddRange(coreModules);
            allModules.AddRange(demoModules);
            
            return allModules;
        }
        
        /// <summary>
        /// 获取Demo应用程序的核心模块配置
        /// </summary>
        /// <returns>核心模块配置列表</returns>
        public static List<ModuleMetadata> GetDemoCoreModuleConfigurations()
        {
            return GetDemoModuleConfigurations()
                .Where(m => m.Category == ModuleCategory.Core)
                .OrderBy(m => m.Priority)
                .ToList();
        }
        
        /// <summary>
        /// 获取Demo应用程序的功能模块配置
        /// </summary>
        /// <returns>功能模块配置列表</returns>
        public static List<ModuleMetadata> GetDemoFeatureModuleConfigurations()
        {
            return GetDemoModuleConfigurations()
                .Where(m => m.Category == ModuleCategory.Feature)
                .OrderBy(m => m.Priority)
                .ToList();
        }
        
        /// <summary>
        /// 获取Demo应用程序的UI模块配置
        /// </summary>
        /// <returns>UI模块配置列表</returns>
        public static List<ModuleMetadata> GetDemoUIModuleConfigurations()
        {
            return GetDemoModuleConfigurations()
                .Where(m => m.Category == ModuleCategory.UI)
                .OrderBy(m => m.Priority)
                .ToList();
        }
    }
}
