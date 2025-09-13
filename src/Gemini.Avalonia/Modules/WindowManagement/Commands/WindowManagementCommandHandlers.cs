using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Gemini.Avalonia.Framework.Commands;
using Gemini.Avalonia.Framework.Services;
using Gemini.Avalonia.Modules.ProjectManagement.ViewModels;
using Gemini.Avalonia.Modules.Output.ViewModels;
using Gemini.Avalonia.Modules.Properties.ViewModels;
using System;

namespace Gemini.Avalonia.Modules.WindowManagement.Commands
{
    /// <summary>
    /// 显示项目管理器命令处理器
    /// </summary>
    [CommandHandler]
    [Export(typeof(ICommandHandler))]
    public class ShowProjectExplorerCommandHandler : CommandHandlerBase<ShowProjectExplorerCommandDefinition>
    {
        private readonly IShell _shell;
        
        [ImportingConstructor]
        public ShowProjectExplorerCommandHandler(IShell shell)
        {
            _shell = shell;
        }
        
        public override Task Run(Command command)
        {
            var tool = _shell.Tools.OfType<ProjectExplorerToolViewModel>().FirstOrDefault();
            if (tool != null)
            {
                tool.IsVisible = !tool.IsVisible;
                if (tool.IsVisible)
                {
                    _shell.ShowTool(tool);
                }
                else
                {
                    _shell.HideTool(tool);
                }
            }
            return Task.CompletedTask;
        }
        
        public override void Update(Command command)
        {
            var tool = _shell.Tools.OfType<ProjectExplorerToolViewModel>().FirstOrDefault();
            command.Enabled = tool != null;
        }
    }
    
    /// <summary>
    /// 显示输出窗口命令处理器
    /// </summary>
    [CommandHandler]
    [Export(typeof(ICommandHandler))]
    public class ShowOutputCommandHandler : CommandHandlerBase<ShowOutputCommandDefinition>
    {
        private readonly IShell _shell;
        
        [ImportingConstructor]
        public ShowOutputCommandHandler(IShell shell)
        {
            _shell = shell;
        }
        
        public override Task Run(Command command)
        {
            var tool = _shell.Tools.OfType<OutputToolViewModel>().FirstOrDefault();
            if (tool != null)
            {
                tool.IsVisible = !tool.IsVisible;
                if (tool.IsVisible)
                {
                    _shell.ShowTool(tool);
                }
                else
                {
                    _shell.HideTool(tool);
                }
            }
            return Task.CompletedTask;
        }
        
        public override void Update(Command command)
        {
            var tool = _shell.Tools.OfType<OutputToolViewModel>().FirstOrDefault();
            command.Enabled = tool != null;
        }
    }
    
    /// <summary>
    /// 显示属性窗口命令处理器
    /// </summary>
    [CommandHandler]
    [Export(typeof(ICommandHandler))]
    public class ShowPropertiesCommandHandler : CommandHandlerBase<ShowPropertiesCommandDefinition>
    {
        private readonly IShell _shell;
        
        [ImportingConstructor]
        public ShowPropertiesCommandHandler(IShell shell)
        {
            _shell = shell;
        }
        
        public override Task Run(Command command)
        {
            var tool = _shell.Tools.OfType<PropertiesToolViewModel>().FirstOrDefault();
            if (tool != null)
            {
                tool.IsVisible = !tool.IsVisible;
                if (tool.IsVisible)
                {
                    _shell.ShowTool(tool);
                }
                else
                {
                    _shell.HideTool(tool);
                }
            }
            return Task.CompletedTask;
        }
        
        public override void Update(Command command)
        {
            var tool = _shell.Tools.OfType<PropertiesToolViewModel>().FirstOrDefault();
            command.Enabled = tool != null;
        }
    }
}