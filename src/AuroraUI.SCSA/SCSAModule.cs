using System.ComponentModel.Composition;
using AuroraUI.Framework;
using AuroraUI.Framework.Modules;
using AuroraUI.Framework.Logging;

namespace SCSA;

/// <summary>
/// SCSA模块 - 提供智能状态感知与分析功能
/// </summary>
[Export(typeof(IModule))]
[ModuleAttribute]
public class SCSAModule : ModuleBase
{
    private static readonly ILogger Logger = LogManager.GetLogger("AuroraUI.SCSA.Module");

    /// <summary>
    /// 模块初始化
    /// </summary>
    public override void Initialize()
    {
        Logger.Info("SCSA模块正在初始化...");
        base.Initialize();
        Logger.Info("SCSA模块初始化完成");
    }

    /// <summary>
    /// 模块后初始化
    /// </summary>
    public override async Task PostInitializeAsync()
    {
        Logger.Info("SCSA模块正在进行后初始化...");
        await base.PostInitializeAsync();
        Logger.Info("SCSA模块后初始化完成");
    }
}
