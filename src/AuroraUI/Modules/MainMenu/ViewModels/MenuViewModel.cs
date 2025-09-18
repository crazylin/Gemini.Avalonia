using AuroraUI.Modules.MainMenu.Models;
using AuroraUI.Modules.ToolBars.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuroraUI.Framework.Logging;

namespace AuroraUI.Modules.MainMenu.ViewModels
{
    [Export(typeof(IMenu))]
    public class MenuViewModel : ReactiveObject, IMenu
    {
        private readonly IMenuBuilder _menuBuilder;
        private IMenu _menu;

        [ImportingConstructor]
        public MenuViewModel(IMenuBuilder menuBuilder)
        {
            LogManager.Debug("MenuViewModel", "构造函数开始");
            LogManager.Debug("MenuViewModel", $"MenuBuilder类型: {menuBuilder?.GetType().FullName ?? "null"}");
            
            _menuBuilder = menuBuilder;
            _menu = new MenuModel();
            
            LogManager.Debug("MenuViewModel", "即将调用MenuBuilder.BuildMenuBar");
            _menuBuilder.BuildMenuBar(MenuDefinitions.MainMenuBar, _menu);
            LogManager.Debug("MenuViewModel", $"MenuBuilder.BuildMenuBar调用完成，菜单项数量: {_menu.Count}");
            
            if (_menu.Count == 0)
            {
                LogManager.Warning("MenuViewModel", "警告: 没有菜单项被添加!");
            }
            else
            {
                LogManager.Info("MenuViewModel", $"成功创建 {_menu.Count} 个顶级菜单项");
            }
            
            this.RaisePropertyChanged(nameof(MenuModel));
            LogManager.Debug("MenuViewModel", "构造函数完成");
        }

        public IMenu Menu
        {
            get => _menu;
            set => this.RaiseAndSetIfChanged(ref _menu, value);
        }

        public IMenu Items => _menu;

        // ICollection<MenuItemBase> implementation
        public int Count => _menu.Count;
        public bool IsReadOnly => _menu.IsReadOnly;

        public void Add(MenuItemBase item) => _menu.Add(item);
        public void Clear() => _menu.Clear();
        public bool Contains(MenuItemBase item) => _menu.Contains(item);
        public void CopyTo(MenuItemBase[] array, int arrayIndex) => _menu.CopyTo(array, arrayIndex);
        public bool Remove(MenuItemBase item) => _menu.Remove(item);
        public IEnumerator<MenuItemBase> GetEnumerator() => _menu.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _menu.GetEnumerator();
    }
}
