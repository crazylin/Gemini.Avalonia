using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuroraUI.Modules.ProjectManagement.Models;

namespace AuroraUI.Modules.ProjectManagement.Services
{
    /// <summary>
    /// 项目服务接口
    /// </summary>
    public interface IProjectService
    {
        /// <summary>
        /// 当前项目根节点
        /// </summary>
        ProjectNode? CurrentProject { get; }
        
        /// <summary>
        /// 是否有打开的项目
        /// </summary>
        bool HasOpenProject { get; }
        
        /// <summary>
        /// 是否有选中的项目项
        /// </summary>
        bool HasSelectedItem { get; }
        
        /// <summary>
        /// 当前选中的项目项
        /// </summary>
        ProjectNode? SelectedItem { get; set; }
        
        /// <summary>
        /// 项目变更事件
        /// </summary>
        event EventHandler<ProjectChangedEventArgs>? ProjectChanged;
        
        /// <summary>
        /// 项目项变更事件
        /// </summary>
        event EventHandler<ProjectItemChangedEventArgs>? ProjectItemChanged;
        
        /// <summary>
        /// 创建新项目
        /// </summary>
        /// <param name="projectPath">项目路径</param>
        /// <returns>异步任务</returns>
        Task CreateNewProjectAsync(string projectPath);
        
        /// <summary>
        /// 打开项目
        /// </summary>
        /// <param name="projectPath">项目文件路径</param>
        /// <returns>异步任务</returns>
        Task OpenProjectAsync(string projectPath);
        
        /// <summary>
        /// 关闭项目
        /// </summary>
        /// <returns>异步任务</returns>
        Task CloseProjectAsync();
        
        /// <summary>
        /// 刷新项目
        /// </summary>
        /// <returns>异步任务</returns>
        Task RefreshProjectAsync();
        
        /// <summary>
        /// 保存项目
        /// </summary>
        /// <returns>异步任务</returns>
        Task SaveProjectAsync();
        
        /// <summary>
        /// 向项目添加文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>异步任务</returns>
        Task AddFileToProjectAsync(string filePath);
        
        /// <summary>
        /// 向项目添加文件夹
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <returns>异步任务</returns>
        Task AddFolderToProjectAsync(string folderPath);
        
        /// <summary>
        /// 删除选中的项目项
        /// </summary>
        /// <returns>异步任务</returns>
        Task DeleteSelectedItemAsync();
        
        /// <summary>
        /// 获取项目文件列表
        /// </summary>
        /// <returns>项目文件列表</returns>
        IEnumerable<ProjectNode> GetProjectFiles();
        
        /// <summary>
        /// 搜索项目文件
        /// </summary>
        /// <param name="searchText">搜索文本</param>
        /// <returns>匹配的项目文件</returns>
        IEnumerable<ProjectNode> SearchProjectFiles(string searchText);
    }
    
    /// <summary>
    /// 项目变更事件参数
    /// </summary>
    public class ProjectChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 旧项目
        /// </summary>
        public ProjectNode? OldProject { get; }
        
        /// <summary>
        /// 新项目
        /// </summary>
        public ProjectNode? NewProject { get; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="oldProject">旧项目</param>
        /// <param name="newProject">新项目</param>
        public ProjectChangedEventArgs(ProjectNode? oldProject, ProjectNode? newProject)
        {
            OldProject = oldProject;
            NewProject = newProject;
        }
    }
    
    /// <summary>
    /// 项目项变更事件参数
    /// </summary>
    public class ProjectItemChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 变更类型
        /// </summary>
        public ProjectItemChangeType ChangeType { get; }
        
        /// <summary>
        /// 变更的项目项
        /// </summary>
        public ProjectNode Item { get; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="changeType">变更类型</param>
        /// <param name="item">变更的项目项</param>
        public ProjectItemChangedEventArgs(ProjectItemChangeType changeType, ProjectNode item)
        {
            ChangeType = changeType;
            Item = item;
        }
    }
    
    /// <summary>
    /// 项目项变更类型
    /// </summary>
    public enum ProjectItemChangeType
    {
        /// <summary>
        /// 添加
        /// </summary>
        Added,
        
        /// <summary>
        /// 删除
        /// </summary>
        Removed,
        
        /// <summary>
        /// 修改
        /// </summary>
        Modified,
        
        /// <summary>
        /// 重命名
        /// </summary>
        Renamed
    }
}