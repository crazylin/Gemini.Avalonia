using System;

namespace Gemini.Avalonia.Framework.Logging
{
    /// <summary>
    /// 日志管理器，提供静态访问日志功能
    /// </summary>
    public static class LogManager
    {
        private static ILogger _logger;
        
        /// <summary>
        /// 初始化日志管理器
        /// </summary>
        /// <param name="logger">日志实例</param>
        public static void Initialize(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        /// 记录调试信息
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void Debug(string message, params object[] args)
        {
            GetLogger().Debug(message, args);
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
        /// 记录警告
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void Warning(string message, params object[] args)
        {
            GetLogger().Warning(message, args);
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
        /// 记录错误（包含异常）
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void Error(Exception exception, string message, params object[] args)
        {
            GetLogger().Error(exception, message, args);
        }
    }
}