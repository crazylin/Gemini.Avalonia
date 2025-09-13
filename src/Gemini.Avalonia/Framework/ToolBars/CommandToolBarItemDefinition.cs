using System;
using System.Windows.Input;
using Avalonia.Input;
using Gemini.Avalonia.Framework.Commands;

namespace Gemini.Avalonia.Framework.ToolBars
{
    public class CommandToolBarItemDefinition<TCommandDefinition> : ToolBarItemDefinition
        where TCommandDefinition : CommandDefinitionBase
    {
        private CommandDefinitionBase? _commandDefinition;
        private KeyGesture? _keyGesture;

        public override string Text => _commandDefinition?.Text ?? "Unknown Command";

        public override Uri IconSource => _commandDefinition?.IconSource;

        public override KeyGesture KeyGesture => _keyGesture;

        public override CommandDefinitionBase CommandDefinition 
        {
            get
            {
                if (_commandDefinition == null)
                {
                    InitializeCommandDefinition();
                }
                return _commandDefinition;
            }
        }

        public CommandToolBarItemDefinition(ToolBarItemGroupDefinition group, int sortOrder, ToolBarItemType toolBarItemType = ToolBarItemType.Button,ToolBarItemDisplay toolBarItemDisplay= ToolBarItemDisplay.IconAndText)
            : base(group, sortOrder, toolBarItemType, toolBarItemDisplay)
        {
            // 延迟初始化，在第一次访问时才获取CommandDefinition
        }
        
        private void InitializeCommandDefinition()
        {
            var commandService = IoC.Get<ICommandService>();
            if (commandService != null)
            {
                _commandDefinition = commandService.GetCommandDefinition(typeof(TCommandDefinition));
                _keyGesture = IoC.Get<ICommandKeyGestureService>()?.GetPrimaryKeyGesture(_commandDefinition);
            }
        }
    }
}