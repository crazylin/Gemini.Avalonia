using System.ComponentModel.Composition;
using Gemini.Avalonia.Framework;

namespace Gemini.Avalonia.Modules.WindowManagement
{
    /// <summary>
    /// 窗口管理模块
    /// </summary>
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        /// <summary>
        /// 初始化模块
        /// </summary>
        public override void Initialize()
        {
            // 窗口管理模块初始化
            // 菜单和命令通过MEF自动注册
        }
    }
}