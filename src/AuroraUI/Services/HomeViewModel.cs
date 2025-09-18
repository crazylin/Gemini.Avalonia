using Dock.Model.Mvvm.Controls;

namespace AuroraUI.Services
{
    /// <summary>
    /// 主视图模型，用于包装主布局
    /// </summary>
    public class HomeViewModel : RootDock
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public HomeViewModel()
        {
            CanClose = false;
            CanFloat = false;
            CanPin = false;
        }
    }
}