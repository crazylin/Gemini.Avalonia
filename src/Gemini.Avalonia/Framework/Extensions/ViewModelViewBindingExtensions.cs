using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;

namespace Gemini.Avalonia.Framework.Extensions
{
    /// <summary>
    /// ViewModel和View自动绑定扩展
    /// </summary>
    public static class ViewModelViewBindingExtensions
    {
        
        /// <summary>
        /// 为应用程序注册ViewModel和View的自动绑定（使用默认配置）
        /// </summary>
        /// <param name="application">应用程序实例</param>
        /// <param name="assemblies">要扫描的程序集列表</param>
        public static void RegisterViewModelViewBindings(this Application application, params Assembly[] assemblies)
        {
            RegisterViewModelViewBindings(application, ViewModelViewBindingOptions.Default, assemblies);
        }
        
        /// <summary>
        /// 为应用程序注册ViewModel和View的自动绑定（使用自定义配置）
        /// </summary>
        /// <param name="application">应用程序实例</param>
        /// <param name="options">绑定配置选项</param>
        /// <param name="assemblies">要扫描的程序集列表</param>
        public static void RegisterViewModelViewBindings(this Application application, ViewModelViewBindingOptions options, params Assembly[] assemblies)
        {
            Console.WriteLine("[ViewModelViewBinding] 开始注册ViewModel和View的自动绑定");
            
            if (application == null)
                throw new ArgumentNullException(nameof(application));
                
            if (options == null)
                throw new ArgumentNullException(nameof(options));
                
            if (assemblies == null || assemblies.Length == 0)
             {
                 Console.WriteLine("[ViewModelViewBinding] 未提供程序集列表，将扫描当前应用域中的所有程序集");
                 
                 assemblies = AppDomain.CurrentDomain.GetAssemblies()
                     .Where(a => options.AssemblyFilter?.Invoke(a) ?? true)
                     .ToArray();
             }
             
             Console.WriteLine($"[ViewModelViewBinding] 将扫描 {assemblies.Length} 个程序集");
             foreach (var assembly in assemblies)
             {
                 Console.WriteLine($"[ViewModelViewBinding] 扫描程序集: {assembly.FullName}");
             }
             
             var bindings = DiscoverViewModelViewBindings(assemblies, options);
             
             Console.WriteLine($"[ViewModelViewBinding] 发现 {bindings.Count} 个ViewModel-View绑定关系");
             
             foreach (var binding in bindings)
             {
                 Console.WriteLine($"[ViewModelViewBinding] 正在注册绑定: {binding.ViewModelType.Name} -> {binding.ViewType.Name}");
                 var dataTemplate = CreateDataTemplate(binding.ViewModelType, binding.ViewType);
                 application.DataTemplates.Add(dataTemplate);
                 Console.WriteLine($"[ViewModelViewBinding] 绑定注册完成: {binding.ViewModelType.Name} -> {binding.ViewType.Name}");
             }
             
             Console.WriteLine($"[ViewModelViewBinding] 所有绑定注册完成，共注册了 {bindings.Count} 个DataTemplate");
        }
        
        /// <summary>
        /// 发现ViewModel和View的绑定关系
        /// </summary>
        /// <param name="assemblies">要扫描的程序集</param>
        /// <param name="options">绑定配置选项</param>
        /// <returns>绑定关系列表</returns>
        private static List<ViewModelViewBinding> DiscoverViewModelViewBindings(Assembly[] assemblies, ViewModelViewBindingOptions options)
        {
            var bindings = new List<ViewModelViewBinding>();
            Console.WriteLine("[ViewModelViewBinding] 开始发现ViewModel和View的绑定关系");

            foreach (var assembly in assemblies)
            {
                try
                {
                    Console.WriteLine($"[ViewModelViewBinding] 正在扫描程序集: {assembly.GetName().Name}");
                    
                    // 获取所有ViewModel类型
                    var viewModelTypes = assembly.GetTypes()
                        .Where(t => options.ViewModelFilter?.Invoke(t) ?? 
                                   (t.Name.EndsWith("ViewModel") && t.IsClass && !t.IsAbstract && t.IsPublic))
                        .ToList();

                    Console.WriteLine($"[ViewModelViewBinding] 在程序集 {assembly.GetName().Name} 中找到 {viewModelTypes.Count} 个ViewModel类型");

                    foreach (var viewModelType in viewModelTypes)
                     {
                         Console.WriteLine($"[ViewModelViewBinding] 处理ViewModel: {viewModelType.FullName}");
                         
                         // 跳过排除的ViewModel类型
                         if (options.ExcludedViewModelTypes.Contains(viewModelType))
                         {
                             Console.WriteLine($"[ViewModelViewBinding] 跳过排除的ViewModel: {viewModelType.Name}");
                             continue;
                         }
                             
                         // 根据约定查找对应的View类型
                         var viewTypeName = options.CustomNamingConvention?.Invoke(viewModelType.Name) ?? 
                                           viewModelType.Name.Replace(options.ViewModelSuffix, options.ViewSuffix);
                         
                         Console.WriteLine($"[ViewModelViewBinding] 查找对应的View类型: {viewTypeName}");
                         
                         var viewType = assembly.GetTypes()
                             .FirstOrDefault(t => t.Name == viewTypeName && 
                                                (options.ViewFilter?.Invoke(t) ?? true) &&
                                                !options.ExcludedViewTypes.Contains(t) &&
                                                typeof(Control).IsAssignableFrom(t) &&
                                                t.IsClass && 
                                                !t.IsAbstract && 
                                                t.IsPublic);
 
                         if (viewType != null)
                         {
                             Console.WriteLine($"[ViewModelViewBinding] 找到匹配的View: {viewType.FullName}");
                             bindings.Add(new ViewModelViewBinding(viewModelType, viewType));
                         }
                         else
                         {
                             Console.WriteLine($"[ViewModelViewBinding] 警告: 未找到 {viewModelType.Name} 对应的View: {viewTypeName}");
                             
                             // 列出程序集中所有可能的View类型
                             var allViewTypes = assembly.GetTypes()
                                 .Where(t => typeof(Control).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && t.IsPublic)
                                 .Select(t => t.Name)
                                 .ToList();
                             Console.WriteLine($"[ViewModelViewBinding] 程序集中可用的View类型: {string.Join(", ", allViewTypes)}");
                         }
                     }
                }
                catch (Exception ex)
                 {
                     Console.WriteLine($"[ViewModelViewBinding] 扫描程序集 {assembly.GetName().Name} 时出错: {ex.Message}");
                     Console.WriteLine($"[ViewModelViewBinding] 异常详情: {ex}");
                 }
            }

            return bindings;
        }

        /// <summary>
        /// 创建DataTemplate
        /// </summary>
        /// <param name="viewModelType">ViewModel类型</param>
        /// <param name="viewType">View类型</param>
        /// <returns>DataTemplate实例</returns>
        private static IDataTemplate CreateDataTemplate(Type viewModelType, Type viewType)
        {
            return new FuncDataTemplate(viewModelType, (data, scope) =>
            {
                try
                {
                    var view = (Control?)Activator.CreateInstance(viewType);
                    if (view != null)
                        view.DataContext = data;
                    return view;
                }
                catch (Exception ex)
                {
                    return null;
                }
            });
        }

        /// <summary>
        /// 手动注册单个ViewModel和View的绑定
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel类型</typeparam>
        /// <typeparam name="TView">View类型</typeparam>
        /// <param name="application">应用程序实例</param>
        public static void RegisterViewModelViewBinding<TViewModel, TView>(this Application application)
            where TView : Control, new()
        {
            var dataTemplate = new FuncDataTemplate<TViewModel>((data, scope) =>
            {
                var view = new TView();
                view.DataContext = data;
                return view;
            });

            application.DataTemplates.Add(dataTemplate);
            // 手动注册绑定
        }

        /// <summary>
        /// 清除所有自动注册的DataTemplate
        /// </summary>
        /// <param name="application">应用程序实例</param>
        public static void ClearViewModelViewBindings(this Application application)
        {
            application.DataTemplates.Clear();
            // 清除所有DataTemplate绑定
        }
    }
    
    /// <summary>
    /// ViewModel和View绑定关系
    /// </summary>
    internal class ViewModelViewBinding
    {
        public Type ViewModelType { get; }
        public Type ViewType { get; }
        
        public ViewModelViewBinding(Type viewModelType, Type viewType)
        {
            ViewModelType = viewModelType ?? throw new ArgumentNullException(nameof(viewModelType));
            ViewType = viewType ?? throw new ArgumentNullException(nameof(viewType));
        }
    }
}