using System;
using System.ComponentModel.Composition;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AuroraUI.Framework.Logging
{
    /// <summary>
    /// 使用 Microsoft.Extensions.Logging 的日志实现
    /// </summary>
    [Export(typeof(ILogger))]
    public class Logger : ILogger
    {
        private Microsoft.Extensions.Logging.ILogger _microsoftLogger;
        private readonly string _categoryName;
        
        /// <summary>
        /// 静态的日志工厂，用于创建 Microsoft.Extensions.Logging.ILogger 实例
        /// </summary>
        public static ILoggerFactory LoggerFactory { get; set; } = NullLoggerFactory.Instance;
        
        public Logger() : this("AuroraUI")
        {
        }
        
        public Logger(string categoryName)
        {
            _categoryName = categoryName;
            _microsoftLogger = LoggerFactory.CreateLogger(categoryName);
        }
        
        /// <summary>
        /// 设置日志工厂
        /// </summary>
        /// <param name="loggerFactory">日志工厂</param>
        public static void SetLoggerFactory(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }
        
        /// <summary>
        /// 为指定类别创建日志记录器
        /// </summary>
        /// <param name="categoryName">类别名称</param>
        /// <returns>日志记录器实例</returns>
        public static ILogger CreateLogger(string categoryName)
        {
            return new Logger(categoryName);
        }
        
        public void Debug(string message, params object[] args)
        {
            if (args.Length > 0)
            {
                _microsoftLogger.LogDebug(message, args);
            }
            else
            {
                _microsoftLogger.LogDebug(message);
            }
        }
        
        public void Info(string message, params object[] args)
        {
            if (args.Length > 0)
            {
                _microsoftLogger.LogInformation(message, args);
            }
            else
            {
                _microsoftLogger.LogInformation(message);
            }
        }
        
        public void Warning(string message, params object[] args)
        {
            if (args.Length > 0)
            {
                _microsoftLogger.LogWarning(message, args);
            }
            else
            {
                _microsoftLogger.LogWarning(message);
            }
        }
        
        public void Error(string message, params object[] args)
        {
            if (args.Length > 0)
            {
                _microsoftLogger.LogError(message, args);
            }
            else
            {
                _microsoftLogger.LogError(message);
            }
        }
        
        public void Error(Exception exception, string message, params object[] args)
        {
            if (args.Length > 0)
            {
                _microsoftLogger.LogError(exception, message, args);
            }
            else
            {
                _microsoftLogger.LogError(exception, message);
            }
        }
    }
}