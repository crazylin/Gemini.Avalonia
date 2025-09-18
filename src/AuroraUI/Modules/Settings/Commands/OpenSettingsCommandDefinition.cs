using System;
using System.ComponentModel.Composition;
using AuroraUI.Framework.Commands;

namespace AuroraUI.Modules.Settings.Commands
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
        
        public override Uri IconSource => new Uri("avares://AuroraUI/Assets/Icons/settings.svg");
    }
}