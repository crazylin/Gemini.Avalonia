using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Gemini.Avalonia.Framework.Logging;

namespace Gemini.Avalonia.Framework.Commands
{
    [Export(typeof(ICommandService))]
    public class CommandService : ICommandService
    {
        private readonly Dictionary<Type, CommandDefinitionBase> _commandDefinitionsLookup;
        private readonly Dictionary<CommandDefinitionBase, Command> _commands;
        private readonly Dictionary<Command, TargetableCommand> _targetableCommands;

        private CommandDefinitionBase[] _commandDefinitions;

        [ImportingConstructor]
        public CommandService([ImportMany] CommandDefinitionBase[] commandDefinitions)
        {
            _commandDefinitions = commandDefinitions ?? Array.Empty<CommandDefinitionBase>();
            _commandDefinitionsLookup = new Dictionary<Type, CommandDefinitionBase>();
            _commands = new Dictionary<CommandDefinitionBase, Command>();
            _targetableCommands = new Dictionary<Command, TargetableCommand>();
            
       
            // 导入命令定义完成
             foreach (var cmd in _commandDefinitions)
             {
                 _commandDefinitionsLookup[cmd.GetType()] = cmd;
             }
        }

        public CommandDefinitionBase GetCommandDefinition(Type commandDefinitionType)
        {

            CommandDefinitionBase commandDefinition;
            if (!_commandDefinitionsLookup.TryGetValue(commandDefinitionType, out commandDefinition))
            {
                // 查找匹配的CommandDefinition
                commandDefinition = _commandDefinitions.FirstOrDefault(x => x.GetType() == commandDefinitionType);
                if (commandDefinition != null)
                {
                    _commandDefinitionsLookup[commandDefinitionType] = commandDefinition;
                }

            }

            return commandDefinition;
        }

        public Command GetCommand(CommandDefinitionBase commandDefinition)
        {
            Command command;
            if (!_commands.TryGetValue(commandDefinition, out command))
                command = _commands[commandDefinition] = new Command(commandDefinition);
            return command;
        }

        public TargetableCommand GetTargetableCommand(Command command)
        {
            TargetableCommand targetableCommand;
            if (!_targetableCommands.TryGetValue(command, out targetableCommand))
                targetableCommand = _targetableCommands[command] = new TargetableCommand(command);
            return targetableCommand;
        }
    }
}