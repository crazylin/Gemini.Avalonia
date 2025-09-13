using System.Collections.Generic;
using System.Threading.Tasks;
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Framework.Modules;
using Gemini.Avalonia.Framework.Services;
using Gemini.Avalonia.Modules.Output.ViewModels;

namespace Gemini.Avalonia.Modules.Output
{
    /// <summary>
    /// 输出模块 - 支持延迟加载
    /// </summary>
    public class Module : LazyModuleBase
    {
        private OutputToolViewModel? _outputTool;
        
        /// <summary>
        /// 创建模块元数据
        /// </summary>
        /// <returns>模块元数据</returns>
        protected override ModuleMetadata CreateMetadata()
        {
            return new ModuleMetadata
            {
                Name = "OutputModule",
                Description = "输出模块，提供应用程序输出和日志显示功能",
                Category = ModuleCategory.Feature,
                Priority = 110,
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
            // 输出模块通常在需要显示日志或输出信息时加载
            return true;
        }
        
        /// <summary>
        /// 初始化模块
        /// </summary>
        public override void Initialize()
        {
            if (!IsLoaded) return;
            
            base.Initialize();
            
            // 注册输出工具
            var shell = IoC.Get<IShell>();
            _outputTool = IoC.Get<OutputToolViewModel>();
            if (shell != null && _outputTool != null)
            {
                shell.RegisterTool(_outputTool);
                shell.ShowTool(_outputTool); // 默认显示输出窗口
            }
        }
        
        /// <summary>
        /// 模块卸载时的清理逻辑
        /// </summary>
        /// <returns>清理任务</returns>
        protected override async Task OnUnloadingAsync()
        {
            if (_outputTool != null)
            {
                // 隐藏输出工具
                var shell = IoC.Get<IShell>();
                shell?.HideTool(_outputTool);
                _outputTool = null;
            }
            
            await base.OnUnloadingAsync();
        }
    }
}