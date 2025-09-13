using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
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
        private readonly MenuDefinition[] _menus;
        private readonly MenuItemGroupDefinition[] _menuItemGroups;
        private readonly MenuItemDefinition[] _menuItems;
        private readonly MenuDefinition[] _excludeMenus;
        private readonly MenuItemGroupDefinition[] _excludeMenuItemGroups;
        private readonly MenuItemDefinition[] _excludeMenuItems;

        [ImportingConstructor]
        public MenuBuilder(
            ICommandService commandService,
            [ImportMany] MenuDefinition[] menus,
            [ImportMany] MenuItemGroupDefinition[] menuItemGroups,
            [ImportMany] MenuItemDefinition[] menuItems)
            //ExcludeMenuDefinition[] excludeMenus,
            //ExcludeMenuItemGroupDefinition[] excludeMenuItemGroups,
            //ExcludeMenuItemDefinition[] excludeMenuItems)
        {
  
            _commandService = commandService;
            _menus = menus ?? Array.Empty<MenuDefinition>();
            _menuItemGroups = menuItemGroups ?? Array.Empty<MenuItemGroupDefinition>();
            _menuItems = menuItems ?? Array.Empty<MenuItemDefinition>();
            //_excludeMenus = excludeMenus.Select(x => x.MenuDefinitionToExclude).ToArray();
            //_excludeMenuItemGroups = excludeMenuItemGroups.Select(x => x.MenuItemGroupDefinitionToExclude).ToArray();
            //_excludeMenuItems = excludeMenuItems.Select(x => x.MenuItemDefinitionToExclude).ToArray();
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
                         var headerValue = menuItem.Header; // 这里会触发CommandMenuItemDefinition.Header的访问
                
                        StandardMenuItem menuItemModel;
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
                                   menuItemModel = new TextMenuItem(menuItem);
                                }
                            }
                            catch (Exception ex)
                            {
                                LogManager.Warning("MenuBuilder", $"获取命令异常，创建TextMenuItem: {menuItem.Header}, 异常: {ex.Message}");
                                menuItemModel = new TextMenuItem(menuItem);
                            }
                        }
                        else
                        {
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
                        LogManager.Error("MenuBuilder", $"跳过菜单项 {menuItem.Header}: {ex.Message}");
                        continue;
                    }
                }

                if (i < groups.Count - 1 && menuItems.Any())
                    menuModel.Add(new SeparatorMenuItem());
            }
        }
    }
}
