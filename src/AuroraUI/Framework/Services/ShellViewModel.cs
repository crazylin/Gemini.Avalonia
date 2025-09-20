using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using AuroraUI.Framework.Services;
using AuroraUI.Framework.Logging;
using AuroraUI.Services;
using AuroraUI.Modules.StatusBar;
using AuroraUI.Modules.MainMenu;
using AuroraUI.Modules.ToolBars;
using AuroraUI.Modules.Theme.Services;
using Dock.Avalonia.Controls;
using Dock.Model.Core;
using AuroraUI.Modules.MainMenu.ViewModels;
using AuroraUI.Modules.ToolBars.ViewModels;
using AuroraUI.Views;

namespace AuroraUI.Framework.Services
{
    /// <summary>
    /// Shell服务的默认实现
    /// </summary>
    [Export(typeof(IShell))]
    public class ShellViewModel : ReactiveObject, IShell
    {
        private IDocument? _activeDocument;
        private ITool? _activeTool;
        private ILayoutItem? _activeLayoutItem;
        private ICommand? _closeCommand;
        private bool _showFloatingWindowsInTaskbar = true;
        private IDock? _layout;
        private ShellView? _mainWindow;
        
        [Import(AllowDefault = true)]
        private IThemeService? _themeService;
        
        /// <summary>
        /// 活动文档变更事件
        /// </summary>
        public event EventHandler? ActiveDocumentChanging;
        public event EventHandler? ActiveDocumentChanged;
        
        /// <summary>
        /// 主菜单
        /// </summary>
        public IMenu MainMenu { get; }
        
        /// <summary>
        /// 工具栏集合
        /// </summary>
        public IToolBars ToolBars { get; }
        
        /// <summary>
        /// 状态栏
        /// </summary>
        public IStatusBar StatusBar { get; }
        
        /// <summary>
        /// 文档集合
        /// </summary>
        public ObservableCollection<IDocument> Documents { get; } = new();
        
        /// <summary>
        /// 工具集合
        /// </summary>
        public ObservableCollection<ITool> Tools { get; } = new();
        
        /// <summary>
        /// 活动文档
        /// </summary>
        public IDocument? ActiveDocument
        {
            get => _activeDocument;
            set
            {
                if (_activeDocument == value) return;
                
                ActiveDocumentChanging?.Invoke(this, EventArgs.Empty);
                
                this.RaiseAndSetIfChanged(ref _activeDocument, value);
                
                ActiveDocumentChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        
        /// <summary>
        /// 活动工具
        /// </summary>
        public ITool? ActiveTool
        {
            get => _activeTool;
            set => this.RaiseAndSetIfChanged(ref _activeTool, value);
        }
        
        /// <summary>
        /// 活动布局项
        /// </summary>
        public ILayoutItem? ActiveLayoutItem
        {
            get => _activeLayoutItem;
            set => this.RaiseAndSetIfChanged(ref _activeLayoutItem, value);
        }
        
        /// <summary>
        /// 活动文档（IShell.ActiveItem的实现）
        /// </summary>
        public IDocument? ActiveItem => ActiveDocument;
        
        /// <summary>
        /// 是否在任务栏显示浮动窗口
        /// </summary>
        public bool ShowFloatingWindowsInTaskbar
        {
            get => _showFloatingWindowsInTaskbar;
            set => this.RaiseAndSetIfChanged(ref _showFloatingWindowsInTaskbar, value);
        }
        
        /// <summary>
        /// 主窗口视图
        /// </summary>
        public ShellView? MainWindow
        {
            get => _mainWindow;
            internal set => this.RaiseAndSetIfChanged(ref _mainWindow, value);
        }
        
        /// <summary>
        /// 关闭命令
        /// </summary>
        public ICommand CloseCommand
        {
            get => _closeCommand ??= ReactiveCommand.CreateFromTask(async () => await CloseAsync());
        }
        
        /// <summary>
        /// 停靠布局工厂
        /// </summary>
        public DockFactory DockFactory { get; }
        
        /// <summary>
        /// 停靠布局
        /// </summary>
        public IDock? Layout
        {
            get => _layout;
            set => this.RaiseAndSetIfChanged(ref _layout, value);
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        [ImportingConstructor]
        public ShellViewModel(IMenu mainMenu, IToolBars toolBars, IStatusBar statusBar)
        {
            LogManager.Debug("ShellViewModel", "构造函数被调用");
            
            MainMenu = mainMenu ?? throw new ArgumentNullException(nameof(mainMenu));
            ToolBars = toolBars ?? throw new ArgumentNullException(nameof(toolBars));
            StatusBar = statusBar ?? throw new ArgumentNullException(nameof(statusBar));
            DockFactory = new DockFactory();
            
            // 工具栏已通过MEF自动初始化
            
            // 初始化停靠布局
            Layout = DockFactory.CreateLayout();
            DockFactory.InitLayout(Layout);
            
            LogManager.Debug("ShellViewModel", "构造函数完成");
        }
        
        /// <summary>
        /// 初始化Shell（在MEF组合完成后调用）
        /// </summary>
        public void Initialize()
        {
            // 初始化主题服务
            _themeService?.Initialize();
            LogManager.Debug("ShellViewModel", "主题服务已初始化");
        }
        
        /// <summary>
        /// 注册工具
        /// </summary>
        public bool RegisterTool(ITool tool)
        {
            if (tool == null)
                throw new ArgumentNullException(nameof(tool));
                
            if (!Tools.Contains(tool))
            {
                // 初始化工具
                tool.Initialize();
                
                Tools.Add(tool);
                LogManager.Debug("ShellViewModel", $"RegisterTool: 添加工具到Tools集合: {tool.DisplayName}");
                
                // 添加到停靠布局
                if (tool is Tool toolInstance)
                {
                    LogManager.Debug("ShellViewModel", $"RegisterTool: 添加工具到dock布局: {tool.DisplayName}");
                    
                    // 根据工具类型确定停靠位置
                    var alignment = GetDockAlignmentForTool(tool);
                    DockFactory.AddTool(toolInstance, alignment);
                    LogManager.Debug("ShellViewModel", $"RegisterTool: 工具成功添加到dock布局，对齐方式: {alignment}");
                }
                else
                {
                    LogManager.Warning("ShellViewModel", $"RegisterTool: 工具不是Tool类型: {tool.GetType().Name}");
                }
                
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 显示工具
        /// </summary>
        public void ShowTool<T>() where T : ITool
        {
            var tool = Tools.OfType<T>().FirstOrDefault();
            if (tool != null)
            {
                tool.IsVisible = true;
                ActiveTool = tool;
            }
        }
        
        /// <summary>
        /// 根据工具类型获取停靠位置
        /// </summary>
        /// <param name="tool">工具实例</param>
        /// <returns>停靠位置</returns>
        public DockAlignment GetDockAlignmentForTool(ITool tool)
        {
            // 使用工具自身定义的首选位置
            return tool.PreferredLocation switch
            {
                PaneLocation.Left => DockAlignment.Left,
                PaneLocation.Right => DockAlignment.Right,
                PaneLocation.Bottom => DockAlignment.Bottom,
                _ => DockAlignment.Left // 默认左侧
            };
        }
        
        /// <summary>
        /// 显示工具
        /// </summary>
        public void ShowTool(ITool tool)
        {
            if (tool == null)
                throw new ArgumentNullException(nameof(tool));
                
            LogManager.Debug("ShellViewModel", $"ShowTool调用: {tool.DisplayName}");
        LogManager.Debug("ShellViewModel", $"工具已在Tools集合中: {Tools.Contains(tool)}");
        LogManager.Debug("ShellViewModel", $"工具当前IsVisible: {tool.IsVisible}");
                
            if (!Tools.Contains(tool))
            {
                Tools.Add(tool);
                LogManager.Debug("ShellViewModel", $"添加工具到Tools集合: {tool.DisplayName}");
            }
            
            // 确保工具在停靠布局中
            if (tool is Tool toolInstance)
            {
                LogManager.Debug("ShellViewModel", $"处理工具的dock布局: {tool.DisplayName}");
                
                // 根据工具类型确定停靠位置
                var alignment = GetDockAlignmentForTool(tool);
                var targetDock = alignment switch
                {
                    DockAlignment.Left => DockFactory.LeftToolDock,
                    DockAlignment.Right => DockFactory.RightToolDock,
                    DockAlignment.Bottom => DockFactory.BottomToolDock,
                    _ => DockFactory.LeftToolDock
                };
                
                LogManager.Debug("ShellViewModel", $"目标dock: {targetDock?.Id}, VisibleDockables数量: {targetDock?.VisibleDockables?.Count ?? 0}");
                
                // 检查工具是否已经在VisibleDockables中
                bool isInVisibleDockables = targetDock?.VisibleDockables?.Contains(toolInstance) ?? false;
                LogManager.Debug("ShellViewModel", $"工具在VisibleDockables中: {isInVisibleDockables}");
                
                if (!isInVisibleDockables)
                {
                    LogManager.Debug("ShellViewModel", $"添加工具到dock布局: {tool.DisplayName}");
                    DockFactory.AddTool(toolInstance, alignment);
                    LogManager.Debug("ShellViewModel", $"工具成功添加到dock布局，对齐方式: {alignment}");
                }
                else
                {
                    LogManager.Debug("ShellViewModel", $"工具已在dock布局中，确保其为活动状态");
                    // 如果工具已经在停靠布局中，确保它是活动的
                    if (targetDock != null)
                    {
                        targetDock.ActiveDockable = toolInstance;
                        LogManager.Debug("ShellViewModel", $"设置工具为活动dockable");
                    }
                }
            }
            else
            {
                LogManager.Warning("ShellViewModel", $"工具不是Tool类型: {tool.GetType().Name}");
            }
            
            tool.IsVisible = true;
            ActiveTool = tool;
            LogManager.Debug("ShellViewModel", $"ShowTool完成: {tool.DisplayName}, IsVisible: {tool.IsVisible}");
            
            // 更新面板可见性
            DockFactory.UpdatePanelVisibility();
        }
        
        /// <summary>
        /// 隐藏工具
        /// </summary>
        public void HideTool(ITool tool)
        {
            if (tool == null)
                throw new ArgumentNullException(nameof(tool));
                
            LogManager.Debug("ShellViewModel", $"HideTool调用: {tool.DisplayName}");
            
            // 从停靠布局中移除工具
            if (tool is Tool toolInstance)
            {
                LogManager.Debug("ShellViewModel", $"处理工具从dock布局中移除: {tool.DisplayName}");
                
                // 根据工具类型确定停靠位置
                var alignment = GetDockAlignmentForTool(tool);
                var targetDock = alignment switch
                {
                    DockAlignment.Left => DockFactory.LeftToolDock,
                    DockAlignment.Right => DockFactory.RightToolDock,
                    DockAlignment.Bottom => DockFactory.BottomToolDock,
                    _ => DockFactory.LeftToolDock
                };
                
                LogManager.Debug("ShellViewModel", $"目标dock: {targetDock?.Id}, VisibleDockables数量: {targetDock?.VisibleDockables?.Count ?? 0}");
            
            // 查找工具在VisibleDockables中的实例
            var dockableToRemove = targetDock?.VisibleDockables?.FirstOrDefault(d => d == toolInstance);
            LogManager.Debug("ShellViewModel", $"找到要移除的dockable: {dockableToRemove != null}");
                
                if (dockableToRemove != null)
                {
                    LogManager.Debug("ShellViewModel", $"从dock布局中移除工具: {tool.DisplayName}");
                targetDock?.VisibleDockables?.Remove(dockableToRemove);
                LogManager.Debug("ShellViewModel", $"工具成功从dock布局中移除。新数量: {targetDock?.VisibleDockables?.Count ?? 0}");
                    
                    // 如果当前工具是活动工具，设置其他工具为活动
                    if (targetDock?.ActiveDockable == dockableToRemove)
                    {
                        targetDock.ActiveDockable = targetDock.VisibleDockables?.FirstOrDefault();
                        LogManager.Debug("ShellViewModel", $"设置新的活动dockable: {targetDock.ActiveDockable?.Id}");
                    }
                }
                else
                {
                    LogManager.Debug("ShellViewModel", $"工具在dock布局中未找到: {tool.DisplayName}");
                LogManager.Debug("ShellViewModel", $"目标dock中可用的dockables: {string.Join(", ", targetDock?.VisibleDockables?.Select(d => d.Id) ?? new string[0])}");
                }
            }
            else
            {
                LogManager.Warning("ShellViewModel", $"工具不是Tool类型: {tool.GetType().Name}");
            }
            
            tool.IsVisible = false;
            
            // 如果隐藏的是当前活动工具，清除活动工具
            if (ActiveTool == tool)
            {
                ActiveTool = null;
                LogManager.Debug("ShellViewModel", $"清除活动工具");
            }
            
            LogManager.Debug("ShellViewModel", $"HideTool完成: {tool.DisplayName}, IsVisible: {tool.IsVisible}");
            
            // 更新面板可见性
            DockFactory.UpdatePanelVisibility();
        }
        
        /// <summary>
        /// 异步打开文档
        /// </summary>
        public async Task OpenDocumentAsync(IDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
                
            if (!Documents.Contains(document))
            {
                Documents.Add(document);
                // 添加到停靠布局
                if (document is Document documentInstance)
                    DockFactory.AddDocument(documentInstance);
            }
            
            ActiveDocument = document;
            await Task.CompletedTask;
        }
        
        /// <summary>
        /// 异步关闭文档
        /// </summary>
        public async Task CloseDocumentAsync(IDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
                
            if (Documents.Contains(document))
            {
                Documents.Remove(document);
                
                if (ActiveDocument == document)
                {
                    ActiveDocument = Documents.LastOrDefault();
                }
            }
            await Task.CompletedTask;
        }
        
        /// <summary>
        /// 关闭所有文档
        /// </summary>
        public async Task<bool> CloseAllDocumentsAsync()
        {
            var documentsToClose = Documents.ToList();
            
            foreach (var document in documentsToClose)
            {
                // 这里可以添加保存确认逻辑
                await CloseDocumentAsync(document);
            }
            
            return await Task.FromResult(true);
        }
        
        /// <summary>
        /// 关闭Shell
        /// </summary>
        public void Close()
        {
            // 异步关闭，但不等待结果
            _ = Task.Run(async () => await CloseAsync());
        }
        
        /// <summary>
        /// 异步关闭Shell
        /// </summary>
        public async Task<bool> CloseAsync()
        {
            // 先尝试关闭所有文档
            var canClose = await CloseAllDocumentsAsync();
            
            if (canClose)
            {
                // 清理资源
                Documents.Clear();
                Tools.Clear();
            }
            
            return canClose;
        }
    }
    
    /// <summary>
    /// 文档事件参数
    /// </summary>
    public class DocumentEventArgs : EventArgs
    {
        public IDocument? Document { get; }
        
        public DocumentEventArgs(IDocument? document)
        {
            Document = document;
        }
    }
}