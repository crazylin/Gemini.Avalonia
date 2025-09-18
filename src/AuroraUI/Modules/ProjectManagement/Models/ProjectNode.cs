using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using ReactiveUI;

namespace AuroraUI.Modules.ProjectManagement.Models
{
    /// <summary>
    /// 项目节点类型
    /// </summary>
    public enum ProjectNodeType
    {
        /// <summary>
        /// 项目
        /// </summary>
        Project,
        
        /// <summary>
        /// 文件夹
        /// </summary>
        Folder,
        
        /// <summary>
        /// 文件
        /// </summary>
        File
    }
    
    /// <summary>
    /// 项目节点模型
    /// </summary>
    public class ProjectNode : ReactiveObject
    {
        private string _name = string.Empty;
        private string _path = string.Empty;
        private bool _isExpanded;
        private bool _isSelected;
        private ProjectNode? _parent;
        
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        
        /// <summary>
        /// 节点路径
        /// </summary>
        public string Path
        {
            get => _path;
            set => this.RaiseAndSetIfChanged(ref _path, value);
        }
        
        /// <summary>
        /// 节点类型
        /// </summary>
        public ProjectNodeType Type { get; set; }
        
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
        }
        
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }
        
        /// <summary>
        /// 父节点
        /// </summary>
        public ProjectNode? Parent
        {
            get => _parent;
            set => this.RaiseAndSetIfChanged(ref _parent, value);
        }
        
        /// <summary>
        /// 子节点集合
        /// </summary>
        public ObservableCollection<ProjectNode> Children { get; }
        
        /// <summary>
        /// 图标路径
        /// </summary>
        public string IconPath
        {
            get
            {
                return Type switch
                {
                    ProjectNodeType.Project => "avares://AuroraUI/Assets/Icons/project.svg",
                    ProjectNodeType.Folder => "avares://AuroraUI/Assets/Icons/folder.svg",
                    ProjectNodeType.File => GetFileIcon()
                };
            }
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public ProjectNode()
        {
            Children = new ObservableCollection<ProjectNode>();
            
            // 当展开状态改变时，更新图标
            this.WhenAnyValue(x => x.IsExpanded)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(IconPath)));
        }
        
        /// <summary>
        /// 获取文件图标
        /// </summary>
        /// <returns>文件图标</returns>
        private string GetFileIcon()
        {
            var extension = System.IO.Path.GetExtension(Name).ToLower();
            return extension switch
            {
                ".cs" => "avares://AuroraUI/Assets/Icons/csharp-file.svg",
                ".axaml" or ".xaml" => "avares://AuroraUI/Assets/Icons/xaml-file.svg",
                ".csproj" or ".sln" => "avares://AuroraUI/Assets/Icons/project-file.svg",
                ".json" => "avares://AuroraUI/Assets/Icons/json-file.svg",
                ".xml" => "avares://AuroraUI/Assets/Icons/text-file.svg",
                ".txt" => "avares://AuroraUI/Assets/Icons/text-file.svg",
                ".md" => "avares://AuroraUI/Assets/Icons/text-file.svg",
                ".png" or ".jpg" or ".jpeg" or ".gif" or ".bmp" => "avares://AuroraUI/Assets/Icons/image-file.svg",
                ".dll" or ".exe" => "avares://AuroraUI/Assets/Icons/default-file.svg",
                _ => "avares://AuroraUI/Assets/Icons/default-file.svg"
            };
        }
        
        /// <summary>
        /// 获取完整路径
        /// </summary>
        /// <returns>完整路径</returns>
        public string GetFullPath()
        {
            if (Parent == null)
                return Path;
            
            return System.IO.Path.Combine(Parent.GetFullPath(), Name);
        }
        
        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="child">子节点</param>
        public void AddChild(ProjectNode child)
        {
            child.Parent = this;
            Children.Add(child);
        }
        
        /// <summary>
        /// 移除子节点
        /// </summary>
        /// <param name="child">子节点</param>
        public void RemoveChild(ProjectNode child)
        {
            child.Parent = null;
            Children.Remove(child);
        }
        
        /// <summary>
        /// 查找子节点
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <returns>找到的节点</returns>
        public ProjectNode? FindChild(string name)
        {
            foreach (var child in Children)
            {
                if (child.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                    return child;
                
                var found = child.FindChild(name);
                if (found != null)
                    return found;
            }
            
            return null;
        }
        
        /// <summary>
        /// 获取所有子节点（递归）
        /// </summary>
        /// <returns>所有子节点</returns>
        public System.Collections.Generic.IEnumerable<ProjectNode> GetAllChildren()
        {
            foreach (var child in Children)
            {
                yield return child;
                
                foreach (var grandChild in child.GetAllChildren())
                {
                    yield return grandChild;
                }
            }
        }
        
        /// <summary>
        /// 重写ToString方法
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            return $"{IconPath} {Name}";
        }
    }
}