using System;
using System.Collections.Generic;
using System.Linq;
using Gemini.Avalonia.Framework.Logging;

namespace Gemini.Avalonia.Framework.Modules
{
    /// <summary>
    /// 模块依赖解析器，负责处理模块间的依赖关系
    /// </summary>
    public class ModuleDependencyResolver
    {
        private readonly Dictionary<string, ModuleMetadata> _modules = new();
        
        /// <summary>
        /// 注册模块
        /// </summary>
        /// <param name="metadata">模块元数据</param>
        public void RegisterModule(ModuleMetadata metadata)
        {
            _modules[metadata.Name] = metadata;
        }
        
        /// <summary>
        /// 获取模块的加载顺序（考虑依赖关系）
        /// </summary>
        /// <param name="targetModules">目标模块列表</param>
        /// <returns>按依赖关系排序的模块列表</returns>
        public List<ModuleMetadata> GetLoadOrder(IEnumerable<ModuleMetadata> targetModules)
        {
            var result = new List<ModuleMetadata>();
            var visited = new HashSet<string>();
            var visiting = new HashSet<string>();
            
            foreach (var module in targetModules)
            {
                if (!visited.Contains(module.Name))
                {
                    var dependencyChain = new List<string>();
                    if (VisitModule(module, visited, visiting, result, dependencyChain))
                    {
                        // 成功处理了所有依赖
                    }
                    else
                    {
                        LogManager.Warning("ModuleDependencyResolver", $"模块 {module.Name} 存在循环依赖或依赖未满足");
                    }
                }
            }
            
            return result.Distinct().ToList();
        }
        
        /// <summary>
        /// 访问模块并处理其依赖关系（深度优先搜索）
        /// </summary>
        /// <param name="module">当前模块</param>
        /// <param name="visited">已访问的模块</param>
        /// <param name="visiting">正在访问的模块（用于检测循环依赖）</param>
        /// <param name="result">结果列表</param>
        /// <param name="dependencyChain">依赖链（用于调试循环依赖）</param>
        /// <returns>是否成功处理</returns>
        private bool VisitModule(ModuleMetadata module, HashSet<string> visited, 
            HashSet<string> visiting, List<ModuleMetadata> result, List<string> dependencyChain)
        {
            // 检测循环依赖
            if (visiting.Contains(module.Name))
            {
                dependencyChain.Add(module.Name);
                LogManager.Error("ModuleDependencyResolver", 
                    $"检测到循环依赖: {string.Join(" -> ", dependencyChain)}");
                return false;
            }
            
            // 如果已经访问过，直接返回
            if (visited.Contains(module.Name))
            {
                return true;
            }
            
            visiting.Add(module.Name);
            dependencyChain.Add(module.Name);
            
            // 首先处理所有依赖
            foreach (var dependencyName in module.Dependencies)
            {
                if (_modules.TryGetValue(dependencyName, out var dependency))
                {
                    if (!VisitModule(dependency, visited, visiting, result, dependencyChain))
                    {
                        return false; // 依赖处理失败
                    }
                }
                else
                {
                    LogManager.Warning("ModuleDependencyResolver", 
                        $"模块 {module.Name} 的依赖 {dependencyName} 未找到");
                    // 继续处理其他依赖，而不是立即失败
                }
            }
            
            // 依赖处理完成后，添加当前模块
            visiting.Remove(module.Name);
            dependencyChain.RemoveAt(dependencyChain.Count - 1);
            visited.Add(module.Name);
            result.Add(module);
            
            return true;
        }
        
        /// <summary>
        /// 检查指定模块的所有依赖是否都已加载
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="loadedModules">已加载的模块集合</param>
        /// <returns>依赖是否都已满足</returns>
        public bool AreDependenciesLoaded(string moduleName, HashSet<string> loadedModules)
        {
            if (!_modules.TryGetValue(moduleName, out var module))
            {
                return false;
            }
            
            return module.Dependencies.All(dep => loadedModules.Contains(dep));
        }
        
        /// <summary>
        /// 获取模块的直接依赖
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>依赖模块列表</returns>
        public List<string> GetDirectDependencies(string moduleName)
        {
            if (_modules.TryGetValue(moduleName, out var module))
            {
                return module.Dependencies.ToList();
            }
            return new List<string>();
        }
        
        /// <summary>
        /// 获取模块的所有传递依赖
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>所有依赖模块列表（包括传递依赖）</returns>
        public List<string> GetAllDependencies(string moduleName)
        {
            var allDeps = new HashSet<string>();
            var visited = new HashSet<string>();
            
            CollectDependencies(moduleName, allDeps, visited);
            
            return allDeps.ToList();
        }
        
        /// <summary>
        /// 递归收集所有依赖
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="allDeps">所有依赖集合</param>
        /// <param name="visited">已访问的模块</param>
        private void CollectDependencies(string moduleName, HashSet<string> allDeps, HashSet<string> visited)
        {
            if (visited.Contains(moduleName) || !_modules.TryGetValue(moduleName, out var module))
            {
                return;
            }
            
            visited.Add(moduleName);
            
            foreach (var dep in module.Dependencies)
            {
                allDeps.Add(dep);
                CollectDependencies(dep, allDeps, visited);
            }
        }
    }
}
