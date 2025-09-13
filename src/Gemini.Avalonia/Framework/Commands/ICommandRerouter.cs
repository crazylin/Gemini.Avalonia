namespace Gemini.Avalonia.Framework.Commands
{
    public interface ICommandRerouter
    {
        object GetHandler(CommandDefinitionBase commandDefinition);
    }
}