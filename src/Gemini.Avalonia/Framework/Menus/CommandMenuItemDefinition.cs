using System;
using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Input;
using ReactiveUI;
using Gemini.Avalonia.Framework.Commands;
using Gemini.Avalonia.Framework.Logging;


namespace Gemini.Avalonia.Framework.Menus
{
    public class CommandMenuItemDefinition<TCommandDefinition> : MenuItemDefinition
        where TCommandDefinition : CommandDefinitionBase
    {
        private CommandDefinitionBase? _commandDefinition;
        private KeyGesture? _keyGesture;
        private bool _initialized = false;

        public override string Header
        {
            get 
            { 
                EnsureInitialized();
                return _commandDefinition?.Text ?? "[未知命令]"; 
            }
        }

        public override Uri IconSource
        {
            get 
            { 
                EnsureInitialized();
                return _commandDefinition?.IconSource; 
            }
        }

        public override KeyGesture KeyGesture
        {
            get 
            { 
                EnsureInitialized();
                return _keyGesture; 
            }
        }

        public override CommandDefinitionBase CommandDefinition
        {
            get 
            { 
                EnsureInitialized();
                return _commandDefinition; 
            }
        }

        public CommandMenuItemDefinition(MenuItemGroupDefinition group, int sortOrder)
            : base(group, sortOrder)
        {
            // 延迟初始化，在第一次访问时才获取CommandDefinition
        }

        private void EnsureInitialized()
        {
            if (!_initialized)
            {
                try
                {
                    var commandService = IoC.Get<ICommandService>();
                    if (commandService != null)
                    {
                        _commandDefinition = commandService.GetCommandDefinition(typeof(TCommandDefinition));
                        if (_commandDefinition != null)
                        {
                            _keyGesture = IoC.Get<ICommandKeyGestureService>()?.GetPrimaryKeyGesture(_commandDefinition);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 初始化CommandDefinition失败，忽略错误
                }
                _initialized = true;
            }
        }
        
      
    }
}