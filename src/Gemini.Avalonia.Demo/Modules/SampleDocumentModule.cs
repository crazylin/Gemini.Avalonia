using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Styling;
using Gemini.Avalonia.Demo.ViewModels;
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Framework.Services;

namespace Gemini.Avalonia.Demo.Modules
{
    /// <summary>
    /// 示例文档模块
    /// </summary>
    public class SampleDocumentModule : ModuleBase
    {
        /// <summary>
        /// 获取默认文档
        /// </summary>
        public override IEnumerable<IDocument> DefaultDocuments
        {
            get
            {
                // 创建一个示例文档
                yield return new SampleDocumentViewModel
                {
                    Title = "示例文档 1",
                    Content = "这是一个示例文档的内容。\n\n您可以在这里编辑文本内容。"
                };
                
                yield return new SampleDocumentViewModel
                {
                    Title = "示例文档 2",
                    Content = "这是另一个示例文档。\n\n演示多文档支持。"
                };
            }
        }
        
        /// <summary>
        /// 初始化模块
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            
            // 通过IoC容器获取Shell服务
            var shell = IoC.Get<IShell>();
            
            // 显示工具栏
            shell.ToolBars.Visible = true;
            
            // 注册文档类型或其他初始化逻辑
        }
        
        /// <summary>
        /// 异步后初始化
        /// </summary>
        /// <returns>异步任务</returns>
        public override async Task PostInitializeAsync()
        {
            await base.PostInitializeAsync();
            
            // 打开默认文档
            // OpenDefaultDocuments(); // 需要通过其他方式获取Shell实例
        }
    }
}