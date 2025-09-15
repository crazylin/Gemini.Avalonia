using System;
using System.Windows.Input;
using Avalonia.Input;
using Avalonia.Threading;
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Framework.Commands;
using ReactiveUI;


namespace Gemini.Avalonia.Modules.MainMenu.Models
{
    public class CommandMenuItem : StandardMenuItem
    {
        public override string Header => _command.Text;

        public override Uri IconSource
        {
            get => _command?.IconSource;
            set { /* CommandMenuItem的IconSource由_command.IconSource控制，忽略setter */ }
        }


        public override KeyGesture KeyGesture => IoC.Get<ICommandKeyGestureService>()
            .GetPrimaryKeyGesture(_command.CommandDefinition);

        public override ICommand Command
        {
            get
            {
                try
                {
                    var commandService = IoC.Get<ICommandService>();
                    if (commandService == null)
                    {
                        return null;
                    }
                    
                    var targetableCommand = commandService.GetTargetableCommand(_command);
                    
                    return targetableCommand;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public override bool IsChecked => _command.Checked;

        public override bool IsVisible => _command.Visible;

        private Command _command;
        private readonly StandardMenuItem _parent;
        //private readonly List<StandardMenuItem> _listItems;

        public CommandMenuItem(Command command, StandardMenuItem parent)
        {
            _command = command;
            _parent = parent;
            
            // 延迟初始化Command属性，避免MEF循环依赖
            // 使用Dispatcher.UIThread.Post确保在UI线程上初始化
            Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    var commandService = IoC.Get<ICommandService>();
                    if (commandService != null)
                    {
                        var targetableCommand = commandService.GetTargetableCommand(_command);
                        if (targetableCommand != null)
                        {
                            // 设置基类MenuItemBase的Command属性
                            ((MenuItemBase)this).Command = targetableCommand;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 忽略异常
                }
            });
            
            //_listItems = new List<StandardMenuItem>();
            _command.PropertyChanged += _command_PropertyChanged;
        }

        private void _command_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_command.Visible):
                    this.RaisePropertyChanged(nameof(IsVisible));
                    break;

                case nameof(_command.Checked):
                    this.RaisePropertyChanged(nameof(IsChecked));
                    break;

                case nameof(_command.Text):
                    base.Header = _command.Text;
                    this.RaisePropertyChanged(nameof(Header));
                    break;
                    
                case nameof(_command.IconSource):
                    this.RaisePropertyChanged(e.PropertyName);
                    break;
            }
        }

    }
}
