using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Framework.Commands;
using Gemini.Avalonia.Framework.ToolBars;
using Gemini.Avalonia.Modules.ToolBars.Models;

namespace Gemini.Avalonia.Modules.ToolBars
{
    [Export(typeof(IToolBarBuilder))]
    public class ToolBarBuilder : IToolBarBuilder
    {
        private readonly ICommandService _commandService;
        private readonly IModuleFilterService _moduleFilterService;
        private readonly ToolBarDefinition[] _toolBars;
        private readonly ToolBarItemGroupDefinition[] _toolBarItemGroups;
        private readonly ToolBarItemDefinition[] _toolBarItems;

        [ImportingConstructor]
        public ToolBarBuilder(
            ICommandService commandService,
            IModuleFilterService moduleFilterService,
            [ImportMany] ToolBarDefinition[] toolBars,
            [ImportMany] ToolBarItemGroupDefinition[] toolBarItemGroups,
            [ImportMany] ToolBarItemDefinition[] toolBarItems,
            [ImportMany] ExcludeToolBarDefinition[] excludeToolBars,
            [ImportMany] ExcludeToolBarItemGroupDefinition[] excludeToolBarItemGroups,
            [ImportMany] ExcludeToolBarItemDefinition[] excludeToolBarItems)
        {
            _commandService = commandService;
            _moduleFilterService = moduleFilterService;
            
            // 先应用排除过滤
            var filteredToolBars = toolBars
                .Where(x => !excludeToolBars.Select(y => y.ToolBarDefinitionToExclude).Contains(x))
                .ToArray();
            var filteredToolBarItemGroups = toolBarItemGroups
                .Where(x => !excludeToolBarItemGroups.Select(y => y.ToolBarItemGroupDefinitionToExclude).Contains(x))
                .ToArray();
            var filteredToolBarItems = toolBarItems
                .Where(x => !excludeToolBarItems.Select(y => y.ToolBarItemDefinitionToExclude).Contains(x))
                .ToArray();
            
            // 再应用模块过滤
            _toolBars = FilterByEnabledModules(filteredToolBars);
            _toolBarItemGroups = FilterByEnabledModules(filteredToolBarItemGroups);
            _toolBarItems = FilterByEnabledModules(filteredToolBarItems);
        }

        public void BuildToolBars(IToolBars result)
        {
            // 开始构建工具栏
            var toolBars = _toolBars.OrderBy(x => x.SortOrder);

            foreach (var toolBar in toolBars)
            {
                    var toolBarModel = new ToolBarModel();
                BuildToolBar(toolBar, toolBarModel);
                if (toolBarModel.Items.Any())
                {
                    result.Items.Add(toolBarModel);
                }
            }
        }

        public void BuildToolBar(ToolBarDefinition toolBarDefinition, IToolBar result)
        {
            var groups = _toolBarItemGroups
                .Where(x => x.ToolBar == toolBarDefinition)
                .OrderBy(x => x.SortOrder)
                .ToList();

            for (int i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var toolBarItems = _toolBarItems
                    .Where(x => x.Group == group)
                    .OrderBy(x => x.SortOrder);

                foreach (var toolBarItem in toolBarItems)
                {
                    if (toolBarItem.CommandDefinition == null)
            {
                continue;
            }
                    
                    switch (toolBarItem.ToolBarItemType)
                    {
                        case ToolBarItemType.Button:
                            var buttonCommand = _commandService.GetCommand(toolBarItem.CommandDefinition);
                            var buttonItem = new ButtonToolBarItem(toolBarItem, buttonCommand, result);
                            result.Add(buttonItem);
                            break;
                        case ToolBarItemType.ToggleButton:
                            var toggleCommand = _commandService.GetCommand(toolBarItem.CommandDefinition);
                            result.Add(new ToggleButtonToolBarItem(toolBarItem, toggleCommand, result));
                            // 添加切换按钮工具栏项
                            break;
                    }
                }
      
                if (i < groups.Count - 1 && toolBarItems.Any())
                    result.Add(new SeparatorToolBarItem());
            }
        }
        
        /// <summary>
        /// 根据启用的模块过滤工具栏定义
        /// </summary>
        /// <typeparam name="T">工具栏定义类型</typeparam>
        /// <param name="items">原始工具栏定义数组</param>
        /// <returns>过滤后的工具栏定义数组</returns>
        private T[] FilterByEnabledModules<T>(T[] items) where T : class
        {
            if (items.Length == 0) return items;
            
            var filteredItems = items.Where(item => _moduleFilterService.IsTypeFromEnabledModule(item.GetType())).ToArray();
            
            if (filteredItems.Length != items.Length)
            {
                System.Console.WriteLine($"ToolBarBuilder {typeof(T).Name}过滤: {items.Length} -> {filteredItems.Length} (移除了 {items.Length - filteredItems.Length} 个禁用模块的项目)");
            }
            
            return filteredItems;
        }
    }
}