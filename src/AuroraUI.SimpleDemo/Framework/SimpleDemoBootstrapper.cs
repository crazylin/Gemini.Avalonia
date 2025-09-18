using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using AuroraUI.Framework;
using AuroraUI.Framework.Logging;
using AuroraUI.Framework.Modules;
using AuroraUI.Framework.Services;
using AuroraUI.Views;

namespace AuroraUI.SimpleDemo.Framework
{
    /// <summary>
    /// 简单Demo启动器
    /// </summary>
    public class SimpleDemoBootstrapper : AppBootstrapper
    {
        /// <summary>
        /// 初始化Demo应用程序
        /// </summary>
        public new SimpleDemoBootstrapper Initialize()
        {
            // 在基类初始化之前设置要禁用的模块列表（先静默设置）
            ModuleFilterService.DisabledModules.Clear();
            ModuleFilterService.DisabledModules.Add("ProjectManagementModule");
            
            // 添加SimpleDemo模块（使用最简单的方式）
            AddModule<SimpleDemoModule>();
            
            // 调用基类初始化（这会初始化日志系统）
            base.Initialize();
            
            // 在日志系统初始化后记录禁用模块信息
            LogManager.Info("SimpleDemoBootstrapper", 
                $"已配置禁用模块: {string.Join(", ", ModuleFilterService.DisabledModules)}");
            
            LogManager.Info("SimpleDemoBootstrapper", "简单Demo应用程序初始化完成");
            return this;
        }
        
        /// <summary>
        /// 启动Demo应用程序
        /// </summary>
        /// <returns>启动任务</returns>
        public new async Task<ShellView> StartAsync()
        {
            LogManager.Info("SimpleDemoBootstrapper", "开始启动简单Demo应用程序");
            
            // 调用基类启动
            var mainWindow = await base.StartAsync();
   
            LogManager.Info("SimpleDemoBootstrapper", "简单Demo应用程序启动完成");
            return mainWindow;
        }
   }
}