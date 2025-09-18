using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AuroraUI.Framework;
using AuroraUI.Framework.Commands;
using AuroraUI.Framework.Logging;
using AuroraUI.Framework.Modules;
using AuroraUI.Framework.Services;

namespace AuroraUI.Modules.MainMenu.Commands
{
    /// <summary>
    /// 关于应用程序命令定义
    /// </summary>
    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class AboutApplicationCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Help.About";
        
        public override string Name => CommandName;
        public override string Text => LocalizationService?.GetString("Help.About");
        public override string ToolTip => LocalizationService?.GetString("Help.About.ToolTip");
        public override Uri IconSource => new Uri("avares://AuroraUI/Assets/Icons/about.svg");
    }

    /// <summary>
    /// 关于应用程序命令处理器
    /// </summary>
    [Export(typeof(ICommandHandler))]
    public class AboutApplicationCommandHandler : CommandHandlerBase<AboutApplicationCommandDefinition>
    {
        [Import]
        private IShell? _shell;

        public override async Task Run(Command command)
        {
            try
            {
                LogManager.Info("AboutApplicationCommand", "显示关于对话框");
                
                // 创建关于对话框
                var aboutDialog = new AboutDialog();
                
                // 获取主窗口作为父窗口
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
                    desktop.MainWindow != null)
                {
                    await aboutDialog.ShowDialog(desktop.MainWindow);
                }
                else
                {
                    LogManager.Warning("AboutApplicationCommand", "无法获取主窗口，以独立窗口显示");
                    aboutDialog.Show();
                }
                
                LogManager.Info("AboutApplicationCommand", "关于对话框已显示");
            }
            catch (Exception ex)
            {
                LogManager.Error("AboutApplicationCommand", $"显示关于对话框失败: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 模块列表命令定义
    /// </summary>
    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class ModuleListCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Help.ModuleList";
        
        public override string Name => CommandName;
        public override string Text => LocalizationService?.GetString("Help.ModuleList");
        public override string ToolTip => LocalizationService?.GetString("Help.ModuleList.ToolTip");
        public override Uri IconSource => new Uri("avares://AuroraUI/Assets/Icons/modules.svg");
    }

    /// <summary>
    /// 模块列表命令处理器
    /// </summary>
    [Export(typeof(ICommandHandler))]
    public class ModuleListCommandHandler : CommandHandlerBase<ModuleListCommandDefinition>
    {
        [Import]
        private IModuleManager? _moduleManager;

        public override async Task Run(Command command)
        {
            try
            {
                LogManager.Info("ModuleListCommand", "显示模块列表对话框");
                
                // 创建模块列表对话框
                var moduleListDialog = new ModuleListDialog(_moduleManager);
                
                // 获取主窗口作为父窗口
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
                    desktop.MainWindow != null)
                {
                    await moduleListDialog.ShowDialog(desktop.MainWindow);
                }
                else
                {
                    LogManager.Warning("ModuleListCommand", "无法获取主窗口，以独立窗口显示");
                    moduleListDialog.Show();
                }
                
                LogManager.Info("ModuleListCommand", "模块列表对话框已显示");
            }
            catch (Exception ex)
            {
                LogManager.Error("ModuleListCommand", $"显示模块列表对话框失败: {ex.Message}");
            }
        }
    }
}

/// <summary>
/// 关于对话框
/// </summary>
public class AboutDialog : Window
{
    public AboutDialog()
    {
        Title = "关于 AuroraUI";
        Width = 450;
        Height = 350;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        CanResize = false;
        
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version?.ToString() ?? "1.0.0.0";
        var copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "Copyright © 2024";
        
        var content = new StackPanel
        {
            Margin = new Avalonia.Thickness(20),
            Spacing = 15
        };
        
        // 应用程序标题
        content.Children.Add(new TextBlock
        {
            Text = "AuroraUI Framework",
            FontSize = 20,
            FontWeight = Avalonia.Media.FontWeight.Bold,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        });
        
        // 版本信息
        content.Children.Add(new TextBlock
        {
            Text = $"版本: {version}",
            FontSize = 14,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        });
        
        // 描述
        content.Children.Add(new TextBlock
        {
            Text = "基于 Avalonia UI 的现代化桌面应用程序开发框架",
            FontSize = 12,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap
        });
        
        // 版权信息
        content.Children.Add(new TextBlock
        {
            Text = copyright,
            FontSize = 11,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        });
        
        // 关闭按钮
        var closeButton = new Button
        {
            Content = "关闭",
            Width = 80,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Margin = new Avalonia.Thickness(0, 20, 0, 0)
        };
        closeButton.Click += (s, e) => Close();
        content.Children.Add(closeButton);
        
        Content = content;
    }
}

/// <summary>
/// 模块列表对话框
/// </summary>
public class ModuleListDialog : Window
{
    private readonly IModuleManager? _moduleManager;
    
    public ModuleListDialog(IModuleManager? moduleManager)
    {
        _moduleManager = moduleManager;
        Title = "已加载的模块列表";
        Width = 600;
        Height = 500;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        var content = new Grid();
        content.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        content.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        
        // 模块列表
        var listBox = new ListBox
        {
            Margin = new Avalonia.Thickness(10)
        };
        
        if (_moduleManager != null)
        {
            var modules = GetLoadedModules();
            foreach (var module in modules)
            {
                var item = new ListBoxItem();
                var panel = new StackPanel();
                
                // 模块名称
                panel.Children.Add(new TextBlock
                {
                    Text = module.Name,
                    FontWeight = Avalonia.Media.FontWeight.Bold,
                    FontSize = 14
                });
                
                // 模块类型
                panel.Children.Add(new TextBlock
                {
                    Text = $"类型: {module.Type}",
                    FontSize = 12,
                    Foreground = Avalonia.Media.Brushes.Gray
                });
                
                // 模块版本
                if (!string.IsNullOrEmpty(module.Version))
                {
                    panel.Children.Add(new TextBlock
                    {
                        Text = $"版本: {module.Version}",
                        FontSize = 12,
                        Foreground = Avalonia.Media.Brushes.Gray
                    });
                }
                
                // 模块描述
                if (!string.IsNullOrEmpty(module.Description))
                {
                    panel.Children.Add(new TextBlock
                    {
                        Text = module.Description,
                        FontSize = 11,
                        Foreground = Avalonia.Media.Brushes.DarkGray,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                        Margin = new Avalonia.Thickness(0, 5, 0, 0)
                    });
                }
                
                item.Content = panel;
                listBox.Items.Add(item);
            }
        }
        else
        {
            listBox.Items.Add(new ListBoxItem
            {
                Content = new TextBlock
                {
                    Text = "无法获取模块管理器",
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                }
            });
        }
        
        Grid.SetRow(listBox, 0);
        content.Children.Add(listBox);
        
        // 关闭按钮
        var closeButton = new Button
        {
            Content = "关闭",
            Width = 80,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Margin = new Avalonia.Thickness(10)
        };
        closeButton.Click += (s, e) => Close();
        Grid.SetRow(closeButton, 1);
        content.Children.Add(closeButton);
        
        Content = content;
    }
    
    private List<ModuleInfo> GetLoadedModules()
    {
        var modules = new List<ModuleInfo>();
        
        if (_moduleManager == null)
            return modules;
            
        try
        {
            // 通过反射获取已加载的模块信息
            var moduleManagerType = _moduleManager.GetType();
            var modulesField = moduleManagerType.GetField("_modules", BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (modulesField != null && modulesField.GetValue(_moduleManager) is IEnumerable<IModule> loadedModules)
            {
                foreach (var module in loadedModules)
                {
                    var moduleType = module.GetType();
                    var assembly = moduleType.Assembly;
                    var version = assembly.GetName().Version?.ToString();
                    
                    // 尝试获取模块属性
                    var moduleAttribute = moduleType.GetCustomAttribute<ModuleAttribute>();
                    
                    modules.Add(new ModuleInfo
                    {
                        Name = moduleType.Name,
                        Type = moduleType.FullName ?? moduleType.Name,
                        Version = version,
                        Description = moduleAttribute != null ? "已加载模块" : "标准模块"
                    });
                }
            }
        }
        catch (Exception ex)
        {
            LogManager.Error("ModuleListDialog", $"获取模块列表失败: {ex.Message}");
            modules.Add(new ModuleInfo
            {
                Name = "错误",
                Type = "无法获取模块信息",
                Description = ex.Message
            });
        }
        
        return modules.OrderBy(m => m.Name).ToList();
    }
    
    private class ModuleInfo
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string? Version { get; set; }
        public string? Description { get; set; }
    }
}
