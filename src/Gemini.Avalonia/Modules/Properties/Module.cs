using Microsoft.Extensions.DependencyInjection;
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Framework.Services;
using Gemini.Avalonia.Modules.Properties.ViewModels;

namespace Gemini.Avalonia.Modules.Properties
{
    [Module]
    public class Module : ModuleBase
    {
        public override void Initialize()
        {
            var shell = IoC.Get<IShell>();
            var propertiesTool = IoC.Get<PropertiesToolViewModel>();
            shell?.RegisterTool(propertiesTool!);
        }
    }
}