using System;
using System.Threading.Tasks;
using AuroraUI.Framework;
using AuroraUI.Framework.Logging;
using AuroraUI.Framework.Modules;
using AuroraUI.Framework.Services;
using AuroraUI.Views;

namespace SCSA
{
    /// <summary>
    /// SCSA启动器
    /// </summary>
    public class SCSABootstrapper : AppBootstrapper
    {
        public new SCSABootstrapper Initialize()
        {
            // 禁用项目管理模块
            ModuleFilterService.DisabledModules.Clear();
            ModuleFilterService.DisabledModules.Add("ProjectManagementModule");
            ModuleFilterService.DisabledModules.Add("PropertiesModule");
            
            // 添加SCSA模块（使用统一的方式）
            AddModule<SCSAModule>();
            
            // 基类初始化
            base.Initialize();
            
            LogManager.Info("SCSABootstrapper", "SCSA应用程序初始化完成");
            return this;
        }
        
        public new async Task<ShellView> StartAsync()
        {
            var mainWindow = await base.StartAsync();
            
            // 设置主窗口标题
            mainWindow.Title = "SCSA - 数据采集分析系统";
            
            // 强制加载设置模块以确保 ApplicationSettingsViewModel 可用
            await ForceLoadSettingsModule();
            
            LogManager.Info("SCSABootstrapper", "SCSA应用程序启动完成");
            return mainWindow;
        }
        
        /// <summary>
        /// 强制加载设置模块
        /// </summary>
        private async Task ForceLoadSettingsModule()
        {
            try
            {
                LogManager.Info("SCSABootstrapper", "开始强制加载设置模块...");
                
                var moduleManager = IoC.Get<IModuleManager>();
                if (moduleManager != null)
                {
                    // 强制加载设置模块
                    await moduleManager.LoadModuleAsync("SettingsModule");
                    LogManager.Info("SCSABootstrapper", "设置模块强制加载完成");
                }
                else
                {
                    LogManager.Error("SCSABootstrapper", "无法获取模块管理器");
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("SCSABootstrapper", $"强制加载设置模块失败: {ex.Message}");
            }
        }
        
    }
}