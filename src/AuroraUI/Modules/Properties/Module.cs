using System.Threading.Tasks;
using AuroraUI.Framework;
using AuroraUI.Framework.Modules;
using AuroraUI.Framework.Services;
using AuroraUI.Modules.Properties.ViewModels;

namespace AuroraUI.Modules.Properties
{
    /// <summary>
    /// 属性模块 - 支持延迟加载
    /// </summary>
    public class Module : LazyModuleBase
    {
        private PropertiesToolViewModel? _propertiesTool;
        
        /// <summary>
        /// 创建模块元数据
        /// </summary>
        /// <returns>模块元数据</returns>
        protected override ModuleMetadata CreateMetadata()
        {
            return new ModuleMetadata
            {
                Name = "PropertiesModule",
                Description = "属性模块，提供对象属性编辑功能",
                Category = ModuleCategory.Feature,
                Priority = 120,
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
            // 属性模块通常在需要编辑对象属性时加载
            return true;
        }
        
        /// <summary>
        /// 初始化模块
        /// </summary>
        public override void Initialize()
        {
            if (!IsLoaded) return;
            
            base.Initialize();
            
            // 注册属性工具
            var shell = IoC.Get<IShell>();
            _propertiesTool = IoC.Get<PropertiesToolViewModel>();
            if (shell != null && _propertiesTool != null)
            {
                shell.RegisterTool(_propertiesTool);
                shell.ShowTool(_propertiesTool); // 默认显示属性窗口
            }
        }
        
        /// <summary>
        /// 模块卸载时的清理逻辑
        /// </summary>
        /// <returns>清理任务</returns>
        protected override async Task OnUnloadingAsync()
        {
            if (_propertiesTool != null)
            {
                // 隐藏属性工具
                var shell = IoC.Get<IShell>();
                shell?.HideTool(_propertiesTool);
                _propertiesTool = null;
            }
            
            await base.OnUnloadingAsync();
        }
    }
}