using System;
using System.Windows.Input;
using AuroraUI.Framework.Logging;

namespace AuroraUI.Framework.Commands
{
    public class TargetableCommand : ICommand
    {
        private readonly Command _command;
        private readonly ICommandRouter _commandRouter;

        public TargetableCommand(Command command)
        {
            _command = command;
            _commandRouter = IoC.Get<ICommandRouter>();
        }

        public bool CanExecute(object parameter)
        {
            var commandHandler = _commandRouter.GetCommandHandler(_command.CommandDefinition);
            if (commandHandler == null)
                return false;

            commandHandler.Update(_command);

            return _command.Enabled;
        }

        public async void Execute(object parameter)
        {
            LogManager.Debug("TargetableCommand", $"Execute被调用: {_command.CommandDefinition.GetType().Name}");
            var commandHandler = _commandRouter.GetCommandHandler(_command.CommandDefinition);
            if (commandHandler == null)
            {
                LogManager.Warning("TargetableCommand", $"未找到命令处理器: {_command.CommandDefinition.GetType().Name}");
                return;
            }

            LogManager.Info("TargetableCommand", $"执行命令: {_command.CommandDefinition.GetType().Name}");
            await commandHandler.Run(_command);
            LogManager.Info("TargetableCommand", $"命令执行完成: {_command.CommandDefinition.GetType().Name}");
        }

#pragma warning disable CS0067 // 事件从不使用
        public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067

        //public event EventHandler CanExecuteChanged
        //{
        //    add { CommandManager.RequerySuggested += value; }
        //    remove { CommandManager.RequerySuggested -= value; }
        //}
    }
}
