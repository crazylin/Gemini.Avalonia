using System.ComponentModel.Composition;
using AuroraUI.Framework;
using AuroraUI.Framework.Services;

namespace AuroraUI.Modules.MainMenu
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