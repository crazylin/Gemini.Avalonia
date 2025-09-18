using Avalonia.Controls;
using AuroraUI.Modules.ProjectManagement.ViewModels;

namespace AuroraUI.Modules.ProjectManagement.Views
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