using System.ComponentModel.Composition;
using Gemini.Avalonia.Framework;

namespace Gemini.Avalonia.Modules.ToolBars
{
    /// <summary>
    /// 工具栏模块
    /// </summary>
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        /// <summary>
        /// 初始化模块
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            
            // 工具栏模块初始化
            // 工具栏服务和构建器通过MEF自动注册
        }
    }
}