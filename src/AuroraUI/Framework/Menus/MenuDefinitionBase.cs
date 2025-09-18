using System;
using System.Windows.Input;
using Avalonia.Input;
using AuroraUI.Framework.Commands;

namespace AuroraUI.Framework.Menus
{
    public abstract class MenuDefinitionBase
    {
        public abstract int SortOrder { get; }
        public abstract string Header { get; }
        public abstract Uri IconSource { get; }
        public abstract KeyGesture KeyGesture { get; }
        public abstract CommandDefinitionBase CommandDefinition { get; }
    }
}