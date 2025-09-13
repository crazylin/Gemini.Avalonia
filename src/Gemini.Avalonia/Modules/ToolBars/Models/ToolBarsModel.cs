using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using ReactiveUI;

namespace Gemini.Avalonia.Modules.ToolBars.Models
{
    [Export(typeof(IToolBars))]
    public class ToolBarsModel : ReactiveObject, IToolBars
    {
        private readonly IToolBarBuilder _toolBarBuilder;
        private bool _visible = true;
        
        public ObservableCollection<IToolBar> Items { get; } = new ObservableCollection<IToolBar>();
        
        public bool Visible
        {
            get => _visible;
            set => this.RaiseAndSetIfChanged(ref _visible, value);
        }
        
        [ImportingConstructor]
        public ToolBarsModel(IToolBarBuilder toolBarBuilder)
        {
            _toolBarBuilder = toolBarBuilder;
            // 构造函数完成
        }
        
        /// <summary>
        /// 初始化工具栏
        /// </summary>
        public void InitializeToolBars()
        {
            _toolBarBuilder.BuildToolBars(this);
        }
    }
}
