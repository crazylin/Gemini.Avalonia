using AuroraUI.Modules.ToolBars.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraUI.Modules.ToolBars.ViewModels
{
    [Export(typeof(ToolBarViewModel))]
    public class ToolBarViewModel : ReactiveObject
    {
        private readonly IToolBarBuilder _toolBarBuilder;
        private IToolBars _toolBars;

        public ToolBarViewModel(IToolBarBuilder menuBuilder)
        {
            _toolBarBuilder = menuBuilder;
            _toolBars = new ToolBarsModel(_toolBarBuilder);
            _toolBarBuilder.BuildToolBars(_toolBars);
            this.RaisePropertyChanged(nameof(ToolBars));
        }

        public IToolBars ToolBars
        {
            set => this.RaiseAndSetIfChanged(ref _toolBars, value);
            get => _toolBars;
        }
    }
}
