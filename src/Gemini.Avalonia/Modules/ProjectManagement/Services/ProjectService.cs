using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gemini.Avalonia.Modules.ProjectManagement.Models;

namespace Gemini.Avalonia.Modules.ProjectManagement.Services
{
    /// <summary>
    /// 项目服务实现
    /// </summary>
    [Export(typeof(IProjectService))]
    public class ProjectService : IProjectService
    {
        private ProjectNode? _currentProject;
        private ProjectNode? _selectedItem;
        
        /// <summary>
        /// 当前项目根节点
        /// </summary>
        public ProjectNode? CurrentProject
        {
            get => _currentProject;
            private set
            {
                _currentProject = value;
                ProjectChanged?.Invoke(this, new ProjectChangedEventArgs(_currentProject, value));
            }
        }
        
        /// <summary>
        /// 是否有打开的项目
        /// </summary>
        public bool HasOpenProject => CurrentProject != null;
        
        /// <summary>
        /// 是否有选中的项目项
        /// </summary>
        public bool HasSelectedItem => SelectedItem != null;
        
        /// <summary>
        /// 当前选中的项目项
        /// </summary>
        public ProjectNode? SelectedItem
        {
            get => _selectedItem;
            set => _selectedItem = value;
        }
        
        /// <summary>
        /// 项目变更事件
        /// </summary>
        public event EventHandler<ProjectChangedEventArgs>? ProjectChanged;
        
        /// <summary>
        /// 项目项变更事件
        /// </summary>
        public event EventHandler<ProjectItemChangedEventArgs>? ProjectItemChanged;
        
        /// <summary>
        /// 创建新项目
        /// </summary>
        /// <param name="projectPath">项目路径</param>
        /// <returns>异步任务</returns>
        public async Task CreateNewProjectAsync(string projectPath)
        {
            try
            {
                Directory.CreateDirectory(projectPath);
                
                var projectName = Path.GetFileName(projectPath);
                var projectNode = new ProjectNode
                {
                    Name = projectName,
                    Path = projectPath,
                    Type = ProjectNodeType.Project
                };
                CurrentProject = projectNode;
                
                await SaveProjectAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"创建项目失败: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// 打开项目
        /// </summary>
        /// <param name="projectPath">项目文件路径</param>
        /// <returns>异步任务</returns>
        public async Task OpenProjectAsync(string projectPath)
        {
            try
            {
                if (!Directory.Exists(projectPath))
                {
                    throw new DirectoryNotFoundException($"项目目录不存在: {projectPath}");
                }
                
                var projectName = Path.GetFileName(projectPath);
                var projectNode = new ProjectNode
                {
                    Name = projectName,
                    Path = projectPath,
                    Type = ProjectNodeType.Project
                };
                
                await LoadProjectStructureAsync(projectNode, projectPath);
                CurrentProject = projectNode;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"打开项目失败: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// 关闭项目
        /// </summary>
        /// <returns>异步任务</returns>
        public async Task CloseProjectAsync()
        {
            try
            {
                if (CurrentProject != null)
                {
                    await SaveProjectAsync();
                    CurrentProject = null;
                    SelectedItem = null;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"关闭项目失败: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// 刷新项目
        /// </summary>
        /// <returns>异步任务</returns>
        public async Task RefreshProjectAsync()
        {
            try
            {
                if (CurrentProject != null)
                {
                    var projectPath = CurrentProject.Path;
                    CurrentProject.Children.Clear();
                    await LoadDirectoryStructureAsync(CurrentProject, projectPath);
                    ProjectItemChanged?.Invoke(this, new ProjectItemChangedEventArgs(ProjectItemChangeType.Modified, CurrentProject));
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"刷新项目失败: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// 保存项目
        /// </summary>
        /// <returns>异步任务</returns>
        public async Task SaveProjectAsync()
        {
            try
            {
                if (CurrentProject != null)
                {
                    // 这里可以实现项目文件的保存逻辑
                    // 例如保存项目配置、状态等
                    await Task.CompletedTask;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"保存项目失败: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// 向项目添加文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>异步任务</returns>
        public async Task AddFileToProjectAsync(string filePath)
        {
            try
            {
                if (CurrentProject == null)
                {
                    throw new InvalidOperationException("没有打开的项目");
                }
                
                if (!File.Exists(filePath))
                {
                    await File.WriteAllTextAsync(filePath, string.Empty);
                }
                
                var fileName = Path.GetFileName(filePath);
                var fileNode = new ProjectNode
                {
                    Name = fileName,
                    Path = filePath,
                    Type = ProjectNodeType.File
                };
                CurrentProject.AddChild(fileNode);
                
                ProjectItemChanged?.Invoke(this, new ProjectItemChangedEventArgs(ProjectItemChangeType.Added, fileNode));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"添加文件失败: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// 向项目添加文件夹
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <returns>异步任务</returns>
        public async Task AddFolderToProjectAsync(string folderPath)
        {
            try
            {
                if (CurrentProject == null)
                {
                    throw new InvalidOperationException("没有打开的项目");
                }
                
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                
                var folderName = Path.GetFileName(folderPath);
                var folderNode = new ProjectNode
                {
                    Name = folderName,
                    Path = folderPath,
                    Type = ProjectNodeType.Folder
                };
                await LoadDirectoryStructureAsync(folderNode, folderPath);
                CurrentProject.AddChild(folderNode);
                
                ProjectItemChanged?.Invoke(this, new ProjectItemChangedEventArgs(ProjectItemChangeType.Added, folderNode));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"添加文件夹失败: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// 删除选中项
        /// </summary>
        /// <returns>异步任务</returns>
        public async Task DeleteSelectedItemAsync()
        {
            try
            {
                if (SelectedItem == null || SelectedItem == CurrentProject)
                {
                    return;
                }
                
                var itemToDelete = SelectedItem;
                
                // 从文件系统删除
                if (itemToDelete.Type == ProjectNodeType.File)
                {
                    File.Delete(itemToDelete.Path);
                }
                else if (itemToDelete.Type == ProjectNodeType.Folder)
                {
                    Directory.Delete(itemToDelete.Path, true);
                }
                
                // 从项目树删除
                itemToDelete.Parent?.RemoveChild(itemToDelete);
                
                ProjectItemChanged?.Invoke(this, new ProjectItemChangedEventArgs(ProjectItemChangeType.Removed, itemToDelete));
                SelectedItem = null;
                
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"删除项目项失败: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// 获取项目文件
        /// </summary>
        /// <returns>项目文件集合</returns>
        public IEnumerable<ProjectNode> GetProjectFiles()
        {
            if (CurrentProject == null)
            {
                return Enumerable.Empty<ProjectNode>();
            }
            
            return GetAllNodes(CurrentProject)
                .Where(node => node.Type == ProjectNodeType.File);
        }
        
        /// <summary>
        /// 搜索项目文件
        /// </summary>
        /// <param name="searchText">搜索文本</param>
        /// <returns>匹配的文件集合</returns>
        public IEnumerable<ProjectNode> SearchProjectFiles(string searchText)
        {
            if (CurrentProject == null || string.IsNullOrWhiteSpace(searchText))
            {
                return Enumerable.Empty<ProjectNode>();
            }
            
            return GetAllNodes(CurrentProject)
                .Where(node => node.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }
        
        /// <summary>
        /// 加载项目结构
        /// </summary>
        /// <param name="projectNode">项目节点</param>
        /// <param name="projectDirectory">项目目录</param>
        /// <returns>异步任务</returns>
        private async Task LoadProjectStructureAsync(ProjectNode projectNode, string projectDirectory)
        {
            try
            {
                await LoadDirectoryStructureAsync(projectNode, projectDirectory);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"加载项目结构失败: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// 加载目录结构
        /// </summary>
        /// <param name="parentNode">父节点</param>
        /// <param name="directoryPath">目录路径</param>
        /// <returns>异步任务</returns>
        private async Task LoadDirectoryStructureAsync(ProjectNode parentNode, string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    return;
                }
                
                // 加载子目录
                var directories = Directory.GetDirectories(directoryPath)
                    .Where(dir => !ShouldIgnoreDirectory(dir))
                    .OrderBy(dir => Path.GetFileName(dir));
                
                foreach (var directory in directories)
                {
                    var dirName = Path.GetFileName(directory);
                    var dirNode = new ProjectNode
                    {
                        Name = dirName,
                        Path = directory,
                        Type = ProjectNodeType.Folder
                    };
                    parentNode.AddChild(dirNode);
                    
                    // 递归加载子目录
                    await LoadDirectoryStructureAsync(dirNode, directory);
                }
                
                // 加载文件
                var files = Directory.GetFiles(directoryPath)
                    .Where(file => !ShouldIgnoreFile(file))
                    .OrderBy(file => Path.GetFileName(file));
                
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    var fileNode = new ProjectNode
                    {
                        Name = fileName,
                        Path = file,
                        Type = ProjectNodeType.File
                    };
                    parentNode.AddChild(fileNode);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"加载目录结构失败: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// 获取所有节点
        /// </summary>
        /// <param name="node">根节点</param>
        /// <returns>所有节点</returns>
        private IEnumerable<ProjectNode> GetAllNodes(ProjectNode node)
        {
            yield return node;
            
            foreach (var child in node.Children)
            {
                foreach (var descendant in GetAllNodes(child))
                {
                    yield return descendant;
                }
            }
        }
        
        /// <summary>
        /// 是否应该忽略目录
        /// </summary>
        /// <param name="directoryPath">目录路径</param>
        /// <returns>是否忽略</returns>
        private bool ShouldIgnoreDirectory(string directoryPath)
        {
            var dirName = Path.GetFileName(directoryPath);
            var ignoredDirs = new[] { "bin", "obj", ".vs", ".git", "node_modules", "packages" };
            return ignoredDirs.Contains(dirName, StringComparer.OrdinalIgnoreCase);
        }
        
        /// <summary>
        /// 是否应该忽略文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>是否忽略</returns>
        private bool ShouldIgnoreFile(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var ignoredExtensions = new[] { ".user", ".suo", ".cache" };
            return ignoredExtensions.Any(ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }
    }
}