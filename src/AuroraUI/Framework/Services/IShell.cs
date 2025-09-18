using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AuroraUI.Modules.MainMenu;
using AuroraUI.Modules.StatusBar;
using AuroraUI.Modules.ToolBars;
using ReactiveUI;
using Dock.Model.Core;
using AuroraUI.Services;
using AuroraUI.Views;

namespace AuroraUI.Framework.Services
{
    /// <summary>
    /// Shell接口，管理应用程序的主要UI结构
    /// </summary>
    public interface IShell : IReactiveObject
    {
        /// <summary>
        /// 活动文档变更前事件
        /// </summary>
        event EventHandler? ActiveDocumentChanging;
        
        /// <summary>
        /// 活动文档变更后事件
        /// </summary>
        event EventHandler? ActiveDocumentChanged;
        
        /// <summary>
        /// 是否在任务栏显示浮动窗口
        /// </summary>
        bool ShowFloatingWindowsInTaskbar { get; set; }
        
        /// <summary>
        /// 主窗口视图
        /// </summary>
        ShellView? MainWindow { get; }
        
        /// <summary>
        /// 主菜单
        /// </summary>
        IMenu MainMenu { get; }
        
        /// <summary>
        /// 工具栏集合
        /// </summary>
        IToolBars ToolBars { get; }
        
        /// <summary>
        /// 状态栏
        /// </summary>
        IStatusBar StatusBar { get; }
        
        /// <summary>
        /// 活动布局项
        /// </summary>
        ILayoutItem? ActiveLayoutItem { get; set; }
        
        /// <summary>
        /// 活动文档
        /// </summary>
        IDocument? ActiveItem { get; }
        
        /// <summary>
        /// 活动文档（别名）
        /// </summary>
        IDocument? ActiveDocument => ActiveItem;
        
        /// <summary>
        /// 文档集合
        /// </summary>
        ObservableCollection<IDocument> Documents { get; }
        
        /// <summary>
        /// 工具集合
        /// </summary>
        ObservableCollection<ITool> Tools { get; }
        
        /// <summary>
        /// 注册工具
        /// </summary>
        /// <param name="tool">工具实例</param>
        /// <returns>是否注册成功</returns>
        bool RegisterTool(ITool tool);
        
        /// <summary>
        /// 显示工具（泛型版本）
        /// </summary>
        /// <typeparam name="TTool">工具类型</typeparam>
        void ShowTool<TTool>() where TTool : ITool;
        
        /// <summary>
        /// 显示工具
        /// </summary>
        /// <param name="model">工具实例</param>
        void ShowTool(ITool model);
        
        /// <summary>
        /// 隐藏工具
        /// </summary>
        /// <param name="tool">工具实例</param>
        void HideTool(ITool tool);
        
        /// <summary>
        /// 异步打开文档
        /// </summary>
        /// <param name="model">文档实例</param>
        /// <returns>任务</returns>
        Task OpenDocumentAsync(IDocument model);
        
        /// <summary>
        /// 异步关闭文档
        /// </summary>
        /// <param name="document">文档实例</param>
        /// <returns>任务</returns>
        Task CloseDocumentAsync(IDocument document);
        
        /// <summary>
        /// 关闭Shell
        /// </summary>
        void Close();
        
        /// <summary>
        /// 异步关闭Shell
        /// </summary>
        /// <returns>是否成功关闭</returns>
        Task<bool> CloseAsync();
        
        /// <summary>
        /// 停靠布局工厂
        /// </summary>
        DockFactory DockFactory { get; }
        
        /// <summary>
        /// 停靠布局
        /// </summary>
        IDock? Layout { get; set; }
    }
}