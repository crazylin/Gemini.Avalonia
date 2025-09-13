using System.ComponentModel.Composition;
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Framework.Services;

namespace Gemini.Avalonia.Modules.MainMenu
{
    /// <summary>
    /// 主菜单模块
    /// </summary>
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        /// <summary>
        /// 模块初始化
        /// </summary>
        public override void Initialize()
        {
            // 主菜单模块初始化逻辑
        }
    }
}