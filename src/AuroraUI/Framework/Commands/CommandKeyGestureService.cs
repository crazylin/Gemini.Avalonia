using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Avalonia.Input;
using Avalonia.ReactiveUI;

namespace AuroraUI.Framework.Commands
{
    [Export(typeof(ICommandKeyGestureService))]
    public class CommandKeyGestureService : ICommandKeyGestureService
    {
        private readonly CommandKeyboardShortcut[] _keyboardShortcuts;
        private readonly ICommandService _commandService;

        [ImportingConstructor]
        public CommandKeyGestureService(
            [ImportMany] CommandKeyboardShortcut[] keyboardShortcuts,
            [ImportMany] ExcludeCommandKeyboardShortcut[] excludeKeyboardShortcuts,
            ICommandService commandService)
        {
            _keyboardShortcuts = keyboardShortcuts
                .Except(excludeKeyboardShortcuts.Select(x => x.KeyboardShortcut))
                .OrderBy(x => x.SortOrder)
                .ToArray();
            _commandService = commandService;
        }

        public void BindKeyGestures(InputElement uiElement)
        {
            foreach (var keyboardShortcut in _keyboardShortcuts)
                if (keyboardShortcut.KeyGesture != null)
                {
                    var keyBinding = new KeyBinding();
                    keyBinding.Command =
                        _commandService.GetTargetableCommand(
                            _commandService.GetCommand(keyboardShortcut.CommandDefinition));
                    keyBinding.Gesture = keyboardShortcut.KeyGesture;

                    uiElement.KeyBindings.Add(keyBinding);
                }
        }

        public KeyGesture GetPrimaryKeyGesture(CommandDefinitionBase commandDefinition)
        {
            var keyboardShortcut = _keyboardShortcuts.FirstOrDefault(x => x.CommandDefinition == commandDefinition);
            return (keyboardShortcut != null)
                ? keyboardShortcut.KeyGesture
                : null;
        }
    }
}