using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Styling;

namespace Gemini.Avalonia.Framework
{
    /// <summary>
    /// 模块接口，定义模块的基本结构
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// 全局资源字典集合
        /// </summary>
        IEnumerable<IStyle> GlobalResourceDictionaries { get; }
        
        /// <summary>
        /// 默认文档集合
        /// </summary>
        IEnumerable<IDocument> DefaultDocuments { get; }
        
        /// <summary>
        /// 默认工具类型集合
        /// </summary>
        IEnumerable<Type> DefaultTools { get; }
        
        /// <summary>
        /// 预初始化
        /// </summary>
        void PreInitialize();
        
        /// <summary>
        /// 初始化
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// 异步后初始化
        /// </summary>
        /// <returns>任务</returns>
        Task PostInitializeAsync();
    }
}