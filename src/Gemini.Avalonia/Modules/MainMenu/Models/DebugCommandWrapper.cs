using System;
using System.Windows.Input;
using Gemini.Avalonia.Framework.Logging;

namespace Gemini.Avalonia.Modules.MainMenu.Models
{
    public class DebugCommandWrapper : ICommand
    {
        private readonly ICommand _innerCommand;
        private readonly string _menuName;

        public DebugCommandWrapper(ICommand innerCommand, string menuName)
        {
            _innerCommand = innerCommand;
            _menuName = menuName;
            LogManager.Debug("DebugCommandWrapper", $"创建调试包装器: {_menuName}");
        }

        public bool CanExecute(object parameter)
        {
            var canExecute = _innerCommand?.CanExecute(parameter) ?? false;
            LogManager.Debug("DebugCommandWrapper", $"CanExecute - 菜单: {_menuName}, 结果: {canExecute}");
            return canExecute;
        }

        public void Execute(object parameter)
        {
            LogManager.Info("DebugCommandWrapper", $"Execute - 菜单: {_menuName}");
            try
            {
                _innerCommand?.Execute(parameter);
                LogManager.Debug("DebugCommandWrapper", "Command executed successfully");
            }
            catch (Exception ex)
            {
                LogManager.Error("DebugCommandWrapper", $"Command execution failed: {ex.Message}");
                throw;
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add { _innerCommand.CanExecuteChanged += value; }
            remove { _innerCommand.CanExecuteChanged -= value; }
        }
    }
}