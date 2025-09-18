using System;
using Microsoft.Extensions.Logging;

namespace AuroraUI.Framework.Logging
{
    /// <summary>
    /// 日志管理器，提供静态访问日志功能
    /// </summary>
    public static class LogManager
    {
        private static ILogger _logger;
        private static ILoggerFactory _loggerFactory;
        
        /// <summary>
        /// 初始化日志管理器
        /// </summary>
        /// <param name="logger">日志实例</param>
        public static void Initialize(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        /// <summary>
        /// 使用 Microsoft.Extensions.Logging 初始化日志管理器
        /// </summary>
        /// <param name="loggerFactory">日志工厂</param>
        public static void Initialize(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            Logger.SetLoggerFactory(loggerFactory);
            _logger = new Logger("AuroraUI.Default");
        }
        
        /// <summary>
        /// 获取日志实例
        /// </summary>
        /// <returns>日志实例</returns>
        public static ILogger GetLogger()
        {
            return _logger ?? throw new InvalidOperationException("日志管理器尚未初始化，请先调用Initialize方法");
        }
        
        /// <summary>
        /// 获取指定类别的日志实例
        /// </summary>
        /// <param name="categoryName">类别名称</param>
        /// <returns>日志实例</returns>
        public static ILogger GetLogger(string categoryName)
        {
            if (_loggerFactory == null)
                throw new InvalidOperationException("日志管理器尚未使用LoggerFactory初始化");
            
            return Logger.CreateLogger(categoryName);
        }
        
        /// <summary>
        /// 获取指定类型的日志实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>日志实例</returns>
        public static ILogger GetLogger<T>()
        {
            return GetLogger(typeof(T).FullName);
        }
        
        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void Debug(string message, params object[] args)
        {
            GetLogger().Debug(message, args);
        }
        
        /// <summary>
        /// 记录调试信息（带类别）
        /// </summary>
        /// <param name="category">类别</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void Debug(string category, string message, params object[] args)
        {
            var logger = GetLogger(category);
            logger.Debug(message, args);
        }
        
        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void Info(string message, params object[] args)
        {
            GetLogger().Info(message, args);
        }
        
        /// <summary>
        /// 记录信息（带类别）
        /// </summary>
        /// <param name="category">类别</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void Info(string category, string message, params object[] args)
        {
            var logger = GetLogger(category);
            logger.Info(message, args);
        }
        
        /// <summary>
        /// 记录警告
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void Warning(string message, params object[] args)
        {
            GetLogger().Warning(message, args);
        }
        
        /// <summary>
        /// 记录警告（带类别）
        /// </summary>
        /// <param name="category">类别</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void Warning(string category, string message, params object[] args)
        {
            var logger = GetLogger(category);
            logger.Warning(message, args);
        }
        
        /// <summary>
        /// 记录错误
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void Error(string message, params object[] args)
        {
            GetLogger().Error(message, args);
        }
        
        /// <summary>
        /// 记录错误（带类别）
        /// </summary>
        /// <param name="category">类别</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void Error(string category, string message, params object[] args)
        {
            var logger = GetLogger(category);
            logger.Error(message, args);
        }
        
        /// <summary>
        /// 记录错误（包含异常）
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void Error(Exception exception, string message, params object[] args)
        {
            GetLogger().Error(exception, message, args);
        }
        
        /// <summary>
        /// 记录错误（包含异常和类别）
        /// </summary>
        /// <param name="category">类别</param>
        /// <param name="exception">异常对象</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void Error(string category, Exception exception, string message, params object[] args)
        {
            var logger = GetLogger(category);
            logger.Error(exception, message, args);
        }
    }
}