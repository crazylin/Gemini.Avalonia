namespace Gemini.Avalonia.Framework.Modules
{
    /// <summary>
    /// 模块分类，用于控制加载策略
    /// </summary>
    public enum ModuleCategory
    {
        /// <summary>
        /// 核心模块，应用启动时必须加载
        /// </summary>
        Core,
        
        /// <summary>
        /// UI基础模块，界面显示时加载
        /// </summary>
        UI,
        
        /// <summary>
        /// 功能模块，首次使用时加载
        /// </summary>
        Feature,
        
        /// <summary>
        /// 扩展模块，用户主动启用时加载
        /// </summary>
        Extension
    }
}
