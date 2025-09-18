using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Avalonia.Input;
using AuroraUI.Framework;
using AuroraUI.Framework.Commands;
using AuroraUI.Framework.ToolBars;
using AuroraUI.Modules.ToolBars;
using ReactiveUI;


namespace AuroraUI.Modules.ToolBars.Models
{
	public class ButtonToolBarItem : ToolBarItemBase, ICommandUiItem
    {
	    private readonly ToolBarItemDefinition _toolBarItem;
	    private readonly Command _command;
        private readonly KeyGesture _keyGesture;
        private readonly IToolBar _parent;

		public override string Text => TrimMnemonics(_command.Text);

        public override Uri IconSource => _command.IconSource;

        public string IconName => _command.IconName;

        public bool ImageOnly 
        {
            get 
            {
                var result = _toolBarItem.ToolBarItemDisplay == ToolBarItemDisplay.IconOnly;
                // 计算ImageOnly属性
                return result;
            }
        }
        public override string ToolTip
	    {
	        get
	        {
                var inputGestureText = (_keyGesture != null)
                    ? $" ({_keyGesture.ToString()})"
                    : string.Empty;

                return $"{_command.ToolTip}{inputGestureText}".Trim();
	        }
	    }
        

	    public override bool HasToolTip => !string.IsNullOrWhiteSpace(ToolTip);

        public override ICommand Command => IoC.Get<ICommandService>().GetTargetableCommand(_command);

        public override bool IsChecked => _command.Checked;

        public override bool IsVisible => _command.Visible;


        public ButtonToolBarItem(ToolBarItemDefinition toolBarItem, Command command, IToolBar parent)
		{
		    _toolBarItem = toolBarItem;
		    _command = command;
            _keyGesture = IoC.Get<ICommandKeyGestureService>()?.GetPrimaryKeyGesture(_command.CommandDefinition);
            _parent = parent;

            command.PropertyChanged += OnCommandPropertyChanged;
		}

        private void OnCommandPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Framework.Commands.Command.Text):
                    this.RaisePropertyChanged(nameof(Text));
                    break;
                case nameof(Framework.Commands.Command.IconSource):
                    this.RaisePropertyChanged(nameof(IconSource));
                    break;
                case nameof(Framework.Commands.Command.IconName):
                    this.RaisePropertyChanged(nameof(IconName));
                    break;
                case nameof(Framework.Commands.Command.ToolTip):
                    this.RaisePropertyChanged(nameof(ToolTip));
                    this.RaisePropertyChanged(nameof(HasToolTip));
                    break;
                case nameof(Framework.Commands.Command.Checked):
                    this.RaisePropertyChanged(nameof(IsChecked));
                    break;
                case nameof(Framework.Commands.Command.Visible):
                    this.RaisePropertyChanged(nameof(IsVisible));
                    break;
         
            }
        }

	    CommandDefinitionBase ICommandUiItem.CommandDefinition => _command.CommandDefinition;

        void ICommandUiItem.Update(CommandHandlerWrapper commandHandler)
	    {
	        // TODO
	    }

        /// <summary>
        /// Remove mnemonics underscore used by menu from text.
        /// Also replace escaped/double underscores by a single underscore.
        /// Displayed text will be the same than with a menu item.
        /// </summary>
        private static string TrimMnemonics(string text)
        {
            var resultArray = new char[text.Length];

            int resultLength = 0;
            bool previousWasUnderscore = false;
            bool mnemonicsFound = false;

            for (int textIndex = 0; textIndex < text.Length; textIndex++)
            {
                char c = text[textIndex];

                if (c == '_')
                {
                    if (!previousWasUnderscore)
                    {
                        // If previous character was not an underscore but the current is one, we set the flag.
                        previousWasUnderscore = true;

                        // Also, if mnemonics mark was not found yet, we also skip that underscore in result.
                        if (!mnemonicsFound)
                            continue;
                    }
                    else
                    {
                        // If both current and previous character are underscores, it is an escaped underscore.
                        // We will include that second underscore in result and restore the flag.
                        previousWasUnderscore = false;

                        // If mnemonics mark was already found, previous underscore was included in result so we can escape this one.
                        if (mnemonicsFound)
                            continue;
                    }
                }
                else
                {
                    // If previous character was an underscore and the current is not one, we found the mnemonics mark.
                    // We will stop to search and include all the following characters, except escaped underscores, in result.
                    if (!mnemonicsFound && previousWasUnderscore)
                        mnemonicsFound = true;

                    previousWasUnderscore = false;
                }

                resultArray[resultLength++] = c;
            }

            // If last character was an underscore and mnemonics mark was not found, it should be included in result.
            if (previousWasUnderscore && !mnemonicsFound)
                resultArray[resultLength++] = '_';

            return new string(resultArray, 0, resultLength);
        }
    }
}