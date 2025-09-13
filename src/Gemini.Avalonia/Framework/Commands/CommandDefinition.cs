
using System;

namespace Gemini.Avalonia.Framework.Commands
{
    public abstract class CommandDefinition : CommandDefinitionBase
    {
        public override Uri IconSource
        {
            get { return null; }
        }
        public override string IconName
        {
            get { return null; }
        }

        public sealed override bool IsList
        {
            get { return false; }
        }
    }
}