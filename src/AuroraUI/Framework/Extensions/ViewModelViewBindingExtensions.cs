using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;

namespace AuroraUI.Framework.Extensions
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
            if (application == null)
                throw new ArgumentNullException(nameof(application));
                
            if (options == null)
                throw new ArgumentNullException(nameof(options));
                
            if (assemblies == null || assemblies.Length == 0)
             {
                 assemblies = AppDomain.CurrentDomain.GetAssemblies()
                     .Where(a => options.AssemblyFilter?.Invoke(a) ?? true)
                     .ToArray();
             }
             
             var bindings = DiscoverViewModelViewBindings(assemblies, options);
             
             foreach (var binding in bindings)
             {
                 var dataTemplate = CreateDataTemplate(binding.ViewModelType, binding.ViewType);
                 application.DataTemplates.Add(dataTemplate);
             }
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

            foreach (var assembly in assemblies)
            {
                try
                {
                    
                    // 获取所有ViewModel类型
                    var viewModelTypes = assembly.GetTypes()
                        .Where(t => options.ViewModelFilter?.Invoke(t) ?? 
                                   (t.Name.EndsWith("ViewModel") && t.IsClass && !t.IsAbstract && t.IsPublic))
                        .ToList();


                    foreach (var viewModelType in viewModelTypes)
                     {
                         
                         // 跳过排除的ViewModel类型
                         if (options.ExcludedViewModelTypes.Contains(viewModelType))
                         {
                             continue;
                         }
                             
                         // 根据约定查找对应的View类型
                         var viewTypeName = options.CustomNamingConvention?.Invoke(viewModelType.Name) ?? 
                                           viewModelType.Name.Replace(options.ViewModelSuffix, options.ViewSuffix);
                         
                         
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
                             bindings.Add(new ViewModelViewBinding(viewModelType, viewType));
                         }
                         else
                         {
                             if (options.ShowWarningsForMissingViews)
                             {
                                 Console.WriteLine($"[ViewModelViewBinding] 警告: 未找到 {viewModelType.Name} 对应的View: {viewTypeName}");
                             }
                         }
                     }
                }
                catch (Exception ex)
                 {
                     Console.WriteLine($"[ViewModelViewBinding] 扫描程序集 {assembly.GetName().Name} 时出错: {ex.Message}");
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
                    {
                        view.DataContext = data;
                        return view;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ViewModelViewBinding] 创建 {viewType.Name} 时出错: {ex.Message}");
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