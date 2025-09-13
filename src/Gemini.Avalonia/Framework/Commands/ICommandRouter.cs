namespace Gemini.Avalonia.Framework.Commands
{
    public interface ICommandRouter
    {
        CommandHandlerWrapper GetCommandHandler(CommandDefinitionBase commandDefinition);
    }
}