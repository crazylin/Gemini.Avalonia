using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System.Diagnostics;
using Gemini.Avalonia.Framework.Commands;
using Gemini.Avalonia.Framework.Logging;
using Gemini.Avalonia.Demo.Framework;

namespace Gemini.Avalonia.Demo.Commands
{
    /// <summary>
    /// Demo应用程序特有的命令定义
    /// </summary>
    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class LoadSampleDocumentModuleCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Demo.LoadSampleDocumentModule";
        
        public override string Name => CommandName;
        public override string Text => LocalizationService?.GetString("Command.Demo.LoadSampleDocumentModule");
        public override string ToolTip => LocalizationService?.GetString("Command.Demo.LoadSampleDocumentModule.ToolTip");
        public override Uri IconSource => new Uri("avares://Gemini.Avalonia/Assets/Icons/document.svg");
    }
    
    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class LoadAllDemoFeaturesCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Demo.LoadAllFeatures";
        
        public override string Name => CommandName;
        public override string Text => LocalizationService?.GetString("Command.Demo.LoadAllFeatures");
        public override string ToolTip => LocalizationService?.GetString("Command.Demo.LoadAllFeatures.ToolTip");
        public override Uri IconSource => new Uri("avares://Gemini.Avalonia/Assets/Icons/folder.svg");
    }
    
    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class ShowModuleStatusCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Demo.ShowModuleStatus";
        
        public override string Name => CommandName;
        public override string Text => LocalizationService?.GetString("Command.Demo.ShowModuleStatus");
        public override string ToolTip => LocalizationService?.GetString("Command.Demo.ShowModuleStatus.ToolTip");
        public override Uri IconSource => new Uri("avares://Gemini.Avalonia/Assets/Icons/properties.svg");
    }
}

namespace Gemini.Avalonia.Demo.Commands
{
    /// <summary>
    /// Demo应用程序命令处理器
    /// </summary>
    [Export(typeof(ICommandHandler))]
    public class LoadSampleDocumentModuleCommandHandler : CommandHandlerBase<LoadSampleDocumentModuleCommandDefinition>
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        
        private async Task ShowMessageAsync(string title, string message)
        {
            // 在控制台输出提示信息，作为临时的用户反馈
            Console.WriteLine($"🔔 {title}: {message}");
            Logger.Info("Demo", $"{title}: {message}");
            
            // 创建一个简单的对话框提示（未来可以改进为更好的UI）
            await Task.Delay(100);
        }
        
        public override async Task Run(Command command)
        {
            Logger.Info("Demo", "开始按需加载示例文档模块...");
            
            try
            {
                // 获取Demo引导器实例
                var app = Application.Current as App;
                if (app != null)
                {
                    // 通过反射获取私有字段 _bootstrapper
                    var bootstrapperField = typeof(App).GetField("_bootstrapper", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (bootstrapperField?.GetValue(app) is DemoBootstrapper bootstrapper)
                    {
                        var result = await bootstrapper.LoadDemoFeatureAsync("SampleDocumentModule");
                        
                        var message = result ? "示例文档模块加载成功！" : "示例文档模块已经加载或加载失败。";
                        await ShowMessageAsync("加载示例文档模块", message);
                        
                        if (result)
                        {
                            Logger.Info("Demo", "示例文档模块加载成功");
                        }
                        else
                        {
                            Logger.Warning("Demo", "示例文档模块加载失败");
                        }
                    }
                    else
                    {
                        await ShowMessageAsync("错误", "无法获取Demo引导器实例");
                        Logger.Error("Demo", "无法获取Demo引导器实例");
                    }
                }
            }
            catch (System.Exception ex)
            {
                await ShowMessageAsync("错误", $"加载示例文档模块时出错: {ex.Message}");
                Logger.Error("Demo", $"加载示例文档模块时出错: {ex.Message}");
            }
        }
    }
    
    [Export(typeof(ICommandHandler))]
    public class LoadAllDemoFeaturesCommandHandler : CommandHandlerBase<LoadAllDemoFeaturesCommandDefinition>
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        
        private async Task ShowMessageAsync(string title, string message)
        {
            // 在控制台输出提示信息，作为临时的用户反馈
            Console.WriteLine($"🔔 {title}: {message}");
            Logger.Info("Demo", $"{title}: {message}");
            
            // 创建一个简单的对话框提示（未来可以改进为更好的UI）
            await Task.Delay(100);
        }
        
        public override async Task Run(Command command)
        {
            Logger.Info("Demo", "开始加载所有Demo功能模块...");
            
            try
            {
                // 获取Demo引导器实例
                var app = Application.Current as App;
                if (app != null)
                {
                    var bootstrapperField = typeof(App).GetField("_bootstrapper", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (bootstrapperField?.GetValue(app) is DemoBootstrapper bootstrapper)
                    {
                        await bootstrapper.LoadAllDemoFeaturesAsync();
                        await ShowMessageAsync("加载Demo功能", "所有Demo功能模块加载完成！\n\n已加载的模块包括：\n• 项目管理模块\n• 输出窗口模块\n• 属性窗口模块\n• 设置模块");
                        Logger.Info("Demo", "所有Demo功能模块加载完成");
                    }
                    else
                    {
                        await ShowMessageAsync("错误", "无法获取Demo引导器实例");
                        Logger.Error("Demo", "无法获取Demo引导器实例");
                    }
                }
            }
            catch (System.Exception ex)
            {
                await ShowMessageAsync("错误", $"加载所有Demo功能模块时出错: {ex.Message}");
                Logger.Error("Demo", $"加载所有Demo功能模块时出错: {ex.Message}");
            }
        }
    }
    
    [Export(typeof(ICommandHandler))]
    public class ShowModuleStatusCommandHandler : CommandHandlerBase<ShowModuleStatusCommandDefinition>
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        
        private async Task ShowMessageAsync(string title, string message)
        {
            // 在控制台输出提示信息，作为临时的用户反馈
            Console.WriteLine($"🔔 {title}: {message}");
            Logger.Info("Demo", $"{title}: {message}");
            
            // 创建一个简单的对话框提示（未来可以改进为更好的UI）
            await Task.Delay(100);
        }
        
        public override async Task Run(Command command)
        {
            Logger.Info("Demo", "显示模块状态信息...");
            
            try
            {
                // 获取Demo引导器实例
                var app = Application.Current as App;
                if (app != null)
                {
                    var bootstrapperField = typeof(App).GetField("_bootstrapper", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (bootstrapperField?.GetValue(app) is DemoBootstrapper bootstrapper)
                    {
                        var moduleManager = bootstrapper.GetDemoModuleManager();
                        if (moduleManager != null)
                        {
                            var statusText = $"=== 模块状态报告 ===\n\n";
                            statusText += $"已注册模块数量: {moduleManager.RegisteredModules.Count}\n";
                            statusText += $"已加载模块数量: {moduleManager.LoadedModules.Count}\n\n";
                            statusText += "已加载的模块:\n";
                            
                            foreach (var module in moduleManager.LoadedModules)
                            {
                                statusText += $"• {module.Name} ({module.Category}) - 优先级: {module.Priority}\n";
                            }
                            
                            await ShowMessageAsync("模块状态", statusText);
                            
                            Logger.Info("Demo", "=== 模块状态报告 ===");
                            Logger.Info("Demo", $"已注册模块数量: {moduleManager.RegisteredModules.Count}");
                            Logger.Info("Demo", $"已加载模块数量: {moduleManager.LoadedModules.Count}");
                            
                            Logger.Info("Demo", "已加载的模块:");
                            foreach (var module in moduleManager.LoadedModules)
                            {
                                Logger.Info("Demo", $"  - {module.Name} ({module.Category}) - 优先级: {module.Priority}");
                            }
                            
                            Logger.Info("Demo", "==================");
                        }
                        else
                        {
                            await ShowMessageAsync("错误", "无法获取模块管理器实例");
                        }
                    }
                    else
                    {
                        await ShowMessageAsync("错误", "无法获取Demo引导器实例");
                    }
                }
            }
            catch (System.Exception ex)
            {
                await ShowMessageAsync("错误", $"显示模块状态时出错: {ex.Message}");
                Logger.Error("Demo", $"显示模块状态时出错: {ex.Message}");
            }
        }
    }
}
