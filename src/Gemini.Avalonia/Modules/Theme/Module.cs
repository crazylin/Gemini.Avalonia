using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Modules.Theme.Services;
using Gemini.Avalonia.Framework.Logging;

namespace Gemini.Avalonia.Modules.Theme
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        
        [Import]
        private IThemeService? _themeService;
        
        [Import]
        private IThemeResourceManager? _themeResourceManager;
        
        public override void Initialize()
        {
            Logger.Info("主题模块初始化开始");
            // 主题服务将在Shell中初始化，这里不需要重复初始化
            Logger.Info("主题模块初始化完成");
        }
        
        public override Task PostInitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}