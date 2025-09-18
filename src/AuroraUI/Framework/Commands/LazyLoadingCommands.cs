using System.ComponentModel.Composition;
using System.Threading.Tasks;
using AuroraUI.Framework.Commands;
using AuroraUI.Framework.Logging;
using AuroraUI.Framework.Modules;

namespace AuroraUI.Framework.Commands
{
    /// <summary>
    /// 延迟加载模块相关命令定义
    /// </summary>
    [Export(typeof(CommandDefinition))]
    public class LoadProjectManagementModuleCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Module.LoadProjectManagement";
        
        public override string Name => CommandName;
        public override string Text => "加载项目管理模块";
        public override string ToolTip => "按需加载项目管理模块";
    }
    
    [Export(typeof(CommandDefinition))]
    public class LoadOutputModuleCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Module.LoadOutput";
        
        public override string Name => CommandName;
        public override string Text => "加载输出模块";
        public override string ToolTip => "按需加载输出模块";
    }
    
    [Export(typeof(CommandDefinition))]
    public class LoadPropertiesModuleCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Module.LoadProperties";
        
        public override string Name => CommandName;
        public override string Text => "加载属性模块";
        public override string ToolTip => "按需加载属性模块";
    }
    
    [Export(typeof(CommandDefinition))]
    public class LoadAllFeatureModulesCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Module.LoadAllFeatures";
        
        public override string Name => CommandName;
        public override string Text => "加载所有功能模块";
        public override string ToolTip => "一次性加载所有功能模块";
    }
}

namespace AuroraUI.Framework.Commands
{
    /// <summary>
    /// 延迟加载模块相关命令处理器
    /// </summary>
    [Export(typeof(ICommandHandler))]
    public class LoadProjectManagementModuleCommandHandler : CommandHandlerBase<LoadProjectManagementModuleCommandDefinition>
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        
        public override async Task Run(Command command)
        {
            Logger.Info("正在按需加载项目管理模块...");
            var bootstrapper = IoC.Get<AppBootstrapper>();
            var result = await bootstrapper.LoadFeatureModuleAsync("ProjectManagementModule");
            
            if (result)
            {
                Logger.Info("项目管理模块加载成功");
            }
            else
            {
                Logger.Warning("项目管理模块加载失败");
            }
        }
    }
    
    [Export(typeof(ICommandHandler))]
    public class LoadOutputModuleCommandHandler : CommandHandlerBase<LoadOutputModuleCommandDefinition>
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        
        public override async Task Run(Command command)
        {
            Logger.Info("正在按需加载输出模块...");
            var bootstrapper = IoC.Get<AppBootstrapper>();
            var result = await bootstrapper.LoadFeatureModuleAsync("OutputModule");
            
            if (result)
            {
                Logger.Info("输出模块加载成功");
            }
            else
            {
                Logger.Warning("输出模块加载失败");
            }
        }
    }
    
    [Export(typeof(ICommandHandler))]
    public class LoadPropertiesModuleCommandHandler : CommandHandlerBase<LoadPropertiesModuleCommandDefinition>
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        
        public override async Task Run(Command command)
        {
            Logger.Info("正在按需加载属性模块...");
            var bootstrapper = IoC.Get<AppBootstrapper>();
            var result = await bootstrapper.LoadFeatureModuleAsync("PropertiesModule");
            
            if (result)
            {
                Logger.Info("属性模块加载成功");
            }
            else
            {
                Logger.Warning("属性模块加载失败");
            }
        }
    }
    
    [Export(typeof(ICommandHandler))]
    public class LoadAllFeatureModulesCommandHandler : CommandHandlerBase<LoadAllFeatureModulesCommandDefinition>
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        
        public override async Task Run(Command command)
        {
            Logger.Info("正在加载所有功能模块...");
            var bootstrapper = IoC.Get<AppBootstrapper>();
            await bootstrapper.LoadAllFeatureModulesAsync();
            Logger.Info("所有功能模块加载完成");
        }
    }
}
