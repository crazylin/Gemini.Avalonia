using ReactiveUI;

namespace AuroraUI.Demo.ViewModels
{
    /// <summary>
    /// 查找和替换对话框的视图模型
    /// </summary>
    public class FindReplaceViewModel : ReactiveObject
    {
        private string _findText = string.Empty;
        private string _replaceText = string.Empty;
        private bool _matchCase = false;
        private bool _matchWholeWord = false;
        private string _statusMessage = "准备就绪";

        /// <summary>
        /// 查找文本
        /// </summary>
        public string FindText
        {
            get => _findText;
            set => this.RaiseAndSetIfChanged(ref _findText, value);
        }

        /// <summary>
        /// 替换文本
        /// </summary>
        public string ReplaceText
        {
            get => _replaceText;
            set => this.RaiseAndSetIfChanged(ref _replaceText, value);
        }

        /// <summary>
        /// 区分大小写
        /// </summary>
        public bool MatchCase
        {
            get => _matchCase;
            set => this.RaiseAndSetIfChanged(ref _matchCase, value);
        }

        /// <summary>
        /// 全字匹配
        /// </summary>
        public bool MatchWholeWord
        {
            get => _matchWholeWord;
            set => this.RaiseAndSetIfChanged(ref _matchWholeWord, value);
        }

        /// <summary>
        /// 状态消息
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
        }
    }
}
