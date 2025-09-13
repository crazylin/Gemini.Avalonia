using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Framework.Commands;
using Gemini.Avalonia.Modules.Settings.ViewModels;

namespace Gemini.Avalonia.Modules.Settings.Commands
{
    [CommandHandler]
    public class OpenSettingsCommandHandler : CommandHandlerBase<OpenSettingsCommandDefinition>
    {
        public OpenSettingsCommandHandler()
        {
        }

        public override async Task Run(Command command)
        {
            // 开始执行设置命令
            
            var settingsViewModel = IoC.Get<SettingsViewModel>();
            if (settingsViewModel != null)
            {
                await settingsViewModel.ShowDialog();
                // 设置对话框已显示
            }
            else
            {
                // 无法获取SettingsViewModel
            }
            
            // 设置命令执行完成
        }
    }
}