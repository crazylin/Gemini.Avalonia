using System;
using System.ComponentModel.Composition;

namespace Gemini.Avalonia.Framework.Commands
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandDefinitionAttribute : ExportAttribute
    {
        public CommandDefinitionAttribute()
            : base(typeof(CommandDefinitionBase))
        {
            
        }
    }
}