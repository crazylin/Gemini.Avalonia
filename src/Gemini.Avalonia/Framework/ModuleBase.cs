using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Styling;
using Gemini.Avalonia.Framework.Services;

namespace Gemini.Avalonia.Framework
{
    /// <summary>
    /// 模块基类，提供IModule接口的基础实现
    /// </summary>
    public abstract class ModuleBase : IModule
    {
        /// <summary>
        /// 全局资源字典集合
        /// </summary>
        public virtual IEnumerable<IStyle> GlobalResourceDictionaries
        {
            get { yield break; } // 默认返回空集合
        }
        
        /// <summary>
        /// 默认文档集合
        /// </summary>
        public virtual IEnumerable<IDocument> DefaultDocuments
        {
            get { yield break; } // 默认返回空集合
        }
        
        /// <summary>
        /// 默认工具类型集合
        /// </summary>
        public virtual IEnumerable<Type> DefaultTools
        {
            get { yield break; } // 默认返回空集合
        }
        
        /// <summary>
        /// 预初始化，在依赖注入容器配置之前调用
        /// </summary>
        public virtual void PreInitialize()
        {
            // 默认实现为空，子类可重写
        }
        
        /// <summary>
        /// 初始化，在依赖注入容器配置之后调用
        /// </summary>
        public virtual void Initialize()
        {
            // 默认实现为空，子类可重写
        }
        
        /// <summary>
        /// 异步后初始化，在所有模块初始化完成后调用
        /// </summary>
        /// <returns>异步任务</returns>
        public virtual async Task PostInitializeAsync()
        {
            // 默认实现为空，子类可重写
            await Task.CompletedTask;
        }
        
        /// <summary>
        /// 注册默认工具到Shell
        /// </summary>
        /// <param name="shell">Shell服务</param>
        protected virtual void RegisterDefaultTools(IShell shell)
        {
            foreach (var toolType in DefaultTools)
            {
                if (typeof(ITool).IsAssignableFrom(toolType))
                {
                    try
                    {
                        var tool = Activator.CreateInstance(toolType) as ITool;
                        if (tool != null)
                        {
                            shell.RegisterTool(tool);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 记录错误但不中断初始化过程
                        System.Diagnostics.Debug.WriteLine($"Failed to create tool {toolType.Name}: {ex.Message}");
                    }
                }
            }
        }
        
        /// <summary>
        /// 打开默认文档到Shell
        /// </summary>
        /// <param name="shell">Shell服务</param>
        protected virtual async Task OpenDefaultDocumentsAsync(IShell shell)
        {
            foreach (var document in DefaultDocuments)
            {
                try
                {
                    await shell.OpenDocumentAsync(document);
                }
                catch (Exception ex)
                {
                    // 记录错误但不中断初始化过程
                    System.Diagnostics.Debug.WriteLine($"Failed to open default document {document.DisplayName}: {ex.Message}");
                }
            }
        }
    }
}