using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;

namespace AuroraUI.Framework.Extensions
{
    /// <summary>
    /// ViewModel和View自动绑定配置选项
    /// </summary>
    public class ViewModelViewBindingOptions
    {
        /// <summary>
        /// ViewModel后缀，默认为"ViewModel"
        /// </summary>
        public string ViewModelSuffix { get; set; } = "ViewModel";
        
        /// <summary>
        /// View后缀，默认为"View"
        /// </summary>
        public string ViewSuffix { get; set; } = "View";
        
        /// <summary>
        /// 是否启用详细日志输出
        /// </summary>
        public bool EnableVerboseLogging { get; set; } = true;
        
        /// <summary>
        /// 是否在找不到对应View时显示警告
        /// </summary>
        public bool ShowWarningsForMissingViews { get; set; } = true;
        
        /// <summary>
        /// 程序集过滤器，用于筛选要扫描的程序集
        /// </summary>
        public Func<Assembly, bool>? AssemblyFilter { get; set; }
        
        /// <summary>
        /// ViewModel类型过滤器，用于筛选要处理的ViewModel类型
        /// </summary>
        public Func<Type, bool>? ViewModelFilter { get; set; }
        
        /// <summary>
        /// View类型过滤器，用于筛选要处理的View类型
        /// </summary>
        public Func<Type, bool>? ViewFilter { get; set; }
        
        /// <summary>
        /// 自定义命名约定，用于从ViewModel类型名称生成View类型名称
        /// </summary>
        public Func<string, string>? CustomNamingConvention { get; set; }
        
        /// <summary>
        /// 排除的ViewModel类型列表
        /// </summary>
        public HashSet<Type> ExcludedViewModelTypes { get; set; } = new();
        
        /// <summary>
        /// 排除的View类型列表
        /// </summary>
        public HashSet<Type> ExcludedViewTypes { get; set; } = new();
        
        /// <summary>
        /// 判断程序集是否为系统程序集
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <returns>如果是系统程序集则返回true</returns>
        private static bool IsSystemAssembly(string? assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName))
                return true;
                
            // 系统程序集前缀列表
            var systemPrefixes = new[]
            {
                "System", "Microsoft", "mscorlib", "netstandard", "Avalonia.Base",
                "Avalonia.Controls", "Avalonia.Input", "Avalonia.Interactivity",
                "Avalonia.Layout", "Avalonia.Logging", "Avalonia.Markup",
                "Avalonia.Metadata", "Avalonia.Platform", "Avalonia.Styling",
                "Avalonia.Utilities", "Avalonia.Visuals", "ReactiveUI.Events",
                "Splat", "DynamicData", "Dock.Model"
            };
            
            return systemPrefixes.Any(prefix => assemblyName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }
        
        /// <summary>
        /// 默认配置
        /// </summary>
        public static ViewModelViewBindingOptions Default => new ViewModelViewBindingOptions
        {
            AssemblyFilter = assembly => 
            {
                return !assembly.IsDynamic && 
                       !string.IsNullOrEmpty(assembly.Location) &&
                       !IsSystemAssembly(assembly.GetName().Name);
            },
            
            ViewModelFilter = type => 
                type.Name.EndsWith("ViewModel") && 
                type.IsClass && 
                !type.IsAbstract && 
                type.IsPublic,
                
            ViewFilter = type => 
                type.Name.EndsWith("View") && 
                type.IsClass && 
                !type.IsAbstract && 
                type.IsPublic &&
                typeof(Control).IsAssignableFrom(type)
        };
        
        /// <summary>
        /// 创建用于Aurora框架的配置
        /// </summary>
        public static ViewModelViewBindingOptions ForAuroraFramework => new ViewModelViewBindingOptions
        {
            AssemblyFilter = assembly => 
            {
                return !assembly.IsDynamic && 
                       !string.IsNullOrEmpty(assembly.Location) &&
                       !IsSystemAssembly(assembly.GetName().Name);
            },
            
            ViewModelFilter = type => 
                type.Name.EndsWith("ViewModel") && 
                type.IsClass && 
                !type.IsAbstract && 
                type.IsPublic,
                
            ViewFilter = type => 
                type.Name.EndsWith("View") && 
                type.IsClass && 
                !type.IsAbstract && 
                type.IsPublic &&
                typeof(Control).IsAssignableFrom(type),
                
            EnableVerboseLogging = true,
            ShowWarningsForMissingViews = true
        };
    }
}