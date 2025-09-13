using System.Windows;
using System.Windows.Input;
using Avalonia.Input;

namespace Gemini.Avalonia.Framework.Commands
{
    public interface ICommandKeyGestureService
    {
        void BindKeyGestures(InputElement uiElement);
        KeyGesture GetPrimaryKeyGesture(CommandDefinitionBase commandDefinition);
    }
}