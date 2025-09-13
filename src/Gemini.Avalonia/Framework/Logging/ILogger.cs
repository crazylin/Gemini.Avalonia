using System;

namespace Gemini.Avalonia.Framework.Logging
{
    /// <summary>
    /// 日志接口
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        void Debug(string message, params object[] args);
        
        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        void Info(string message, params object[] args);
        
        /// <summary>
        /// 记录警告
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        void Warning(string message, params object[] args);
        
        /// <summary>
        /// 记录错误
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        void Error(string message, params object[] args);
        
        /// <summary>
        /// 记录错误（包含异常）
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        void Error(Exception exception, string message, params object[] args);
    }
}