using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;

namespace Gemini.Avalonia.Framework.Logging
{
    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }
    
    /// <summary>
    /// 默认日志实现
    /// </summary>
    [Export(typeof(ILogger))]
    public class Logger : ILogger
    {
        private static LogLevel _minLogLevel = LogLevel.Info;
        private static bool _enableConsoleOutput = false;
        
        /// <summary>
        /// 设置最小日志级别
        /// </summary>
        /// <param name="level">日志级别</param>
        public static void SetMinLogLevel(LogLevel level)
        {
            _minLogLevel = level;
        }
        
        /// <summary>
        /// 启用或禁用控制台输出
        /// </summary>
        /// <param name="enable">是否启用</param>
        public static void EnableConsoleOutput(bool enable)
        {
            _enableConsoleOutput = enable;
        }
        
        public void Debug(string message, params object[] args)
        {
            Log(LogLevel.Debug, message, args);
        }
        
        public void Info(string message, params object[] args)
        {
            Log(LogLevel.Info, message, args);
        }
        
        public void Warning(string message, params object[] args)
        {
            Log(LogLevel.Warning, message, args);
        }
        
        public void Error(string message, params object[] args)
        {
            Log(LogLevel.Error, message, args);
        }
        
        public void Error(Exception exception, string message, params object[] args)
        {
            var fullMessage = args.Length > 0 ? string.Format(message, args) : message;
            fullMessage += $" Exception: {exception.Message}";
            Log(LogLevel.Error, fullMessage);
        }
        
        private void Log(LogLevel level, string message, params object[] args)
        {
            if (level < _minLogLevel)
                return;
                
            var formattedMessage = args.Length > 0 ? $"{message} {string.Join(" ", args.Select(ar => ar.ToString()))}" : message;
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var logMessage = $"[{timestamp}] [{level}] {formattedMessage}";

            // 输出到调试器
            System.Diagnostics.Debug.WriteLine(logMessage);
            
            // 可选的控制台输出
            if (_enableConsoleOutput)
            {
                System.Console.WriteLine(logMessage);
            }
        }
    }
}