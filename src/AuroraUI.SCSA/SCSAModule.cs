using System.ComponentModel.Composition;
using AuroraUI.Framework;
using AuroraUI.Framework.Modules;
using AuroraUI.Framework.Logging;
using AuroraUI.Framework.Services;
using SCSA.ViewModels;

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
        
        // 在后初始化阶段注册工具，确保MEF容器已完全组装
        try
        {
            var shell = AuroraUI.Framework.IoC.Get<IShell>();
            if (shell != null)
            {
                var parameterTool = AuroraUI.Framework.IoC.Get<ParameterConfigurationToolViewModel>();
                if (parameterTool != null)
                {
                    shell.RegisterTool(parameterTool);
                    Logger.Info("参数配置工具已注册到Shell");
                }
                else
                {
                    Logger.Warning("无法获取ParameterConfigurationToolViewModel实例");
                }
                
            }
            else
            {
                Logger.Warning("无法获取IShell实例");
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"注册参数配置工具时发生错误: {ex.Message}", ex);
            Logger.Error($"异常详细信息: {ex}");
        }
        
        Logger.Info("SCSA模块后初始化完成");
    }
}
