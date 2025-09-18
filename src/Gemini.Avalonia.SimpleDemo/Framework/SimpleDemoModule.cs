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
        /// 创建示例文档
        /// </summary>
        /// <returns>示例文档</returns>
        private SimpleDocumentViewModel CreateSampleDocument()
        {
            var document = new SimpleDocumentViewModel
            {
                Content = GetSampleContent()
            };
            document.DisplayName = "示例文档";
            return document;
        }

 
        /// <summary>
        /// 获取示例内容
        /// </summary>
        /// <returns>示例内容</returns>
        private string GetSampleContent()
        {
            return @"测试文档";
        }
    }
}
