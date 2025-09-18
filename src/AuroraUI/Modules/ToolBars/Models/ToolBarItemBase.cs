using ReactiveUI;
using System;
using System.Windows.Input;

namespace AuroraUI.Modules.ToolBars.Models
{
	public class ToolBarItemBase : ReactiveObject
	{
        private string _name = "-";
        private string _text = string.Empty;
        private string _toolTip = string.Empty;
        private Uri? _iconSource;
        private ICommand? _command;
        private bool _isVisible = true;
        private bool _isEnabled = true;
        private bool _isChecked;
        private bool _hasToolTip;
        
        public virtual string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        
        public virtual string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }
        
        public virtual string ToolTip
        {
            get => _toolTip;
            set => this.RaiseAndSetIfChanged(ref _toolTip, value);
        }
        
        public virtual Uri? IconSource
        {
            get => _iconSource;
            set => this.RaiseAndSetIfChanged(ref _iconSource, value);
        }
        
        public virtual ICommand? Command
        {
            get => _command;
            set => this.RaiseAndSetIfChanged(ref _command, value);
        }
        
        public virtual bool IsVisible
        {
            get => _isVisible;
            set => this.RaiseAndSetIfChanged(ref _isVisible, value);
        }
        
        public virtual bool IsEnabled
        {
            get => _isEnabled;
            set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
        }
        
        public virtual bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }
        
        public virtual bool HasToolTip
        {
            get => _hasToolTip;
            set => this.RaiseAndSetIfChanged(ref _hasToolTip, value);
        }
    }
}