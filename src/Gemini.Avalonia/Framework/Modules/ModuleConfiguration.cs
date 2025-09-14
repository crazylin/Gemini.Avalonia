using System;
using System.Collections.Generic;
using System.Linq;
using Gemini.Avalonia.Modules.MainMenu;
using Gemini.Avalonia.Modules.ToolBars;
using Gemini.Avalonia.Modules.Theme;
using Gemini.Avalonia.Modules.WindowManagement;
using Gemini.Avalonia.Modules.StatusBar;
using Gemini.Avalonia.Modules.Output;
using Gemini.Avalonia.Modules.Properties;
using Gemini.Avalonia.Modules.ProjectManagement;
using Gemini.Avalonia.Modules.Settings;
using Gemini.Avalonia.Modules.UndoRedo;

namespace Gemini.Avalonia.Framework.Modules
{
    /// <summary>
    /// 模块配置，定义所有模块的元数据和加载策略
    /// </summary>
    public static class ModuleConfiguration
    {
        /// <summary>
        /// 获取所有模块配置
        /// </summary>
        /// <returns>模块配置列表</returns>
        public static List<ModuleMetadata> GetAllModuleConfigurations()
        {
            return new List<ModuleMetadata>
            {
                // 核心模块 - 必须在启动时加载
                new ModuleMetadata
                {
                    Name = "MainMenuModule",
                    Description = "主菜单模块，提供应用程序主菜单功能",
                    Category = ModuleCategory.Core,
                    Priority = 10,
                    AllowLazyLoading = false,
                    ModuleType = typeof(Gemini.Avalonia.Modules.MainMenu.Module)
                },
                
                // ToolBars模块已删除 - 工具栏服务通过MEF自动注册
                
                // StatusBar 通过MEF自动注册，不需要独立的模块类
                
                // UI 模块 - 界面显示时加载
                new ModuleMetadata
                {
                    Name = "ThemeModule",
                    Description = "主题模块，提供应用程序主题切换功能",
                    Category = ModuleCategory.UI,
                    Priority = 40,
                    AllowLazyLoading = true,
                    ModuleType = typeof(Gemini.Avalonia.Modules.Theme.Module)
                },
                
                // WindowManagement模块已删除 - 窗口管理命令和菜单通过MEF自动注册
                
                // // 功能模块 - 按需加载
                // new ModuleMetadata
                // {
                //     Name = "ProjectManagementModule",
                //     Description = "项目管理模块，提供项目创建、打开、管理功能",
                //     Category = ModuleCategory.Feature,
                //     Priority = 100,
                //     AllowLazyLoading = true,
                //     ModuleType = typeof(Gemini.Avalonia.Modules.ProjectManagement.Module),
                //     Dependencies = new List<string> { "MainMenuModule" }
                // },
                
                // new ModuleMetadata
                // {
                //     Name = "OutputModule",
                //     Description = "输出模块，提供应用程序输出和日志显示功能",
                //     Category = ModuleCategory.Feature,
                //     Priority = 110,
                //     AllowLazyLoading = true,
                //     ModuleType = typeof(Gemini.Avalonia.Modules.Output.Module)
                // },
                
                // new ModuleMetadata
                // {
                //     Name = "PropertiesModule",
                //     Description = "属性模块，提供对象属性编辑功能",
                //     Category = ModuleCategory.Feature,
                //     Priority = 120,
                //     AllowLazyLoading = true,
                //     ModuleType = typeof(Gemini.Avalonia.Modules.Properties.Module)
                // },
                
                new ModuleMetadata
                {
                    Name = "SettingsModule",
                    Description = "设置模块，提供应用程序配置和设置功能",
                    Category = ModuleCategory.Feature,
                    Priority = 130,
                    AllowLazyLoading = true,
                    ModuleType = typeof(Gemini.Avalonia.Modules.Settings.Module),
                    Dependencies = new List<string> { "MainMenuModule" }
                },
                
                // UndoRedo 功能通过MEF自动注册，不需要独立的模块类
            };
        }
        
        /// <summary>
        /// 获取核心模块配置
        /// </summary>
        /// <returns>核心模块配置列表</returns>
        public static List<ModuleMetadata> GetCoreModuleConfigurations()
        {
            return GetAllModuleConfigurations()
                .Where(m => m.Category == ModuleCategory.Core)
                .OrderBy(m => m.Priority)
                .ToList();
        }
        
        /// <summary>
        /// 获取功能模块配置
        /// </summary>
        /// <returns>功能模块配置列表</returns>
        public static List<ModuleMetadata> GetFeatureModuleConfigurations()
        {
            return GetAllModuleConfigurations()
                .Where(m => m.Category == ModuleCategory.Feature)
                .OrderBy(m => m.Priority)
                .ToList();
        }
        
        /// <summary>
        /// 获取UI模块配置
        /// </summary>
        /// <returns>UI模块配置列表</returns>
        public static List<ModuleMetadata> GetUIModuleConfigurations()
        {
            return GetAllModuleConfigurations()
                .Where(m => m.Category == ModuleCategory.UI)
                .OrderBy(m => m.Priority)
                .ToList();
        }
        
        /// <summary>
        /// 获取扩展模块配置
        /// </summary>
        /// <returns>扩展模块配置列表</returns>
        public static List<ModuleMetadata> GetExtensionModuleConfigurations()
        {
            return GetAllModuleConfigurations()
                .Where(m => m.Category == ModuleCategory.Extension)
                .OrderBy(m => m.Priority)
                .ToList();
        }
    }
}
