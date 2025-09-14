using System;
using System.ComponentModel.Composition;
using Gemini.Avalonia.Framework.Commands;

namespace Gemini.Avalonia.Modules.Settings.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class OpenSettingsCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Tools.Options";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text => LocalizationService?.GetString("Tools.Options");

        public override string ToolTip => LocalizationService?.GetString("Tools.Options.ToolTip");
        
        public override Uri IconSource => new Uri("avares://Gemini.Avalonia/Assets/Icons/settings.svg");
    }
}