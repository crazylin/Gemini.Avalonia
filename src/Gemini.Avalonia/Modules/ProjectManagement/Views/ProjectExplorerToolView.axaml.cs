using Avalonia.Controls;
using Gemini.Avalonia.Modules.ProjectManagement.ViewModels;

namespace Gemini.Avalonia.Modules.ProjectManagement.Views
{
    public partial class ProjectExplorerToolView : UserControl
    {
        public ProjectExplorerToolView()
        {
            InitializeComponent();
        }
        
        public ProjectExplorerToolView(ProjectExplorerToolViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}