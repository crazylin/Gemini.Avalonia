using System;
using System.Collections.Generic;

namespace Gemini.Avalonia.Framework.Modules
{
    /// <summary>
    /// 模块元数据，描述模块的基本信息和加载策略
    /// </summary>
    public class ModuleMetadata
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 模块版本
        /// </summary>
        public string Version { get; set; } = "1.0.0";
        
        /// <summary>
        /// 模块描述
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// 模块分类
        /// </summary>
        public ModuleCategory Category { get; set; } = ModuleCategory.Feature;
        
        /// <summary>
        /// 模块类型
        /// </summary>
        public Type? ModuleType { get; set; }
        
        /// <summary>
        /// 依赖的模块列表
        /// </summary>
        public List<string> Dependencies { get; set; } = new();
        
        /// <summary>
        /// 是否已加载
        /// </summary>
        public bool IsLoaded { get; set; } = false;
        
        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool IsInitialized { get; set; } = false;
        
        /// <summary>
        /// 加载优先级（数字越小优先级越高）
        /// </summary>
        public int Priority { get; set; } = 100;
        
        /// <summary>
        /// 是否允许延迟加载
        /// </summary>
        public bool AllowLazyLoading { get; set; } = true;
        
        /// <summary>
        /// 模块实例
        /// </summary>
        public IModule? Instance { get; set; }
    }
}
