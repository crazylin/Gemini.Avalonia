using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows.Input;
using ReactiveUI;
using AuroraUI.Framework.Services;

namespace AuroraUI.Modules.MainMenu.Models
{
    public class MenuItemBase : ReactiveObject,ICollection<MenuItemBase>
    {
        private ObservableCollection<MenuItemBase> _items = new ObservableCollection<MenuItemBase>();
        private string _header = string.Empty;
        private string _headerKey = string.Empty;
        private ICommand? _command;
        private string _keyGesture = string.Empty;
        private bool _isVisible = true;
        private Uri? _iconSource;
        private ILocalizationService? _localizationService;

        public ObservableCollection<MenuItemBase> Items
        {
            get => _items;
            set => _items = value;
        }
        
        public virtual string Header
        {
            get 
            {
                // 如果有HeaderKey，尝试从本地化服务获取翻译
                if (!string.IsNullOrEmpty(_headerKey) && _localizationService != null)
                {
                    return _localizationService.GetString(_headerKey, _header);
                }
                return _header;
            }
            set => this.RaiseAndSetIfChanged(ref _header, value);
        }
        
        public virtual string HeaderKey
        {
            get => _headerKey;
            set 
            {
                this.RaiseAndSetIfChanged(ref _headerKey, value);
                // 当HeaderKey改变时，通知Header属性也改变了
                this.RaisePropertyChanged(nameof(Header));
            }
        }
        
        public virtual ICommand? Command
        {
            get 
            {

                return _command;
            }
            set => this.RaiseAndSetIfChanged(ref _command, value);
        }
        
        public virtual string KeyGesture
        {
            get => _keyGesture;
            set => this.RaiseAndSetIfChanged(ref _keyGesture, value);
        }
        
        public virtual bool IsVisible
        {
            get => _isVisible;
            set => this.RaiseAndSetIfChanged(ref _isVisible, value);
        }
        
        public virtual Uri? IconSource
        {
            get => _iconSource;
            set => this.RaiseAndSetIfChanged(ref _iconSource, value);
        }

        public MenuItemBase()
        {
            // _items已在字段声明时初始化
        }
        
        public MenuItemBase(ILocalizationService localizationService) : this()
        {
            _localizationService = localizationService;
        }
        
        public void SetLocalizationService(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }
        public IEnumerator<MenuItemBase> GetEnumerator()
        {
           return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(MenuItemBase item)
        {
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(MenuItemBase item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(MenuItemBase[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(MenuItemBase item)
        {
           return _items.Remove(item);
        }

        public int Count =>_items.Count;
        public bool IsReadOnly => false;
    }
}
