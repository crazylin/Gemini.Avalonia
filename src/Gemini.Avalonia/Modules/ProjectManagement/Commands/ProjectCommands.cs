using System;
using Gemini.Avalonia.Framework.Commands;

namespace Gemini.Avalonia.Modules.ProjectManagement.Commands
{
    /// <summary>
    /// 新建项目命令定义
    /// </summary>
    [CommandDefinition]
    public class NewProjectCommandDefinition : CommandDefinition
    {
        public override string Name => "Project.New";
        public override string Text => LocalizationService?.GetString("Project.New");
        public override string ToolTip => LocalizationService?.GetString("Project.New.ToolTip");
        public override Uri? IconSource => new Uri("avares://Gemini.Avalonia/Assets/Icons/NewProject.svg");
    }
    
    /// <summary>
    /// 打开项目命令定义
    /// </summary>
    [CommandDefinition]
    public class OpenProjectCommandDefinition : CommandDefinition
    {
        public override string Name => "Project.Open";
        public override string Text => LocalizationService?.GetString("Project.Open") ;
        public override string ToolTip => LocalizationService?.GetString("Project.Open.ToolTip");
        public override Uri? IconSource => new Uri("avares://Gemini.Avalonia/Assets/Icons/OpenProject.svg");
    }
    
    /// <summary>
    /// 关闭项目命令定义
    /// </summary>
    [CommandDefinition]
    public class CloseProjectCommandDefinition : CommandDefinition
    {
        public override string Name => "Project.Close";
        public override string Text => LocalizationService?.GetString("Project.Close");
        public override string ToolTip => LocalizationService?.GetString("Project.Close.ToolTip");
        public override Uri? IconSource => new Uri("avares://Gemini.Avalonia/Assets/Icons/CloseProject.svg");
    }
    
    /// <summary>
    /// 刷新项目命令定义
    /// </summary>
    [CommandDefinition]
    public class RefreshProjectCommandDefinition : CommandDefinition
    {
        public override string Name => "Project.Refresh";
        public override string Text => LocalizationService?.GetString("Project.Refresh");
        public override string ToolTip => LocalizationService?.GetString("Project.Refresh.ToolTip") ;
        public override Uri? IconSource => new Uri("avares://Gemini.Avalonia/Assets/Icons/Refresh.svg");
    }
    
    /// <summary>
    /// 添加文件命令定义
    /// </summary>
    [CommandDefinition]
    public class AddFileCommandDefinition : CommandDefinition
    {
        public override string Name => "Project.AddFile";
        public override string Text => LocalizationService?.GetString("Project.AddFile");
        public override string ToolTip => LocalizationService?.GetString("Project.AddFile.ToolTip");
        public override Uri? IconSource => new Uri("avares://Gemini.Avalonia/Assets/Icons/AddFile.svg");
    }
    
    /// <summary>
    /// 添加文件夹命令定义
    /// </summary>
    [CommandDefinition]
    public class AddFolderCommandDefinition : CommandDefinition
    {
        public override string Name => "Project.AddFolder";
        public override string Text => LocalizationService?.GetString("Project.AddFolder");
        public override string ToolTip => LocalizationService?.GetString("Project.AddFolder.ToolTip");
        public override Uri? IconSource => new Uri("avares://Gemini.Avalonia/Assets/Icons/AddFolder.svg");
    }
    
    /// <summary>
    /// 删除项目项命令定义
    /// </summary>
    [CommandDefinition]
    public class DeleteItemCommandDefinition : CommandDefinition
    {
        public override string Name => "Project.DeleteItem";
        public override string Text => LocalizationService?.GetString("Project.DeleteItem") ;
        public override string ToolTip => LocalizationService?.GetString("Project.DeleteItem.ToolTip") ;
        public override Uri? IconSource => new Uri("avares://Gemini.Avalonia/Assets/Icons/Delete.svg");
    }
}