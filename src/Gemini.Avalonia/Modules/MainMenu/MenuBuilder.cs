using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Framework.Commands;
using Gemini.Avalonia.Framework.Menus;
using Gemini.Avalonia.Framework.Logging;
using Gemini.Avalonia.Modules.MainMenu.Models;

namespace Gemini.Avalonia.Modules.MainMenu
{
    [Export(typeof(IMenuBuilder))]
    public class MenuBuilder : IMenuBuilder
    {
        private readonly ICommandService _commandService;
        private readonly IModuleFilterService _moduleFilterService;
        private readonly MenuDefinition[] _menus;
        private readonly MenuItemGroupDefinition[] _menuItemGroups;
        private readonly MenuItemDefinition[] _menuItems;
        private readonly MenuDefinition[] _excludeMenus;
        private readonly MenuItemGroupDefinition[] _excludeMenuItemGroups;
        private readonly MenuItemDefinition[] _excludeMenuItems;

        [ImportingConstructor]
        public MenuBuilder(
            ICommandService commandService,
            IModuleFilterService moduleFilterService,
            [ImportMany] MenuDefinition[] menus,
            [ImportMany] MenuItemGroupDefinition[] menuItemGroups,
            [ImportMany] MenuItemDefinition[] menuItems)
            //ExcludeMenuDefinition[] excludeMenus,
            //ExcludeMenuItemGroupDefinition[] excludeMenuItemGroups,
            //ExcludeMenuItemDefinition[] excludeMenuItems)
        {
            _commandService = commandService;
            _moduleFilterService = moduleFilterService;
            
            // 应用模块过滤
            _menus = FilterByEnabledModules(menus ?? Array.Empty<MenuDefinition>());
            _menuItemGroups = FilterByEnabledModules(menuItemGroups ?? Array.Empty<MenuItemGroupDefinition>());
            
            // 首先按模块过滤菜单项
            var moduleFilteredMenuItems = FilterByEnabledModules(menuItems ?? Array.Empty<MenuItemDefinition>());
            
            // 然后过滤掉引用不存在命令的菜单项
            _menuItems = FilterByExistingCommands(moduleFilteredMenuItems);
            
            _excludeMenus = Array.Empty<MenuDefinition>();
            _excludeMenuItemGroups = Array.Empty<MenuItemGroupDefinition>();
            _excludeMenuItems = Array.Empty<MenuItemDefinition>();
            
   
            if (_menus.Length == 0)
            {
                LogManager.Warning("MenuBuilder", "没有找到任何菜单定义! 这可能是MEF容器配置问题。");
            }
        }

        public void BuildMenuBar(MenuBarDefinition menuBarDefinition, IMenu result)
        {
            try
            {
                var menus = _menus
                    .Where(x => x.MenuBar == menuBarDefinition)
                    .Where(x => !_excludeMenus.Contains(x))
                    .OrderBy(x => x.SortOrder);

                foreach (var menu in menus)
                {
                    try
                    {
                        var menuModel = new TextMenuItem(menu);
                        AddGroupsRecursive(menu, menuModel);
                        result.Add(menuModel);
                    }
                    catch (Exception ex)
                    {
                        LogManager.Error("MenuBuilder", $"跳过菜单 {menu?.Header}: {ex.Message}");
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("MenuBuilder", $"BuildMenuBar失败: {ex.Message}");
            }
        }

        private void AddGroupsRecursive(MenuDefinitionBase menu, StandardMenuItem menuModel)
        {
            var groups = _menuItemGroups
                .Where(x => x.Parent == menu)
                .Where(x => !_excludeMenuItemGroups.Contains(x))
                .OrderBy(x => x.SortOrder)
                .ToList();

    
            for (int i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var menuItems = _menuItems
                    .Where(x => x.Group == group)
                    .Where(x => !_excludeMenuItems.Contains(x))
                    .OrderBy(x => x.SortOrder)
                    .ToList();

                foreach (var menuItem in menuItems)
                {
                    try
                    {
                        StandardMenuItem menuItemModel;
                        
                        // 如果是CommandMenuItemDefinition，先检查命令是否存在
                        if (menuItem.CommandDefinition != null)
                        {
                            try
                            {
                                var command = _commandService.GetCommand(menuItem.CommandDefinition);
                                if (command != null)
                                {
                                    menuItemModel = new CommandMenuItem(command, menuModel);
                                }
                                else
                                {
                                    // 命令不存在（可能被过滤掉了），跳过此菜单项
                                    LogManager.Debug("MenuBuilder", $"跳过菜单项，因为命令不存在: {menuItem.CommandDefinition?.GetType().Name}");
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                LogManager.Warning("MenuBuilder", $"获取命令异常，跳过菜单项: {menuItem.CommandDefinition?.GetType().Name}, 异常: {ex.Message}");
                                continue;
                            }
                        }
                        else
                        {
                            // 非命令菜单项，创建TextMenuItem
                            menuItemModel = new TextMenuItem(menuItem);
                        }
                        
                        // 只有当menuItem是MenuDefinitionBase类型时才递归处理子菜单
                        if (menuItem is MenuDefinitionBase submenu)
                        {
                            AddGroupsRecursive(submenu, menuItemModel);
                        }
                        menuModel.Add(menuItemModel);
                    }
                    catch (Exception ex)
                    {
                        LogManager.Error("MenuBuilder", $"跳过菜单项: {ex.Message}");
                        continue;
                    }
                }

                if (i < groups.Count - 1 && menuItems.Any())
                    menuModel.Add(new SeparatorMenuItem());
            }
        }
        
        /// <summary>
        /// 根据启用的模块过滤菜单定义
        /// </summary>
        /// <typeparam name="T">菜单定义类型</typeparam>
        /// <param name="items">原始菜单定义数组</param>
        /// <returns>过滤后的菜单定义数组</returns>
        private T[] FilterByEnabledModules<T>(T[] items) where T : class
        {
            if (items.Length == 0) return items;
            
            var filteredItems = items.Where(item => 
            {
                var itemType = item.GetType();
                var isEnabled = IsItemFromEnabledModule(item);
                
                // 显示详细的类型信息用于调试
                var typeInfo = itemType.FullName ?? itemType.Name;
                LogManager.Debug("MenuBuilder", $"菜单项类型: {typeInfo}, 命名空间: {itemType.Namespace}, {(isEnabled ? "已启用" : "已禁用")}");
                
                return isEnabled;
            }).ToArray();
            
            if (filteredItems.Length != items.Length)
            {
                LogManager.Info("MenuBuilder", $"{typeof(T).Name}过滤: {items.Length} -> {filteredItems.Length} (移除了 {items.Length - filteredItems.Length} 个禁用模块的项目)");
            }
            
            return filteredItems;
        }
        
        /// <summary>
        /// 检查菜单项是否来自启用的模块，特殊处理CommandMenuItemDefinition类型
        /// </summary>
        private bool IsItemFromEnabledModule(object item)
        {
            var itemType = item.GetType();
            
            // 对于CommandMenuItemDefinition<T>类型，检查其CommandDefinition的类型
            if (itemType.IsGenericType && itemType.GetGenericTypeDefinition().Name == "CommandMenuItemDefinition`1")
            {
                try
                {
                    // 获取泛型参数，这就是CommandDefinition的类型
                    var genericArguments = itemType.GetGenericArguments();
                    if (genericArguments.Length > 0)
                    {
                        var commandDefType = genericArguments[0];
                        
                        // 根据命令定义类型判断所属模块
                        var commandModule = GetCommandDefinitionModule(commandDefType);
                        if (!string.IsNullOrEmpty(commandModule))
                        {
                            var enabledModules = _moduleFilterService.GetEnabledModuleNames();
                            var isEnabled = enabledModules.Contains(commandModule);
                            
                            LogManager.Debug("MenuBuilder", $"特殊处理CommandMenuItemDefinition: 命令类型={commandDefType.Name}, 所属模块={commandModule}, {(isEnabled ? "已启用" : "已禁用")}");
                            return isEnabled;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Debug("MenuBuilder", $"检查CommandMenuItemDefinition失败: {ex.Message}");
                }
            }
            
            // 默认使用原有的过滤逻辑
            return _moduleFilterService.IsTypeFromEnabledModule(itemType);
        }
        
        /// <summary>
        /// 根据命令定义类型确定其所属模块
        /// </summary>
        private string? GetCommandDefinitionModule(Type commandDefType)
        {
            var typeName = commandDefType.Name;
            
            // 根据命令定义类型名称映射到模块
            if (typeName == "ShowProjectExplorerCommandDefinition")
                return "ProjectManagementModule";
            if (typeName == "ShowOutputCommandDefinition")
                return "OutputModule";
            if (typeName == "ShowPropertiesCommandDefinition")
                return "PropertiesModule";
                
            // 对于其他命令定义，使用命名空间判断
            return _moduleFilterService.GetModuleNameFromType(commandDefType);
        }
        
        /// <summary>
        /// 过滤掉引用不存在命令的菜单项
        /// </summary>
        private MenuItemDefinition[] FilterByExistingCommands(MenuItemDefinition[] menuItems)
        {
            var filteredItems = new List<MenuItemDefinition>();
            
            foreach (var menuItem in menuItems)
            {
                if (menuItem.CommandDefinition != null)
                {
                    // 检查命令是否存在
                    var command = _commandService.GetCommand(menuItem.CommandDefinition);
                    if (command != null)
                    {
                        filteredItems.Add(menuItem);
                        LogManager.Debug("MenuBuilder", $"保留菜单项，命令存在: {menuItem.CommandDefinition.GetType().Name}");
                    }
                    else
                    {
                        LogManager.Debug("MenuBuilder", $"过滤菜单项，命令不存在: {menuItem.CommandDefinition.GetType().Name}");
                    }
                }
                else
                {
                    // 非命令菜单项，保留
                    filteredItems.Add(menuItem);
                }
            }
            
            LogManager.Debug("MenuBuilder", $"命令过滤: 总菜单项={menuItems.Length}, 保留={filteredItems.Count}, 过滤={menuItems.Length - filteredItems.Count}");
            
            return filteredItems.ToArray();
        }
    }
}
