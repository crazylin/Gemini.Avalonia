using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using AuroraUI.Framework.Commands;
using AuroraUI.Framework.Logging;
using AuroraUI.Modules.ProjectManagement.Services;
using AuroraUI.Modules.ProjectManagement.ViewModels;

namespace AuroraUI.Modules.ProjectManagement.Commands
{
    /// <summary>
    /// 新建项目命令处理器
    /// </summary>
    [Export(typeof(ICommandHandler))]
    public class NewProjectCommandHandler : CommandHandlerBase<NewProjectCommandDefinition>
    {
        private readonly IProjectService _projectService;
        
        [ImportingConstructor]
        public NewProjectCommandHandler(IProjectService projectService)
        {
            _projectService = projectService;
        }
        
        public override void Update(Command command)
        {
            command.Enabled = true;
        }
        
        public override async Task Run(Command command)
        {
            try
            {
                // 获取主窗口
                var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null;
                
                if (mainWindow?.StorageProvider != null)
                {
                    // 选择项目保存位置
                    var folder = await mainWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                    {
                        Title = "选择项目保存位置",
                        AllowMultiple = false
                    });
                    
                    if (folder.Count > 0)
                    {
                        var projectPath = folder[0].Path.LocalPath;
                        await _projectService.CreateNewProjectAsync(projectPath);
                    }
                }
            }
            catch (Exception ex)
            {
                // 这里可以添加错误处理逻辑
                LogManager.Error("NewProjectCommandHandler", $"创建项目失败: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// 打开项目命令处理器
    /// </summary>
    [Export(typeof(ICommandHandler))]
    public class OpenProjectCommandHandler : CommandHandlerBase<OpenProjectCommandDefinition>
    {
        private readonly IProjectService _projectService;
        
        [ImportingConstructor]
        public OpenProjectCommandHandler(IProjectService projectService)
        {
            _projectService = projectService;
        }
        
        public override void Update(Command command)
        {
            command.Enabled = true;
        }
        
        public override async Task Run(Command command)
        {
            try
            {
                // 获取主窗口
                var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null;
                
                if (mainWindow?.StorageProvider != null)
                {
                    // 选择项目文件
                    var files = await mainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                    {
                        Title = "打开项目文件",
                        AllowMultiple = false,
                        FileTypeFilter = new[]
                        {
                            new FilePickerFileType("项目文件")
                            {
                                Patterns = new[] { "*.csproj", "*.vbproj", "*.fsproj", "*.sln" }
                            }
                        }
                    });
                    
                    if (files.Count > 0)
                    {
                        var projectPath = files[0].Path.LocalPath;
                        await _projectService.OpenProjectAsync(projectPath);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("OpenProjectCommandHandler", $"打开项目失败: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// 关闭项目命令处理器
    /// </summary>
    [Export(typeof(ICommandHandler))]
    public class CloseProjectCommandHandler : CommandHandlerBase<CloseProjectCommandDefinition>
    {
        private readonly IProjectService _projectService;
        
        [ImportingConstructor]
        public CloseProjectCommandHandler(IProjectService projectService)
        {
            _projectService = projectService;
        }
        
        public override void Update(Command command)
        {
            command.Enabled = _projectService.HasOpenProject;
        }
        
        public override async Task Run(Command command)
        {
            try
            {
                await _projectService.CloseProjectAsync();
            }
            catch (Exception ex)
            {
                LogManager.Error("CloseProjectCommandHandler", $"关闭项目失败: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// 刷新项目命令处理器
    /// </summary>
    [Export(typeof(ICommandHandler))]
    public class RefreshProjectCommandHandler : CommandHandlerBase<RefreshProjectCommandDefinition>
    {
        private readonly IProjectService _projectService;
        
        [ImportingConstructor]
        public RefreshProjectCommandHandler(IProjectService projectService)
        {
            _projectService = projectService;
        }
        
        public override void Update(Command command)
        {
            command.Enabled = _projectService.HasOpenProject;
        }
        
        public override async Task Run(Command command)
        {
            try
            {
                await _projectService.RefreshProjectAsync();
            }
            catch (Exception ex)
            {
                LogManager.Error("RefreshProjectCommandHandler", $"刷新项目失败: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// 添加文件命令处理器
    /// </summary>
    [Export(typeof(ICommandHandler))]
    public class AddFileCommandHandler : CommandHandlerBase<AddFileCommandDefinition>
    {
        private readonly IProjectService _projectService;
        
        [ImportingConstructor]
        public AddFileCommandHandler(IProjectService projectService)
        {
            _projectService = projectService;
        }
        
        public override void Update(Command command)
        {
            command.Enabled = _projectService.HasOpenProject;
        }
        
        public override async Task Run(Command command)
        {
            try
            {
                // 获取主窗口
                var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null;
                
                if (mainWindow?.StorageProvider != null)
                {
                    // 选择要添加的文件
                    var files = await mainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                    {
                        Title = "选择要添加的文件",
                        AllowMultiple = true
                    });
                    
                    foreach (var file in files)
                    {
                        var filePath = file.Path.LocalPath;
                        await _projectService.AddFileToProjectAsync(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("AddFileCommandHandler", $"添加文件失败: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// 添加文件夹命令处理器
    /// </summary>
    [Export(typeof(ICommandHandler))]
    public class AddFolderCommandHandler : CommandHandlerBase<AddFolderCommandDefinition>
    {
        private readonly IProjectService _projectService;
        
        [ImportingConstructor]
        public AddFolderCommandHandler(IProjectService projectService)
        {
            _projectService = projectService;
        }
        
        public override void Update(Command command)
        {
            command.Enabled = _projectService.HasOpenProject;
        }
        
        public override async Task Run(Command command)
        {
            try
            {
                // 获取主窗口
                var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null;
                
                if (mainWindow?.StorageProvider != null)
                {
                    // 选择要添加的文件夹
                    var folders = await mainWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                    {
                        Title = "选择要添加的文件夹",
                        AllowMultiple = true
                    });
                    
                    foreach (var folder in folders)
                    {
                        var folderPath = folder.Path.LocalPath;
                        await _projectService.AddFolderToProjectAsync(folderPath);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("AddFolderCommandHandler", $"添加文件夹失败: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// 删除项目项命令处理器
    /// </summary>
    [Export(typeof(ICommandHandler))]
    public class DeleteItemCommandHandler : CommandHandlerBase<DeleteItemCommandDefinition>
    {
        private readonly IProjectService _projectService;
        
        [ImportingConstructor]
        public DeleteItemCommandHandler(IProjectService projectService)
        {
            _projectService = projectService;
        }
        
        public override void Update(Command command)
        {
            command.Enabled = _projectService.HasOpenProject && _projectService.HasSelectedItem;
        }
        
        public override async Task Run(Command command)
        {
            try
            {
                await _projectService.DeleteSelectedItemAsync();
            }
            catch (Exception ex)
            {
                LogManager.Error("DeleteItemCommandHandler", $"删除项目项失败: {ex.Message}");
            }
        }
    }
}