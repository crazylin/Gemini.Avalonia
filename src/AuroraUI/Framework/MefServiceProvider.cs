using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraUI.Framework
{
    /// <summary>
    /// MEF容器到IServiceProvider的适配器
    /// </summary>
    public class MefServiceProvider : IServiceProvider
    {
        private readonly CompositionContainer _container;

        public MefServiceProvider(CompositionContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public object? GetService(Type serviceType)
        {
            try
            {
                // 使用反射调用泛型方法
                var method = typeof(ExportProvider).GetMethod("GetExportedValue", new Type[0]);
                var genericMethod = method?.MakeGenericMethod(serviceType);
                return genericMethod?.Invoke(_container, null);
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// IServiceProvider的扩展方法
    /// </summary>
    public static class ServiceProviderExtensions
    {
        public static T? GetService<T>(this IServiceProvider serviceProvider) where T : class
        {
            return serviceProvider.GetService(typeof(T)) as T;
        }

        public static T GetRequiredService<T>(this IServiceProvider serviceProvider) where T : class
        {
            var service = serviceProvider.GetService<T>();
            if (service == null)
                throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered.");
            return service;
        }
    }
}