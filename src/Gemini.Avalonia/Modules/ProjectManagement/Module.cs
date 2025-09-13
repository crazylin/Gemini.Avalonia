using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Styling;
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Framework.Modules;
using Gemini.Avalonia.Framework.Services;
using Gemini.Avalonia.Modules.ProjectManagement.ViewModels;

namespace Gemini.Avalonia.Modules.ProjectManagement
{
    /// <summary>
    /// 项目管理模块 - 支持延迟加载
    /// </summary>
    public class Module : LazyModuleBase
    {
        private ProjectExplorerToolViewModel? _projectExplorer;
        
        /// <summary>
        /// 创建模块元数据
        /// </summary>
        /// <returns>模块元数据</returns>
        protected override ModuleMetadata CreateMetadata()
        {
            return new ModuleMetadata
            {
                Name = "ProjectManagementModule",
                Description = "项目管理模块，提供项目创建、打开、管理功能",
                Category = ModuleCategory.Feature,
                Priority = 100,
                AllowLazyLoading = true,
                ModuleType = GetType(),
                Dependencies = new List<string> { "MainMenuModule" }
            };
        }
        
        /// <summary>
        /// 默认工具类型集合
        /// </summary>
        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(ProjectExplorerToolViewModel);
            }
        }
        
        /// <summary>
        /// 检查是否应该加载此模块
        /// </summary>
        /// <returns>如果应该加载返回true</returns>
        public override bool ShouldLoad()
        {
            // 项目管理模块默认应该加载，但可以根据用户配置决定
            return true;
        }
        
        /// <summary>
        /// 初始化模块
        /// </summary>
        public override void Initialize()
        {
            if (!IsLoaded) return;
            
            base.Initialize();
            
            // 注册项目资源管理器工具
            var shell = IoC.Get<IShell>();
            _projectExplorer = IoC.Get<ProjectExplorerToolViewModel>();
            if (shell != null && _projectExplorer != null)
            {
                shell.RegisterTool(_projectExplorer);
                shell.ShowTool(_projectExplorer); // 默认显示项目管理器
            }
        }
        
        /// <summary>
        /// 模块卸载时的清理逻辑
        /// </summary>
        /// <returns>清理任务</returns>
        protected override async Task OnUnloadingAsync()
        {
            if (_projectExplorer != null)
            {
                // 隐藏项目资源管理器
                var shell = IoC.Get<IShell>();
                shell?.HideTool(_projectExplorer);
                _projectExplorer = null;
            }
            
            await base.OnUnloadingAsync();
        }
    }
}