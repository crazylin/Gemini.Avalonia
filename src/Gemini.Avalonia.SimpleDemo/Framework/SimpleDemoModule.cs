using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Avalonia.Styling;
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Framework.Logging;
using Gemini.Avalonia.Framework.Services;
using Gemini.Avalonia.SimpleDemo.Documents;

namespace Gemini.Avalonia.SimpleDemo.Framework
{
    /// <summary>
    /// 简单Demo模块
    /// </summary>
    [Export(typeof(IModule))]
    public class SimpleDemoModule : ModuleBase
    {
        /// <summary>
        /// 全局资源字典集合
        /// </summary>
        public override IEnumerable<IStyle> GlobalResourceDictionaries
        {
            get { yield break; }
        }

        /// <summary>
        /// 默认文档集合
        /// </summary>
        public override IEnumerable<IDocument> DefaultDocuments
        {
            get
            {
                yield return CreateWelcomeDocument();
                yield return CreateSampleDocument();
            }
        }

        /// <summary>
        /// 默认工具类型集合
        /// </summary>
        public override IEnumerable<Type> DefaultTools
        {
            get { yield break; }
        }

        /// <summary>
        /// 预初始化
        /// </summary>
        public override void PreInitialize()
        {
            LogManager.Info("SimpleDemoModule", "简单Demo模块预初始化");
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            LogManager.Info("SimpleDemoModule", "简单Demo模块初始化");
        }

        /// <summary>
        /// 异步后初始化
        /// </summary>
        /// <returns>任务</returns>
        public override async Task PostInitializeAsync()
        {
            LogManager.Info("SimpleDemoModule", "简单Demo模块后初始化");
            await Task.CompletedTask;
        }

        /// <summary>
        /// 创建欢迎文档
        /// </summary>
        /// <returns>欢迎文档</returns>
        private SimpleDocument CreateWelcomeDocument()
        {
            var document = new SimpleDocument
            {
                Content = GetWelcomeContent()
            };
            document.DisplayName = "欢迎使用 SimpleDemo";
            return document;
        }

        /// <summary>
        /// 创建示例文档
        /// </summary>
        /// <returns>示例文档</returns>
        private SimpleDocument CreateSampleDocument()
        {
            var document = new SimpleDocument
            {
                Content = GetSampleContent()
            };
            document.DisplayName = "示例文档";
            return document;
        }

        /// <summary>
        /// 获取欢迎内容
        /// </summary>
        /// <returns>欢迎内容</returns>
        private string GetWelcomeContent()
        {
            return @"🎉 欢迎使用 Gemini.Avalonia 简单Demo！

这是一个最简化的演示项目，展示了框架的核心功能：

✨ 主要特性：
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

📄 文档系统
   • 多文档支持
   • 标签页导航
   • 自动保存检测

🏗️ 模块化架构
   • MEF依赖注入
   • 组件自动发现
   • 松耦合设计

🎨 用户界面
   • 现代化布局
   • 响应式设计
   • 主题支持

📋 基本功能演示：
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

1. 这个窗口展示了多文档界面
2. 你可以编辑文本内容
3. 标题栏会显示修改状态
4. 支持标签页切换

🚀 开始使用：
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

→ 切换到 ""示例文档"" 标签页查看更多内容
→ 尝试编辑文本内容
→ 观察标题栏的变化

这个框架为构建复杂的桌面应用提供了强大的基础！";
        }

        /// <summary>
        /// 获取示例内容
        /// </summary>
        /// <returns>示例内容</returns>
        private string GetSampleContent()
        {
            return @"📝 这是一个可编辑的示例文档

你可以在这里编辑任何内容。试试修改下面的文本：

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Hello World! 你好世界！

这是一个文本编辑器的演示。

✨ 支持的功能：
• 多行文本编辑
• 实时保存状态检测
• Unicode字符显示
• 自动换行

🎯 技术特性：
• MVVM数据绑定
• 属性变更通知
• 文档生命周期管理

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

💡 提示：
当你修改内容时，标题栏会显示 * 表示文档已修改。

🔧 框架扩展能力：
这个简单的文本编辑器只是开始，框架支持：
- 语法高亮编辑器
- 富文本编辑器  
- 图片查看器
- 数据表格编辑器
- 图表和可视化组件
- 自定义文档类型
- 插件系统
- 等等...

快来尝试编辑这些内容吧！ 🚀";
        }
    }
}
