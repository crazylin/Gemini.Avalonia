using System;
using System.Windows.Input;
using Avalonia.Input;
using Gemini.Avalonia.Framework.Commands;


namespace Gemini.Avalonia.Framework.ToolBars
{
    public abstract class ToolBarItemDefinition
    {
        private readonly ToolBarItemGroupDefinition _group;
        private readonly int _sortOrder;
        private readonly ToolBarItemType _toolBarItemType;
        private readonly ToolBarItemDisplay _toolBarItemDisplay;
        public ToolBarItemGroupDefinition Group
        {
            get { return _group; }
        }

        public int SortOrder
        {
            get { return _sortOrder; }
        }

        public ToolBarItemType ToolBarItemType
        {
            get { return _toolBarItemType; }
        }
        public ToolBarItemDisplay ToolBarItemDisplay
        {
            get { return _toolBarItemDisplay; }
        }
        public abstract string Text { get; }
        public abstract Uri IconSource { get; }
        public abstract KeyGesture KeyGesture { get; }
        public abstract CommandDefinitionBase CommandDefinition { get; }

        protected ToolBarItemDefinition(ToolBarItemGroupDefinition group, int sortOrder, ToolBarItemType toolBarItemType, ToolBarItemDisplay toolBarItemDisplay)
        {
            _group = group;
            _sortOrder = sortOrder;
            _toolBarItemType = toolBarItemType;
            _toolBarItemDisplay = toolBarItemDisplay;
        }
    }

    public enum ToolBarItemType
    {
        Button,
        ToggleButton
    }
    public enum ToolBarItemDisplay
    {
        IconOnly,
        IconAndText
    }
}