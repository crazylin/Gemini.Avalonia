using System.Collections.Generic;

namespace AuroraUI.Framework
{
    /// <summary>
    /// 模块过滤服务接口，用于按需过滤模块相关的组件
    /// </summary>
    public interface IModuleFilterService
    {
        /// <summary>
        /// 获取启用的模块名称集合
        /// </summary>
        /// <returns>启用的模块名称集合</returns>
        HashSet<string> GetEnabledModuleNames();
        
        /// <summary>
        /// 检查指定类型是否属于启用的模块
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <returns>如果属于启用模块返回true，否则返回false</returns>
        bool IsTypeFromEnabledModule(System.Type type);
        
        /// <summary>
        /// 根据类型的命名空间确定其所属的模块名称
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>模块名称，如果无法确定返回null</returns>
        string? GetModuleNameFromType(System.Type type);
    }
}
