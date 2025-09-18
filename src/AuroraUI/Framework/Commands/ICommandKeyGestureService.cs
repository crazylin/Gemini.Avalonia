using System.Windows;
using System.Windows.Input;
using Avalonia.Input;

namespace AuroraUI.Framework.Commands
{
    public interface ICommandKeyGestureService
    {
        void BindKeyGestures(InputElement uiElement);
        KeyGesture GetPrimaryKeyGesture(CommandDefinitionBase commandDefinition);
    }
}