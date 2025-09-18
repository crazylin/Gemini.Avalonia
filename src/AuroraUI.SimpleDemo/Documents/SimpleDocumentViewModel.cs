using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AuroraUI.Framework;

namespace AuroraUI.SimpleDemo.Documents
{
    /// <summary>
    /// 简单文档视图模型 - 文本编辑器
    /// </summary>
    public class SimpleDocumentViewModel : Document, INotifyPropertyChanged
    {
        private string _content = string.Empty;
        private string? _filePath;
        private bool _isDirty;

        /// <summary>
        /// 文档内容
        /// </summary>
        public string Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    NotifyPropertyChanged();
                    SetDirty(true);
                }
            }
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string? FilePath
        {
            get => _filePath;
            private set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    NotifyPropertyChanged();
                    UpdateDisplayName();
                }
            }
        }

        /// <summary>
        /// 是否已修改
        /// </summary>
        public bool IsDirty
        {
            get => _isDirty;
            private set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    NotifyPropertyChanged();
                    UpdateDisplayName();
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SimpleDocumentViewModel()
        {
            DisplayName = "新建文档";
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public SimpleDocumentViewModel(string filePath)
        {
            FilePath = filePath;
            LoadFromFile(filePath);
        }

        /// <summary>
        /// 设置脏标记
        /// </summary>
        /// <param name="isDirty">是否脏</param>
        private void SetDirty(bool isDirty)
        {
            IsDirty = isDirty;
        }

        /// <summary>
        /// 更新显示名称
        /// </summary>
        private void UpdateDisplayName()
        {
            var name = string.IsNullOrEmpty(FilePath) ? "新建文档" : Path.GetFileName(FilePath);
            DisplayName = IsDirty ? $"{name}*" : name;
        }

        /// <summary>
        /// 从文件加载
        /// </summary>
        /// <param name="filePath">文件路径</param>
        private void LoadFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    _content = File.ReadAllText(filePath, Encoding.UTF8);
                    NotifyPropertyChanged(nameof(Content));
                    SetDirty(false);
                }
            }
            catch (Exception ex)
            {
                // 在实际应用中，这里应该显示错误消息
                Console.WriteLine($"加载文件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 保存文档
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>保存任务</returns>
        public async Task<bool> SaveAsync(string? filePath = null)
        {
            try
            {
                var targetPath = filePath ?? FilePath;
                if (string.IsNullOrEmpty(targetPath))
                {
                    return false; // 需要选择文件路径
                }

                await File.WriteAllTextAsync(targetPath, Content, Encoding.UTF8);
                FilePath = targetPath;
                SetDirty(false);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存文件失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 属性变更事件
        /// </summary>
        public new event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// 属性变更通知
        /// </summary>
        /// <param name="propertyName">属性名</param>
        private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}