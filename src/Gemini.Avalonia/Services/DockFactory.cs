using System;
using System.Collections.Generic;
using System.Linq;
using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;
using Dock.Model.ReactiveUI.Core;
using Gemini.Avalonia.Framework.Services;
using Gemini.Avalonia.Framework.Logging;
using GeminiFramework = Gemini.Avalonia.Framework;

namespace Gemini.Avalonia.Services;

public class DockFactory : Factory
{
    private IRootDock? _rootDock;
    private IDocumentDock? _documentDock;
    private IToolDock? _leftToolDock;
    private IToolDock? _rightToolDock;
    private IToolDock? _bottomToolDock;
    private IDock? _leftDock;
    private IDock? _rightDock;
    private IDock? _bottomDock;
    private IDock? _mainLayout;
    private IDock? _centerDock;
    
    // 保存面板的原始尺寸
    private double _leftDockOriginalProportion = 0.25;
    private double _rightDockOriginalProportion = 0.25;
    private double _bottomDockOriginalProportion = 0.25;

    public IToolDock? LeftToolDock => _leftToolDock;
    public IToolDock? RightToolDock => _rightToolDock;
    public IToolDock? BottomToolDock => _bottomToolDock;
    public IDock? LeftDock => _leftDock;
    public IDock? RightDock => _rightDock;
    public IDock? BottomDock => _bottomDock;
    public IDock? MainLayout => _mainLayout;

    public DockFactory()
    {
    }

    public override IDocumentDock CreateDocumentDock() => new DocumentDock();

    public override IRootDock CreateLayout()
    {
        // 创建默认主文档
        var homeDocument = new Document
        {
            Id = "Home",
            Title = "主页"
        };

        // 创建文档停靠区域
        var documentDock = new DocumentDock
        {
            Id = "Documents",
            Title = "Documents",
            IsCollapsable = false,
            CanCreateDocument = true,
            ActiveDockable = homeDocument,
            VisibleDockables = CreateList<IDockable>(homeDocument)
        };

        // 创建左侧工具停靠区域
        var leftToolDock = new ToolDock
        {
            Id = "LeftTools",
            Title = "Tools",
            Alignment = Alignment.Left,
            ActiveDockable = null,
            VisibleDockables = CreateList<IDockable>()
        };

        // 创建右侧工具停靠区域
        var rightToolDock = new ToolDock
        {
            Id = "RightTools",
            Title = "Properties",
            Alignment = Alignment.Right,
            ActiveDockable = null,
            VisibleDockables = CreateList<IDockable>()
        };

        // 创建底部工具停靠区域
        var bottomToolDock = new ToolDock
        {
            Id = "BottomTools",
            Title = "Output",
            Alignment = Alignment.Bottom,
            Proportion = 0.25,
            ActiveDockable = null,
            VisibleDockables = CreateList<IDockable>()
        };

        // 创建左侧面板
        _leftDock = new ProportionalDock
        {
            Proportion = _leftDockOriginalProportion,
            Orientation = Orientation.Vertical,
            ActiveDockable = null,
            VisibleDockables = CreateList<IDockable>(leftToolDock)
        };

        // 创建右侧面板
        _rightDock = new ProportionalDock
        {
            Proportion = _rightDockOriginalProportion,
            Orientation = Orientation.Vertical,
            ActiveDockable = null,
            VisibleDockables = CreateList<IDockable>(rightToolDock)
        };

        // 创建底部面板
        _bottomDock = new ProportionalDock
        {
            Proportion = _bottomDockOriginalProportion,
            Orientation = Orientation.Vertical,
            ActiveDockable = null,
            VisibleDockables = CreateList<IDockable>(bottomToolDock)
        };

        // 创建中央面板（仅文档区域）
        _centerDock = new ProportionalDock
        {
            Orientation = Orientation.Vertical,
            VisibleDockables = CreateList<IDockable>(documentDock)
        };
        var centerDock = _centerDock;

        // 创建水平布局（左侧、中央、右侧）
        var horizontalLayout = new ProportionalDock
        {
            Orientation = Orientation.Horizontal,
            VisibleDockables = CreateList<IDockable>
            (
                _leftDock,
                new ProportionalDockSplitter(),
                centerDock,
                new ProportionalDockSplitter(),
                _rightDock
            )
        };

        // 创建主布局（水平布局 + 底部面板）
        _mainLayout = new ProportionalDock
        {
            Id = "MainLayout",
            Orientation = Orientation.Vertical,
            VisibleDockables = CreateList<IDockable>
            (
                horizontalLayout,
                new ProportionalDockSplitter(),
                _bottomDock
            )
        };

        var rootDock = CreateRootDock();
        rootDock.IsCollapsable = false;
        rootDock.ActiveDockable = _mainLayout;
        rootDock.DefaultDockable = _mainLayout;
        rootDock.VisibleDockables = CreateList<IDockable>(_mainLayout);

        // 初始化固定停靠区域
        rootDock.LeftPinnedDockables = CreateList<IDockable>();
        rootDock.RightPinnedDockables = CreateList<IDockable>();
        rootDock.TopPinnedDockables = CreateList<IDockable>();
        rootDock.BottomPinnedDockables = CreateList<IDockable>();
        rootDock.PinnedDock = null;

        // 保存引用
        _rootDock = rootDock;
        _documentDock = documentDock;
        _leftToolDock = leftToolDock;
        _rightToolDock = rightToolDock;
        _bottomToolDock = bottomToolDock;

        return rootDock;
    }

    public override void InitLayout(IDockable layout)
    {
        ContextLocator = new Dictionary<string, Func<object?>>
        {
            ["Home"] = () => new HomeViewModel()
        };
        DockableLocator = new Dictionary<string, Func<IDockable?>>
        {
            ["Root"] = () => _rootDock,
            ["Documents"] = () => _documentDock,
            ["LeftTools"] = () => _leftToolDock,
            ["RightTools"] = () => _rightToolDock,
            ["BottomTools"] = () => _bottomToolDock
        };

        HostWindowLocator = new Dictionary<string, Func<IHostWindow?>>
        {
            [nameof(IDockWindow)] = () => new HostWindow()
        };

        base.InitLayout(layout);
    }

    public void AddDocument(GeminiFramework.Document document)
    {
        if (_documentDock != null)
        {
            var dockable = CreateDockableFromDocument(document);
            _documentDock.VisibleDockables?.Add(dockable);
            _documentDock.ActiveDockable = dockable;
        }
    }

    public void AddTool(GeminiFramework.Tool tool, DockAlignment alignment = DockAlignment.Left)
    {
        LogManager.Debug("DockFactory", $"AddTool调用: {tool.Title}, 对齐方式: {alignment}");
        var targetDock = alignment switch
        {
            DockAlignment.Left => _leftToolDock,
            DockAlignment.Right => _rightToolDock,
            DockAlignment.Bottom => _bottomToolDock,
            _ => _leftToolDock
        };

        if (targetDock != null)
        {
            var dockable = CreateDockableFromTool(tool);
            targetDock.VisibleDockables?.Add(dockable);
            if (targetDock.ActiveDockable == null)
                targetDock.ActiveDockable = dockable;
            LogManager.Info("DockFactory", $"工具已添加到dock: {tool.Title}");
        }
        else
        {
            LogManager.Warning("DockFactory", $"未找到目标dock，对齐方式: {alignment}");
        }
    }

    private IDockable CreateDockableFromDocument(GeminiFramework.Document document)
    {
        return new DocumentDockable
        {
            Id = document.Id.ToString(),
            Title = document.Title,
            CanClose = document.CanClose
        };
    }

    private IDockable CreateDockableFromTool(GeminiFramework.Tool tool)
    {
        // 直接返回工具本身，因为Tool类已经继承自Dock.Model.ReactiveUI.Controls.Tool
        // 这样工具的视图模型就会被正确地显示在停靠布局中
        return tool;
    }

    /// <summary>
    /// 更新面板可见性，当工具停靠区域为空时隐藏整个面板释放空间
    /// </summary>
    public void UpdatePanelVisibility()
    {
        if (_mainLayout?.VisibleDockables == null) return;
        
        // 获取水平布局（第一个元素）
        var horizontalLayout = _mainLayout.VisibleDockables.FirstOrDefault() as ProportionalDock;
        if (horizontalLayout?.VisibleDockables == null) return;

        // 检查左侧面板
        bool leftHasTools = _leftToolDock?.VisibleDockables?.Count > 0;
        
        if (!leftHasTools && _leftDock != null)
        {
            // 隐藏左侧面板
            if (horizontalLayout.VisibleDockables.Contains(_leftDock))
            {
                var leftIndex = horizontalLayout.VisibleDockables.IndexOf(_leftDock);
                horizontalLayout.VisibleDockables.Remove(_leftDock);
                // 移除左侧面板后面的分隔符
                if (leftIndex < horizontalLayout.VisibleDockables.Count && 
                    horizontalLayout.VisibleDockables[leftIndex] is ProportionalDockSplitter)
                {
                    horizontalLayout.VisibleDockables.RemoveAt(leftIndex);
                }
            }
        }
        else if (leftHasTools && _leftDock != null)
        {
            // 显示左侧面板
            if (!horizontalLayout.VisibleDockables.Contains(_leftDock))
            {
                // 恢复原始尺寸
                _leftDock.Proportion = _leftDockOriginalProportion;
                horizontalLayout.VisibleDockables.Insert(0, _leftDock);
                horizontalLayout.VisibleDockables.Insert(1, new ProportionalDockSplitter());
            }
        }
        
        // 检查右侧面板
        bool rightHasTools = _rightToolDock?.VisibleDockables?.Count > 0;
        
        if (!rightHasTools && _rightDock != null)
        {
            // 隐藏右侧面板
            if (horizontalLayout.VisibleDockables.Contains(_rightDock))
            {
                var rightIndex = horizontalLayout.VisibleDockables.IndexOf(_rightDock);
                horizontalLayout.VisibleDockables.Remove(_rightDock);
                // 移除右侧面板前面的分隔符
                if (rightIndex > 0 && 
                    horizontalLayout.VisibleDockables[rightIndex - 1] is ProportionalDockSplitter)
                {
                    horizontalLayout.VisibleDockables.RemoveAt(rightIndex - 1);
                }
            }
        }
        else if (rightHasTools && _rightDock != null)
        {
            // 显示右侧面板
            if (!horizontalLayout.VisibleDockables.Contains(_rightDock))
            {
                // 恢复原始尺寸
                _rightDock.Proportion = _rightDockOriginalProportion;
                horizontalLayout.VisibleDockables.Add(new ProportionalDockSplitter());
                horizontalLayout.VisibleDockables.Add(_rightDock);
            }
        }
        
        // 重新计算并设置面板比例，确保中央区域正确扩展
        RecalculateProportions(horizontalLayout);
        
        // 检查底部面板
        bool bottomHasTools = _bottomToolDock?.VisibleDockables?.Count > 0;
        
        if (!bottomHasTools && _bottomDock != null)
        {
            // 隐藏底部面板
            if (_mainLayout.VisibleDockables.Contains(_bottomDock))
            {
                var bottomIndex = _mainLayout.VisibleDockables.IndexOf(_bottomDock);
                _mainLayout.VisibleDockables.Remove(_bottomDock);
                // 移除底部面板前面的分隔符
                if (bottomIndex > 0 && 
                    _mainLayout.VisibleDockables[bottomIndex - 1] is ProportionalDockSplitter)
                {
                    _mainLayout.VisibleDockables.RemoveAt(bottomIndex - 1);
                }
            }
        }
        else if (bottomHasTools && _bottomDock != null)
        {
            // 显示底部面板
            if (!_mainLayout.VisibleDockables.Contains(_bottomDock))
            {
                // 恢复原始尺寸
                _bottomDock.Proportion = _bottomDockOriginalProportion;
                _mainLayout.VisibleDockables.Add(new ProportionalDockSplitter());
                _mainLayout.VisibleDockables.Add(_bottomDock);
            }
        }
    }
    
    /// <summary>
    /// 重新计算水平布局中各面板的比例，确保中央区域正确扩展
    /// </summary>
    /// <param name="horizontalLayout">水平布局</param>
    private void RecalculateProportions(ProportionalDock horizontalLayout)
    {
        if (horizontalLayout?.VisibleDockables == null || _centerDock == null) return;
        
        bool hasLeftPanel = horizontalLayout.VisibleDockables.Contains(_leftDock);
        bool hasRightPanel = horizontalLayout.VisibleDockables.Contains(_rightDock);
        
        // 设置最小宽度以确保面板可见，但允许调整大小
        const double minPanelWidth = 150.0; // 最小面板宽度
        
        if (hasLeftPanel && hasRightPanel)
        {
            // 左中右都存在：使用比例布局，允许调整大小
            if (_leftDock != null) 
            {
                _leftDock.MinWidth = minPanelWidth;
                _leftDock.MaxWidth = double.PositiveInfinity; // 允许无限扩展
                _leftDock.Proportion = _leftDockOriginalProportion; // 使用比例
            }
            if (_rightDock != null) 
            {
                _rightDock.MinWidth = minPanelWidth;
                _rightDock.MaxWidth = double.PositiveInfinity; // 允许无限扩展
                _rightDock.Proportion = _rightDockOriginalProportion; // 使用比例
            }
            _centerDock.Proportion = 1.0 - _leftDockOriginalProportion - _rightDockOriginalProportion; // 中央占用剩余空间
        }
        else if (hasLeftPanel && !hasRightPanel)
        {
            // 只有左中：使用比例布局，允许调整大小
            if (_leftDock != null) 
            {
                _leftDock.MinWidth = minPanelWidth;
                _leftDock.MaxWidth = double.PositiveInfinity;
                _leftDock.Proportion = _leftDockOriginalProportion;
            }
            _centerDock.Proportion = 1.0 - _leftDockOriginalProportion;
        }
        else if (!hasLeftPanel && hasRightPanel)
        {
            // 只有中右：使用比例布局，允许调整大小
            if (_rightDock != null) 
            {
                _rightDock.MinWidth = minPanelWidth;
                _rightDock.MaxWidth = double.PositiveInfinity;
                _rightDock.Proportion = _rightDockOriginalProportion;
            }
            _centerDock.Proportion = 1.0 - _rightDockOriginalProportion;
        }
        else
        {
            // 只有中央：中央占用全部空间
            _centerDock.Proportion = 1.0;
        }
    }
}

public enum DockAlignment
{
    Left,
    Right,
    Bottom
}

public class DocumentDockable : DockableBase
{
    public DocumentDockable()
    {
        CanClose = true;
        CanFloat = true;
        CanPin = false;
    }
}

public class ToolDockable : DockableBase
{
    public ToolDockable()
    {
        CanClose = true;
        CanFloat = true;
        CanPin = true;
    }
}