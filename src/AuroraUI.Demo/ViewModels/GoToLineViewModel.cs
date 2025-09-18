using ReactiveUI;

namespace AuroraUI.Demo.ViewModels
{
    /// <summary>
    /// 跳转到行对话框的视图模型
    /// </summary>
    public class GoToLineViewModel : ReactiveObject
    {
        private string _lineNumber = "1";
        private string _infoMessage = string.Empty;

        /// <summary>
        /// 行号
        /// </summary>
        public string LineNumber
        {
            get => _lineNumber;
            set => this.RaiseAndSetIfChanged(ref _lineNumber, value);
        }

        /// <summary>
        /// 信息消息
        /// </summary>
        public string InfoMessage
        {
            get => _infoMessage;
            set => this.RaiseAndSetIfChanged(ref _infoMessage, value);
        }
    }
}
