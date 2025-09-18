using System;
using ReactiveUI;
using AuroraUI.Framework.Services;

namespace AuroraUI.Framework.Commands
{
    public class Command : ReactiveObject
    {
        private readonly CommandDefinitionBase _commandDefinition;
        private bool _visible = true;
        private bool _enabled = true;
        private bool _checked;
        private string _text;
        private string _toolTip;
        private Uri _iconSource;
        private string _iconName;

        public CommandDefinitionBase CommandDefinition
        {
            get { return _commandDefinition; }       
        }

        public bool Visible
        {
            get { return _visible; }
            set { this.RaiseAndSetIfChanged(ref _visible, value); }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { this.RaiseAndSetIfChanged(ref _enabled, value); }
        }

        public bool Checked
        {
            get { return _checked; }
            set { this.RaiseAndSetIfChanged(ref _checked, value); }
        }

        public string Text
        {
            get { return _text; }
            set { this.RaiseAndSetIfChanged(ref _text, value); }
        }

        public string ToolTip
        {
            get { return _toolTip; }
            set { this.RaiseAndSetIfChanged(ref _toolTip, value); }
        }

        public Uri IconSource
        {
            get { return _iconSource; }
            set { this.RaiseAndSetIfChanged(ref _iconSource, value); }
        }

        public string IconName
        {
            set { this.RaiseAndSetIfChanged(ref _iconName, value); }
            get => _iconName;
        }

        public object Tag { get; set; }

        public Command(CommandDefinitionBase commandDefinition)
        {
            _commandDefinition = commandDefinition;
            Text = commandDefinition.Text;
            ToolTip = commandDefinition.ToolTip;
            IconSource = commandDefinition.IconSource;
            IconName = commandDefinition.IconName;

            try
            {
                // 初始化时设置命令属性
                Text = _commandDefinition.Text;
                ToolTip = _commandDefinition.ToolTip;
                IconSource = _commandDefinition.IconSource;
                IconName = _commandDefinition.IconName;
            }
            catch
            {
                // 忽略本地化服务不可用的情况
            }
        }
    }
}