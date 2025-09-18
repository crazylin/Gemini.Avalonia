using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Styling;
using AuroraUI.Demo.ViewModels;
using AuroraUI.Framework;
using AuroraUI.Framework.Logging;
using AuroraUI.Framework.Modules;
using AuroraUI.Framework.Services;

namespace AuroraUI.Demo.Modules
{
    /// <summary>
    /// 示例文档模块 - 支持延迟加载
    /// </summary>
    public class SampleDocumentModule : LazyModuleBase
    {
        /// <summary>
        /// 创建模块元数据
        /// </summary>
        /// <returns>模块元数据</returns>
        protected override ModuleMetadata CreateMetadata()
        {
            return new ModuleMetadata
            {
                Name = "SampleDocumentModule",
                Description = "示例文档模块，演示文档管理功能",
                Category = ModuleCategory.Feature,
                Priority = 200,
                AllowLazyLoading = true,
                ModuleType = GetType(),
                Dependencies = new List<string> { "MainMenuModule" }
            };
        }
        
        /// <summary>
        /// 检查是否应该加载此模块
        /// </summary>
        /// <returns>如果应该加载返回true</returns>
        public override bool ShouldLoad()
        {
            // Demo模块可以根据配置或用户偏好决定是否加载
            return true; // 默认总是加载Demo文档
        }
        
        /// <summary>
        /// 获取默认文档
        /// </summary>
        public override IEnumerable<IDocument> DefaultDocuments
        {
            get
            {
                // 创建一个示例文档
                var doc1 = new SampleDocumentViewModel("这是一个示例文档的内容。\n\n您可以在这里编辑文本内容。\n\n支持多行文本编辑和保存功能。");
                doc1.DisplayName = "示例文档 1";
                yield return doc1;
                
                var doc2 = new SampleDocumentViewModel("这是另一个示例文档。\n\n演示多文档支持。\n\n您可以同时打开多个文档进行编辑。");
                doc2.DisplayName = "示例文档 2";
                yield return doc2;
            }
        }
        
        /// <summary>
        /// 初始化模块
        /// </summary>
        public override void Initialize()
        {
            if (!IsLoaded) return;
            
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
            
            // 等待一段时间确保Shell完全准备好
            await Task.Delay(2000);
            
            // 打开默认文档
            try
            {
                Console.WriteLine("🔄 开始尝试打开默认文档...");
                LogManager.Info("SampleDocumentModule", "开始尝试打开默认文档");
                
                var shell = IoC.Get<IShell>();
                if (shell != null)
                {
                    Console.WriteLine($"🔄 Shell获取成功，准备打开 {DefaultDocuments.Count()} 个文档");
                    
                    var docs = DefaultDocuments.ToList();
                    foreach (var document in docs)
                    {
                        Console.WriteLine($"🔄 正在打开文档: {document.DisplayName}");
                        LogManager.Info("SampleDocumentModule", $"正在打开文档: {document.DisplayName}");
                        
                        await shell.OpenDocumentAsync(document);
                        
                        Console.WriteLine($"✅ 文档已打开: {document.DisplayName}");
                        LogManager.Info("SampleDocumentModule", $"文档已打开: {document.DisplayName}");
                    }
                    
                    Console.WriteLine($"🎉 所有文档打开完成！");
                    LogManager.Info("SampleDocumentModule", $"已打开 {docs.Count} 个默认文档");
                }
                else
                {
                    Console.WriteLine("❌ 无法获取Shell实例");
                    LogManager.Error("SampleDocumentModule", "无法获取Shell实例");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 打开默认文档时出错: {ex.Message}");
                LogManager.Error("SampleDocumentModule", $"打开默认文档时出错: {ex.Message}");
                LogManager.Error("SampleDocumentModule", $"异常详情: {ex}");
            }
        }
    }
}