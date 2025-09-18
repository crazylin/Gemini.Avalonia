using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Gemini.Avalonia.Framework.Logging;
using Gemini.Avalonia.Framework.Modules;

namespace Gemini.Avalonia.Framework
{
    /// <summary>
    /// 模块过滤服务实现，用于按需过滤模块相关的组件
    /// </summary>
    [Export(typeof(IModuleFilterService))]
    public class ModuleFilterService : IModuleFilterService
    {
        // 模块命名空间到模块名称的映射
        private static readonly Dictionary<string, string> NamespaceToModule = new(StringComparer.OrdinalIgnoreCase)
        {
            // 框架核心组件 - 总是启用
            { "Gemini.Avalonia.Framework", "CoreFramework" },
            
            // 核心模块映射（包含子命名空间）
            { "Gemini.Avalonia.Modules.ProjectManagement", "ProjectManagementModule" },
            { "Gemini.Avalonia.Modules.Output", "OutputModule" },
            { "Gemini.Avalonia.Modules.Properties", "PropertiesModule" },
            { "Gemini.Avalonia.Modules.Settings", "SettingsModule" },
            { "Gemini.Avalonia.Modules.MainMenu", "MainMenuModule" },
            { "Gemini.Avalonia.Modules.Theme", "ThemeModule" },
            { "Gemini.Avalonia.Modules.ToolBars", "ToolBarsModule" },
            { "Gemini.Avalonia.Modules.StatusBar", "StatusBarModule" },
            { "Gemini.Avalonia.Modules.UndoRedo", "UndoRedoModule" },
            { "Gemini.Avalonia.Modules.WindowManagement", "WindowManagementModule" },
            
            // Demo项目映射（包含子命名空间）
            { "Gemini.Avalonia.Demo", "DemoModule" } // Demo项目及其子命名空间
        };

        private static HashSet<string> _disabledModules = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        
        /// <summary>
        /// 全局禁用的模块列表（可以被外部设置）
        /// </summary>
        public static HashSet<string> DisabledModules 
        { 
            get => _disabledModules;
            set 
            { 
                _disabledModules = value ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                // 清除所有实例的缓存
                ClearAllCaches();
            }
        }

        private HashSet<string>? _cachedEnabledModules;
        
        /// <summary>
        /// 所有ModuleFilterService实例的静态列表，用于清除缓存
        /// </summary>
        private static readonly List<WeakReference<ModuleFilterService>> AllInstances = new();
        
        /// <summary>
        /// 构造函数 - 将实例添加到静态列表中
        /// </summary>
        public ModuleFilterService()
        {
            lock (AllInstances)
            {
                AllInstances.Add(new WeakReference<ModuleFilterService>(this));
            }
        }
        
        /// <summary>
        /// 清除所有实例的缓存
        /// </summary>
        private static void ClearAllCaches()
        {
            lock (AllInstances)
            {
                for (int i = AllInstances.Count - 1; i >= 0; i--)
                {
                    if (AllInstances[i].TryGetTarget(out var instance))
                    {
                        instance._cachedEnabledModules = null;
                    }
                    else
                    {
                        // 移除已经被垃圾回收的弱引用
                        AllInstances.RemoveAt(i);
                    }
                }
            }
        }
        
        /// <summary>
        /// 获取启用的模块名称集合
        /// </summary>
        public HashSet<string> GetEnabledModuleNames()
        {
            if (_cachedEnabledModules != null)
                return _cachedEnabledModules;
                
            var enabledModules = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            
            // 始终启用框架核心组件
            enabledModules.Add("CoreFramework");
            
            // 始终启用核心模块
            enabledModules.Add("MainMenuModule");
            enabledModules.Add("ThemeModule"); 
            enabledModules.Add("SettingsModule");
            enabledModules.Add("ToolBarsModule");
            enabledModules.Add("StatusBarModule");
            enabledModules.Add("UndoRedoModule");
            // WindowManagementModule 改为按需启用，不再默认启用
            
            // 始终启用Demo项目（主应用程序）
            enabledModules.Add("DemoModule");
            
            // 从ModuleConfiguration获取配置的模块
            try
            {
                var moduleConfigs = ModuleConfiguration.GetAllModuleConfigurations();
                foreach (var config in moduleConfigs)
                {
                    // 检查模块是否被全局禁用
                    if (!DisabledModules.Contains(config.Name))
                    {
                        enabledModules.Add(config.Name);
                        LogManager.Debug("ModuleFilterService", $"模块配置中启用的模块: {config.Name}");
                    }
                    else
                    {
                        LogManager.Info("ModuleFilterService", $"模块已被禁用: {config.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Warning("ModuleFilterService", $"获取模块配置时出错: {ex.Message}");
            }
            
            // 移除禁用列表中的所有模块
            enabledModules.ExceptWith(DisabledModules);
            
            if (DisabledModules.Count > 0)
            {
                LogManager.Info("ModuleFilterService", $"已禁用 {DisabledModules.Count} 个模块: {string.Join(", ", DisabledModules)}");
            }
            
            _cachedEnabledModules = enabledModules;
            return enabledModules;
        }
        
        /// <summary>
        /// 检查指定类型是否属于启用的模块
        /// </summary>
        public bool IsTypeFromEnabledModule(Type type)
        {
            var moduleName = GetModuleNameFromType(type);
            if (moduleName == null)
            {
                // 未知模块的类型，默认允许加载（向后兼容）
                LogManager.Debug("ModuleFilterService", $"类型 {type.Name} 属于未知模块，默认允许");
                return true;
            }
            
            var enabledModules = GetEnabledModuleNames();
            var isEnabled = enabledModules.Contains(moduleName);
            
            // 特殊处理：检查是否是用于显示其他模块工具的命令
            var toolDisplayDependency = CheckIfToolDisplayCommand(type);
            if (toolDisplayDependency.hasSpecialDependency)
            {
                var dependencyEnabled = enabledModules.Contains(toolDisplayDependency.dependencyModule!);
                LogManager.Debug("ModuleFilterService", 
                    $"工具显示命令 {type.Name} 依赖模块 {toolDisplayDependency.dependencyModule}，依赖状态: {(dependencyEnabled ? "已启用" : "已禁用")}");
                return dependencyEnabled;
            }
            
            LogManager.Debug("ModuleFilterService", $"类型 {type.Name} 属于模块 {moduleName}，{(isEnabled ? "已启用" : "已禁用")}");
            return isEnabled;
        }
        
        /// <summary>
        /// 检查是否是用于显示其他模块工具的命令
        /// </summary>
        private (bool hasSpecialDependency, string? dependencyModule) CheckIfToolDisplayCommand(Type type)
        {
            var typeName = type.Name;
            
            // 检查窗口管理相关的显示命令
            if (typeName.Contains("ShowProjectExplorer"))
            {
                return (true, "ProjectManagementModule");
            }
            
            if (typeName.Contains("ShowOutput"))
            {
                return (true, "OutputModule");
            }
            
            if (typeName.Contains("ShowProperties"))
            {
                return (true, "PropertiesModule");
            }
            
            return (false, null);
        }
        
        /// <summary>
        /// 根据类型的命名空间确定其所属的模块名称
        /// </summary>
        public string? GetModuleNameFromType(Type type)
        {
            var typeNamespace = type.Namespace ?? string.Empty;
            
            // 查找类型属于哪个模块
            foreach (var mapping in NamespaceToModule)
            {
                if (typeNamespace.StartsWith(mapping.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return mapping.Value;
                }
            }
            
            return null; // 未知模块
        }
    }
}
