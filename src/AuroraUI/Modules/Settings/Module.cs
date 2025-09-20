using System.ComponentModel.Composition;
using AuroraUI.Framework;

namespace AuroraUI.Modules.Settings
{
    /// <summary>
    /// 设置模块 - 标准加载模式
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
            
            // 设置模块的命令和菜单项通过MEF自动注册
            // ApplicationSettingsViewModel 现在会在模块初始化时被注册
        }
    }
}