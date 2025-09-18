using System.Collections.Generic;
using System.Reflection;
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
        protected override IEnumerable<Assembly>? GetAdditionalAssemblies()
        {
            // 获取调用方法信息
            var callerMethod = new System.Diagnostics.StackTrace().GetFrame(1)?.GetMethod()?.Name;
            
            // 只为ViewModel绑定提供SCSA程序集，MEF容器会自动包含入口程序集
            if (callerMethod == "RegisterViewModelBindings")
            {
                return new[] { Assembly.GetExecutingAssembly() };
            }
            
            return null;
        }
        
        public new SCSABootstrapper Initialize()
        {
            // 禁用项目管理模块
            ModuleFilterService.DisabledModules.Clear();
            ModuleFilterService.DisabledModules.Add("ProjectManagementModule");
            
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
            mainWindow.Title = "SCSA - 智能状态感知与分析系统";
            
            LogManager.Info("SCSABootstrapper", "SCSA应用程序启动完成");
            return mainWindow;
        }
    }
}