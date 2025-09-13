using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Framework.Services;
using Gemini.Avalonia.Modules.ProjectManagement.Models;
using Gemini.Avalonia.Services;
using ReactiveUI;

namespace Gemini.Avalonia.Modules.ProjectManagement.ViewModels
{
    /// <summary>
    /// 项目资源管理器工具视图模型
    /// </summary>
    [Export(typeof(ITool))]
    public class ProjectExplorerToolViewModel : Tool
    {
        private ProjectNode? _selectedItem;
        private string _searchText = string.Empty;
        private ObservableCollection<ProjectNode> _filteredItems;
        
        /// <summary>
        /// 本地化服务
        /// </summary>
        [Import]
        public ILocalizationService LocalizationService { get; set; } = null!;
        
        /// <summary>
        /// 首选位置
        /// </summary>
        public override PaneLocation PreferredLocation => PaneLocation.Left;
        
        /// <summary>
        /// 首选宽度
        /// </summary>
        public override double PreferredWidth => 250.0;
        
        /// <summary>
        /// 项目节点集合
        /// </summary>
        public ObservableCollection<ProjectNode> Items { get; }
        
        /// <summary>
        /// 过滤后的项目节点集合
        /// </summary>
        public ObservableCollection<ProjectNode> FilteredItems
        {
            get => _filteredItems;
            set => this.RaiseAndSetIfChanged(ref _filteredItems, value);
        }
        
        /// <summary>
        /// 选中的项目节点
        /// </summary>
        public ProjectNode? SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }
        
        /// <summary>
        /// 搜索文本
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set
            {
                this.RaiseAndSetIfChanged(ref _searchText, value);
                FilterItems();
            }
        }
        
        /// <summary>
        /// 刷新命令
        /// </summary>
        public ICommand RefreshCommand { get; }
        
        /// <summary>
        /// 新建项目命令
        /// </summary>
        public ICommand NewProjectCommand { get; }
        
        /// <summary>
        /// 打开项目命令
        /// </summary>
        public ICommand OpenProjectCommand { get; }
        
        /// <summary>
        /// 关闭项目命令
        /// </summary>
        public ICommand CloseProjectCommand { get; }
        
        /// <summary>
        /// 添加文件命令
        /// </summary>
        public ICommand AddFileCommand { get; }
        
        /// <summary>
        /// 添加文件夹命令
        /// </summary>
        public ICommand AddFolderCommand { get; }
        
        /// <summary>
        /// 删除项目命令
        /// </summary>
        public ICommand DeleteItemCommand { get; }
        
        /// <summary>
        /// 重命名项目命令
        /// </summary>
        public ICommand RenameItemCommand { get; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public ProjectExplorerToolViewModel()
        {
            Items = new ObservableCollection<ProjectNode>();
            _filteredItems = new ObservableCollection<ProjectNode>();
            
            // 初始化命令
            RefreshCommand = ReactiveCommand.Create(RefreshProjects);
            NewProjectCommand = ReactiveCommand.Create(NewProject);
            OpenProjectCommand = ReactiveCommand.Create(OpenProject);
            CloseProjectCommand = ReactiveCommand.Create(CloseProject, this.WhenAnyValue(x => x.SelectedItem).Select(item => item?.Type == ProjectNodeType.Project));
            AddFileCommand = ReactiveCommand.Create(AddFile, this.WhenAnyValue(x => x.SelectedItem).Select(item => item != null));
            AddFolderCommand = ReactiveCommand.Create(AddFolder, this.WhenAnyValue(x => x.SelectedItem).Select(item => item != null));
            RenameItemCommand = ReactiveCommand.Create(RenameItem, this.WhenAnyValue(x => x.SelectedItem).Select(item => item != null && item.Type != ProjectNodeType.Project));
            DeleteItemCommand = ReactiveCommand.Create(DeleteItem, this.WhenAnyValue(x => x.SelectedItem).Select(item => item != null && item.Type != ProjectNodeType.Project));
            
            // 初始化示例数据
            InitializeSampleData();
            FilterItems();
        }
        
        /// <summary>
        /// 搜索水印文本
        /// </summary>
        public string SearchWatermarkText => LocalizationService?.GetString("ProjectExplorer.SearchWatermark") ;
        
        /// <summary>
        /// 初始化显示名称（在MEF注入完成后调用）
        /// </summary>
        public override void Initialize()
        {
            DisplayName = LocalizationService?.GetString("ProjectExplorer.Title");
            ToolTip = LocalizationService?.GetString("ProjectExplorer.ToolTip");
        }
        
        /// <summary>
        /// 初始化示例数据
        /// </summary>
        private void InitializeSampleData()
        {
            var sampleProject = new ProjectNode
            {
                Name = "示例项目",
                Type = ProjectNodeType.Project,
                Path = @"C:\Projects\SampleProject",
                IsExpanded = true
            };
            
            // 添加文件夹
            var srcFolder = new ProjectNode
            {
                Name = "src",
                Type = ProjectNodeType.Folder,
                Path = @"C:\Projects\SampleProject\src",
                Parent = sampleProject,
                IsExpanded = true
            };
            
            var viewsFolder = new ProjectNode
            {
                Name = "Views",
                Type = ProjectNodeType.Folder,
                Path = @"C:\Projects\SampleProject\src\Views",
                Parent = srcFolder
            };
            
            var viewModelsFolder = new ProjectNode
            {
                Name = "ViewModels",
                Type = ProjectNodeType.Folder,
                Path = @"C:\Projects\SampleProject\src\ViewModels",
                Parent = srcFolder
            };
            
            // 添加文件
            var mainFile = new ProjectNode
            {
                Name = "MainWindow.axaml",
                Type = ProjectNodeType.File,
                Path = @"C:\Projects\SampleProject\src\Views\MainWindow.axaml",
                Parent = viewsFolder
            };
            
            var mainViewModelFile = new ProjectNode
            {
                Name = "MainWindowViewModel.cs",
                Type = ProjectNodeType.File,
                Path = @"C:\Projects\SampleProject\src\ViewModels\MainWindowViewModel.cs",
                Parent = viewModelsFolder
            };
            
            var projectFile = new ProjectNode
            {
                Name = "SampleProject.csproj",
                Type = ProjectNodeType.File,
                Path = @"C:\Projects\SampleProject\SampleProject.csproj",
                Parent = sampleProject
            };
            
            // 构建层次结构
            viewsFolder.Children.Add(mainFile);
            viewModelsFolder.Children.Add(mainViewModelFile);
            srcFolder.Children.Add(viewsFolder);
            srcFolder.Children.Add(viewModelsFolder);
            sampleProject.Children.Add(srcFolder);
            sampleProject.Children.Add(projectFile);
            
            Items.Add(sampleProject);
        }
        
        /// <summary>
        /// 过滤项目
        /// </summary>
        private void FilterItems()
        {
            FilteredItems.Clear();
            
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var item in Items)
                {
                    FilteredItems.Add(item);
                }
            }
            else
            {
                foreach (var item in Items)
                {
                    var filteredItem = FilterNode(item, SearchText.ToLower());
                    if (filteredItem != null)
                    {
                        FilteredItems.Add(filteredItem);
                    }
                }
            }
        }
        
        /// <summary>
        /// 过滤节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="searchText">搜索文本</param>
        /// <returns>过滤后的节点</returns>
        private ProjectNode? FilterNode(ProjectNode node, string searchText)
        {
            var hasMatch = node.Name.ToLower().Contains(searchText);
            var filteredChildren = new ObservableCollection<ProjectNode>();
            
            foreach (var child in node.Children)
            {
                var filteredChild = FilterNode(child, searchText);
                if (filteredChild != null)
                {
                    filteredChildren.Add(filteredChild);
                    hasMatch = true;
                }
            }
            
            if (hasMatch)
            {
                var filteredNode = new ProjectNode
                {
                    Name = node.Name,
                    Type = node.Type,
                    Path = node.Path,
                    IsExpanded = node.IsExpanded || filteredChildren.Any(),
                    Parent = node.Parent
                };
                
                foreach (var child in filteredChildren)
                {
                    child.Parent = filteredNode;
                    filteredNode.Children.Add(child);
                }
                
                return filteredNode;
            }
            
            return null;
        }
        
        /// <summary>
        /// 刷新项目
        /// </summary>
        private void RefreshProjects()
        {
            // TODO: 实现项目刷新逻辑
            FilterItems();
        }
        
        /// <summary>
        /// 新建项目
        /// </summary>
        private void NewProject()
        {
            // TODO: 实现新建项目逻辑
        }
        
        /// <summary>
        /// 打开项目
        /// </summary>
        private void OpenProject()
        {
            // TODO: 实现打开项目逻辑
        }
        
        /// <summary>
        /// 关闭项目
        /// </summary>
        private void CloseProject()
        {
            if (SelectedItem?.Type == ProjectNodeType.Project)
            {
                Items.Remove(SelectedItem);
                FilterItems();
            }
        }
        
        /// <summary>
        /// 添加文件
        /// </summary>
        private void AddFile()
        {
            if (SelectedItem != null)
            {
                var defaultFileName = LocalizationService?.GetString("DefaultFileName") ?? "NewFile.txt";
                var newFile = new ProjectNode
                {
                    Name = defaultFileName,
                    Type = ProjectNodeType.File,
                    Path = System.IO.Path.Combine(SelectedItem.Path, defaultFileName),
                    Parent = SelectedItem.Type == ProjectNodeType.File ? SelectedItem.Parent : SelectedItem
                };
                
                var targetParent = SelectedItem.Type == ProjectNodeType.File ? SelectedItem.Parent : SelectedItem;
                targetParent?.Children.Add(newFile);
                FilterItems();
            }
        }
        
        /// <summary>
        /// 添加文件夹
        /// </summary>
        private void AddFolder()
        {
            if (SelectedItem != null)
            {
                var defaultFolderName = LocalizationService?.GetString("DefaultFolderName") ?? "NewFolder";
                var newFolder = new ProjectNode
                {
                    Name = defaultFolderName,
                    Type = ProjectNodeType.Folder,
                    Path = System.IO.Path.Combine(SelectedItem.Path, defaultFolderName),
                    Parent = SelectedItem.Type == ProjectNodeType.File ? SelectedItem.Parent : SelectedItem
                };
                
                var targetParent = SelectedItem.Type == ProjectNodeType.File ? SelectedItem.Parent : SelectedItem;
                targetParent?.Children.Add(newFolder);
                FilterItems();
            }
        }
        
        /// <summary>
        /// 删除项目
        /// </summary>
        private void DeleteItem()
        {
            if (SelectedItem != null && SelectedItem.Type != ProjectNodeType.Project)
            {
                SelectedItem.Parent?.Children.Remove(SelectedItem);
                FilterItems();
            }
        }
        
        /// <summary>
        /// 重命名项目
        /// </summary>
        private void RenameItem()
        {
            if (SelectedItem != null && SelectedItem.Type != ProjectNodeType.Project)
            {
                // 这里可以实现重命名逻辑，比如弹出对话框让用户输入新名称
                // 目前只是一个占位实现
                System.Diagnostics.Debug.WriteLine($"重命名项目: {SelectedItem.Name}");
            }
        }
    }
}