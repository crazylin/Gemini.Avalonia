using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Gemini.Avalonia.Framework.Commands
{
    [Export(typeof(ICommandRouter))]
    public class CommandRouter : ICommandRouter
    {
        private static readonly Type CommandHandlerInterfaceType = typeof(ICommandHandler<>);
        private static readonly Type CommandListHandlerInterfaceType = typeof(ICommandListHandler<>);

        private readonly Dictionary<Type, CommandHandlerWrapper> _globalCommandHandlerWrappers;
        private readonly Dictionary<Type, HashSet<Type>> _commandHandlerTypeToCommandDefinitionTypesLookup;
            
        [ImportingConstructor]
        public CommandRouter(
            [ImportMany(typeof(ICommandHandler))] ICommandHandler[] globalCommandHandlers)
        {
            _commandHandlerTypeToCommandDefinitionTypesLookup = new Dictionary<Type, HashSet<Type>>();
            _globalCommandHandlerWrappers = BuildCommandHandlerWrappers(globalCommandHandlers);
        }

        private Dictionary<Type, CommandHandlerWrapper> BuildCommandHandlerWrappers(ICommandHandler[] commandHandlers)
        {
            var commandHandlersList = SortCommandHandlers(commandHandlers);

            // Command handlers are either ICommandHandler<T> or ICommandListHandler<T>.
            // We need to extract T, and use it as the key in our dictionary.

            var result = new Dictionary<Type, CommandHandlerWrapper>();

            foreach (var commandHandler in commandHandlersList)
            {
                var commandHandlerType = commandHandler.GetType();
                EnsureCommandHandlerTypeToCommandDefinitionTypesPopulated(commandHandlerType);
                var commandDefinitionTypes = _commandHandlerTypeToCommandDefinitionTypesLookup[commandHandlerType];
                foreach (var commandDefinitionType in commandDefinitionTypes)
                    result[commandDefinitionType] = CreateCommandHandlerWrapper(commandDefinitionType, commandHandler);
            }

            return result;
        }

        private static List<ICommandHandler> SortCommandHandlers(ICommandHandler[] commandHandlers)
        {
            // Put command handlers defined in priority assemblies, last. This allows applications
            // to override built-in command handlers.

            return commandHandlers
                .OrderBy(h => AppBootstrapper.PriorityAssemblies.Contains(h.GetType().Assembly) ? 1 : 0)
                .ToList();
        }

        public CommandHandlerWrapper GetCommandHandler(CommandDefinitionBase commandDefinition)
        {
            CommandHandlerWrapper commandHandler;

            // If none of the objects in the DataContext hierarchy handle the command,
            // fallback to the global handler.
            if (!_globalCommandHandlerWrappers.TryGetValue(commandDefinition.GetType(), out commandHandler))
                return null;

            return commandHandler;
        }

      

        private static CommandHandlerWrapper CreateCommandHandlerWrapper(
            Type commandDefinitionType, object commandHandler)
        {
            if (typeof(CommandDefinition).IsAssignableFrom(commandDefinitionType))
                return CommandHandlerWrapper.FromCommandHandler(CommandHandlerInterfaceType.MakeGenericType(commandDefinitionType), commandHandler);
            if (typeof(CommandListDefinition).IsAssignableFrom(commandDefinitionType))
                return CommandHandlerWrapper.FromCommandListHandler(CommandListHandlerInterfaceType.MakeGenericType(commandDefinitionType), commandHandler);
            throw new InvalidOperationException();
        }

        private bool IsCommandHandlerForCommandDefinitionType(
            object commandHandler, Type commandDefinitionType)
        {
            var commandHandlerType = commandHandler.GetType();
            EnsureCommandHandlerTypeToCommandDefinitionTypesPopulated(commandHandlerType);
            var commandDefinitionTypes = _commandHandlerTypeToCommandDefinitionTypesLookup[commandHandlerType];
            return commandDefinitionTypes.Contains(commandDefinitionType);
        }

        private void EnsureCommandHandlerTypeToCommandDefinitionTypesPopulated(Type commandHandlerType)
        {
            if (!_commandHandlerTypeToCommandDefinitionTypesLookup.ContainsKey(commandHandlerType))
            {
                var commandDefinitionTypes = _commandHandlerTypeToCommandDefinitionTypesLookup[commandHandlerType] = new HashSet<Type>();

                foreach (var handledCommandDefinitionType in GetAllHandledCommandedDefinitionTypes(commandHandlerType, CommandHandlerInterfaceType))
                    commandDefinitionTypes.Add(handledCommandDefinitionType);

                foreach (var handledCommandDefinitionType in GetAllHandledCommandedDefinitionTypes(commandHandlerType, CommandListHandlerInterfaceType))
                    commandDefinitionTypes.Add(handledCommandDefinitionType);
            }
        }

        private static IEnumerable<Type> GetAllHandledCommandedDefinitionTypes(
            Type type, Type genericInterfaceType)
        {
            var result = new List<Type>();

            while (type != null)
            {
                result.AddRange(type.GetInterfaces()
                    .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericInterfaceType)
                    .Select(x => x.GetGenericArguments().First()));

                type = type.BaseType;
            }

            return result;
        }
    }
}