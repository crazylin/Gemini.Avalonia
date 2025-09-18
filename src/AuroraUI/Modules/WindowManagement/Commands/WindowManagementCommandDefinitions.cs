using System;
using System.ComponentModel.Composition;
using AuroraUI.Framework.Commands;
using AuroraUI.Framework.Services;
using AuroraUI.Framework.Logging;

namespace AuroraUI.Modules.WindowManagement.Commands
{
    /// <summary>
    /// 显示项目管理器命令定义
    /// </summary>
    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class ShowProjectExplorerCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Window.ShowProjectExplorer";
        
        public override string Name => "View.ProjectExplorer";
        public override string Text => LocalizationService?.GetString("View.ProjectExplorer");
        public override string ToolTip => LocalizationService?.GetString("View.ProjectExplorer.ToolTip");
        public override Uri IconSource => new Uri("avares://AuroraUI/Assets/Icons/folder.svg");
    }
    
    /// <summary>
    /// 显示输出窗口命令定义
    /// </summary>
    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class ShowOutputCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Window.ShowOutput";
        
        public override string Name => "View.Output";
        public override string Text => LocalizationService?.GetString("View.Output");
        public override string ToolTip => LocalizationService?.GetString("View.Output.ToolTip");
        public override Uri IconSource => new Uri("avares://AuroraUI/Assets/Icons/console.svg");
    }
    
    /// <summary>
    /// 显示属性窗口命令定义
    /// </summary>
    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class ShowPropertiesCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Window.ShowProperties";
        
        public override string Name => "View.Properties";
        public override string Text => LocalizationService?.GetString("View.Properties");
        public override string ToolTip => LocalizationService?.GetString("View.Properties.ToolTip");
        public override Uri IconSource => new Uri("avares://AuroraUI/Assets/Icons/properties.svg");
    }
}