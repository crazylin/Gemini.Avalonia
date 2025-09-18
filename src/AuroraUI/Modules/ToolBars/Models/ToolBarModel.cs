using System.Collections.ObjectModel;
using ReactiveUI;

namespace AuroraUI.Modules.ToolBars.Models
{
    public class ToolBarModel : ReactiveObject, IToolBar
    {
        private string _name = string.Empty;
        private bool _isVisible = true;
        
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        
        public bool IsVisible
        {
            get => _isVisible;
            set => this.RaiseAndSetIfChanged(ref _isVisible, value);
        }
        
        public ObservableCollection<ToolBarItemBase> Items { get; } = new ObservableCollection<ToolBarItemBase>();
        
        public void Add(ToolBarItemBase item)
        {
            Items.Add(item);
        }
    }
}
