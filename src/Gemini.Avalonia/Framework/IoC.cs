using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;

namespace Gemini.Avalonia.Framework
{
    /// <summary>
    /// IoC容器适配器，提供与原版Gemini兼容的服务定位功能
    /// </summary>
    public static class IoC
    {
        private static CompositionContainer _container;
        
        /// <summary>
        /// 设置MEF容器
        /// </summary>
        /// <param name="container">MEF容器实例</param>
        internal static void SetContainer(CompositionContainer container)
        {
            _container = container;
        }
        
        /// <summary>
        /// 获取指定类型的服务实例
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>服务实例</returns>
        public static T Get<T>()
        {
            if (_container == null)
                throw new InvalidOperationException("IoC容器尚未初始化");
                
            return _container.GetExportedValue<T>();
        }
        
        /// <summary>
        /// 获取指定类型的所有服务实例
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>服务实例集合</returns>
        public static IEnumerable<T> GetAll<T>()
        {
            if (_container == null)
                throw new InvalidOperationException("IoC容器尚未初始化");
                
            return _container.GetExportedValues<T>();
        }
        
        /// <summary>
        /// 获取指定类型的服务实例
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="key">服务键（暂未使用）</param>
        /// <returns>服务实例</returns>
        public static object GetInstance(Type serviceType, string key)
        {
            if (_container == null)
                throw new InvalidOperationException("IoC容器尚未初始化");
                
            return _container.GetExportedValue<object>(serviceType.FullName);
        }
    }
}