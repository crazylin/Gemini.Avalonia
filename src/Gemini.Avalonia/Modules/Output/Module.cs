using Microsoft.Extensions.DependencyInjection;
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Framework.Services;
using Gemini.Avalonia.Modules.Output.ViewModels;

namespace Gemini.Avalonia.Modules.Output
{
    [Module]
    public class Module : ModuleBase
    {
        public override void Initialize()
        {
            var shell = IoC.Get<IShell>();
            var outputTool = IoC.Get<OutputToolViewModel>();
            shell?.RegisterTool(outputTool!);
        }
    }
}