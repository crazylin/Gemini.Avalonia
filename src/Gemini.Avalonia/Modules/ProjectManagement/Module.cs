using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Styling;
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Framework.Services;
using Gemini.Avalonia.Modules.ProjectManagement.ViewModels;

namespace Gemini.Avalonia.Modules.ProjectManagement
{
    /// <summary>
    /// 项目管理模块
    /// </summary>
    public class Module : ModuleBase
    {
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
        /// 初始化模块
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }
        
        /// <summary>
        /// 异步后初始化
        /// </summary>
        /// <returns>异步任务</returns>
        public override async Task PostInitializeAsync()
        {
            await base.PostInitializeAsync();
        }
    }
}