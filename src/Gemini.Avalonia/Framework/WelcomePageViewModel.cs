using System;
using System.Windows.Input;
using Gemini.Avalonia.Framework;
using ReactiveUI;

namespace Gemini.Avalonia.Framework
{
    /// <summary>
    /// 欢迎页面视图模型
    /// </summary>
    public class WelcomePageViewModel : Document
    {
        private string _content;

        /// <summary>
        /// 页面内容
        /// </summary>
        public string Content
        {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WelcomePageViewModel()
        {
            DisplayName = "主页";
            Title = "主页";
            
            Content = @"🎉 欢迎使用 Gemini.Avalonia 框架！

这是一个基于 Avalonia UI 的现代化桌面应用程序框架，提供了：

✨ 主要特性：
• 🏗️ 模块化架构设计
• 🔧 可扩展的插件系统  
• 🎨 现代化的UI组件
• 📁 文档管理系统
• 🛠️ 工具窗口支持
• 🌍 国际化支持
• 📊 性能监控
• 🔍 日志系统

🚀 快速开始：
1. 点击 '文件' → '新建项目' 创建新项目
2. 使用 'Demo' 菜单体验示例功能
3. 查看左侧项目资源管理器
4. 使用右侧属性面板和底部输出窗口

📖 了解更多：
• 查看示例文档了解编辑功能
• 尝试工具栏和菜单命令
• 体验多文档标签页管理

开始您的 Avalonia 开发之旅吧！🎯";

            // 欢迎页面不能关闭
            CanClose = false;
        }
    }
}
