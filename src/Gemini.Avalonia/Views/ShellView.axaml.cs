using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Gemini.Avalonia.Framework.Services;
using System;
using System.Diagnostics;

namespace Gemini.Avalonia.Views
{
    /// <summary>
    /// Shell主窗口视图
    /// </summary>
    public partial class ShellView : Window
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ShellView()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
        {
            // 初始化失败
            throw;
            }
        }
        
        /// <summary>
        /// 带视图模型的构造函数
        /// </summary>
        /// <param name="viewModel">Shell视图模型</param>
        public ShellView(ShellViewModel viewModel) : this()
        {
            try
            {
                DataContext = viewModel;
            }
            catch (Exception ex)
        {
            // 设置DataContext失败
            throw;
            }
        }
        
        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitializeComponent()
        {
            try
            {
                AvaloniaXamlLoader.Load(this);
            }
            catch (Exception ex)
        {
            // XAML加载失败
            throw;
            }
        }
    }
}