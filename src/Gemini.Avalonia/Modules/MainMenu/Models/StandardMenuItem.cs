using System;
using System.Windows.Input;
using Avalonia.Input;

namespace Gemini.Avalonia.Modules.MainMenu.Models
{
    public abstract class StandardMenuItem : MenuItemBase
    {
        public override string Header { get; set; }
        public virtual Uri IconSource { get; }
        public virtual ICommand Command { get; set; }
        public virtual bool IsChecked { get; }
        public virtual bool IsVisible { get; }
        public virtual KeyGesture KeyGesture { set; get; }

    }
}
