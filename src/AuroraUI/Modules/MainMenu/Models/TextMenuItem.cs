using System;
using System.Windows.Input;
using Avalonia.Input;
using AuroraUI.Framework.Menus;
using AuroraUI.Framework.Commands;
using AuroraUI.Framework;
using AuroraUI.Framework.Services;
using AuroraUI.Framework.Logging;
using ReactiveUI;


namespace AuroraUI.Modules.MainMenu.Models
{
    public class TextMenuItem : StandardMenuItem
    {
        private readonly MenuDefinitionBase _menuDefinition;
        private ICommand _command;
        private ILocalizationService _localizationService;

        public override string Header
        {
            get
        {

            if (_localizationService == null)
            {
                try
                {
                    _localizationService = IoC.Get<ILocalizationService>();
        
                }
                catch (Exception ex)
                {
                    LogManager.Error("TextMenuItem 获取本地化服务失败: {0}", ex.Message);
                    return _menuDefinition.Header;
                }
            }
            
            // 如果Header看起来像资源键（包含点），则尝试本地化
            if (_menuDefinition.Header.Contains("."))
            {
  
                var result = _localizationService.GetString(_menuDefinition.Header, _menuDefinition.Header);
     
                return result;
            }
 
            return _menuDefinition.Header;
        }
        }

        public override Uri IconSource => _menuDefinition.IconSource;

        public override KeyGesture KeyGesture => _menuDefinition.KeyGesture;
        
        public override ICommand Command 
        {
            get
            {
                if (_command == null && _menuDefinition.CommandDefinition != null)
                {
                    try
                    {
                        var commandService = IoC.Get<ICommandService>();
                        if (commandService != null)
                        {
                            var command = commandService.GetCommand(_menuDefinition.CommandDefinition);
                            // 获取TargetableCommand，它实现了ICommand接口
                            _command = commandService.GetTargetableCommand(command);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 静默处理异常
                    }
                }

                return _command;
            }
        }
        
        public override bool IsChecked => false;
        public override bool IsVisible => true;

        public TextMenuItem(MenuDefinitionBase menuDefinition)
        {
            _menuDefinition = menuDefinition;
            
            // 尝试获取本地化服务
            try
            {
                _localizationService = IoC.Get<ILocalizationService>();
            }
            catch
            {
                // 如果获取失败，使用原始Header
            }
        }
    }
}
