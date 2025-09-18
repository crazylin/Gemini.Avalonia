using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuroraUI.Framework.Logging;

namespace AuroraUI.Framework.Modules
{
    /// <summary>
    /// 模块加载事件类型
    /// </summary>
    public enum ModuleLoadingEventType
    {
        /// <summary>
        /// 模块注册
        /// </summary>
        ModuleRegistered,
        
        /// <summary>
        /// 模块开始加载
        /// </summary>
        ModuleLoadStarted,
        
        /// <summary>
        /// 模块加载成功
        /// </summary>
        ModuleLoadSucceeded,
        
        /// <summary>
        /// 模块加载失败
        /// </summary>
        ModuleLoadFailed,
        
        /// <summary>
        /// 模块开始卸载
        /// </summary>
        ModuleUnloadStarted,
        
        /// <summary>
        /// 模块卸载成功
        /// </summary>
        ModuleUnloadSucceeded,
        
        /// <summary>
        /// 模块卸载失败
        /// </summary>
        ModuleUnloadFailed
    }
    
    /// <summary>
    /// 模块加载事件参数
    /// </summary>
    public class ModuleLoadingEventArgs : EventArgs
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public ModuleLoadingEventType EventType { get; }
        
        /// <summary>
        /// 模块元数据
        /// </summary>
        public ModuleMetadata ModuleMetadata { get; }
        
        /// <summary>
        /// 异常信息（如果有）
        /// </summary>
        public Exception? Exception { get; }
        
        /// <summary>
        /// 事件发生时间
        /// </summary>
        public DateTime Timestamp { get; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="moduleMetadata">模块元数据</param>
        /// <param name="exception">异常信息</param>
        public ModuleLoadingEventArgs(ModuleLoadingEventType eventType, ModuleMetadata moduleMetadata, Exception? exception = null)
        {
            EventType = eventType;
            ModuleMetadata = moduleMetadata;
            Exception = exception;
            Timestamp = DateTime.Now;
        }
    }
    
    /// <summary>
    /// 模块加载事件监控器，提供统一的事件管理和监控功能
    /// </summary>
    public class ModuleLoadingEventMonitor
    {
        private readonly List<IModuleLoadingEventHandler> _eventHandlers = new();
        
        /// <summary>
        /// 模块加载事件
        /// </summary>
        public event EventHandler<ModuleLoadingEventArgs>? ModuleLoadingEvent;
        
        /// <summary>
        /// 注册事件处理器
        /// </summary>
        /// <param name="handler">事件处理器</param>
        public void RegisterEventHandler(IModuleLoadingEventHandler handler)
        {
            if (handler != null && !_eventHandlers.Contains(handler))
            {
                _eventHandlers.Add(handler);
                LogManager.Debug("ModuleLoadingEventMonitor", $"已注册事件处理器: {handler.GetType().Name}");
            }
        }
        
        /// <summary>
        /// 移除事件处理器
        /// </summary>
        /// <param name="handler">事件处理器</param>
        /// <returns>是否成功移除</returns>
        public bool UnregisterEventHandler(IModuleLoadingEventHandler handler)
        {
            var removed = _eventHandlers.Remove(handler);
            if (removed)
            {
                LogManager.Debug("ModuleLoadingEventMonitor", $"已移除事件处理器: {handler.GetType().Name}");
            }
            return removed;
        }
        
        /// <summary>
        /// 触发模块加载事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="moduleMetadata">模块元数据</param>
        /// <param name="exception">异常信息</param>
        public async Task TriggerEventAsync(ModuleLoadingEventType eventType, ModuleMetadata moduleMetadata, Exception? exception = null)
        {
            var eventArgs = new ModuleLoadingEventArgs(eventType, moduleMetadata, exception);
            
            // 记录日志
            LogEvent(eventArgs);
            
            // 触发事件
            ModuleLoadingEvent?.Invoke(this, eventArgs);
            
            // 通知所有注册的事件处理器
            await NotifyEventHandlersAsync(eventArgs);
        }
        
        /// <summary>
        /// 通知所有事件处理器
        /// </summary>
        /// <param name="eventArgs">事件参数</param>
        private async Task NotifyEventHandlersAsync(ModuleLoadingEventArgs eventArgs)
        {
            foreach (var handler in _eventHandlers)
            {
                try
                {
                    await handler.HandleEventAsync(eventArgs);
                }
                catch (Exception ex)
                {
                    LogManager.Error("ModuleLoadingEventMonitor", 
                        $"事件处理器 {handler.GetType().Name} 处理事件时出错: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// 记录事件日志
        /// </summary>
        /// <param name="eventArgs">事件参数</param>
        private void LogEvent(ModuleLoadingEventArgs eventArgs)
        {
            var message = $"模块 {eventArgs.ModuleMetadata.Name} - {GetEventDescription(eventArgs.EventType)}";
            
            switch (eventArgs.EventType)
            {
                case ModuleLoadingEventType.ModuleRegistered:
                case ModuleLoadingEventType.ModuleLoadStarted:
                case ModuleLoadingEventType.ModuleLoadSucceeded:
                case ModuleLoadingEventType.ModuleUnloadStarted:
                case ModuleLoadingEventType.ModuleUnloadSucceeded:
                    LogManager.Info("ModuleLoadingEventMonitor", message);
                    break;
                    
                case ModuleLoadingEventType.ModuleLoadFailed:
                case ModuleLoadingEventType.ModuleUnloadFailed:
                    var errorMessage = eventArgs.Exception != null 
                        ? $"{message} - 错误: {eventArgs.Exception.Message}"
                        : message;
                    LogManager.Error("ModuleLoadingEventMonitor", errorMessage);
                    break;
            }
        }
        
        /// <summary>
        /// 获取事件描述
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>事件描述</returns>
        private string GetEventDescription(ModuleLoadingEventType eventType)
        {
            return eventType switch
            {
                ModuleLoadingEventType.ModuleRegistered => "已注册",
                ModuleLoadingEventType.ModuleLoadStarted => "开始加载",
                ModuleLoadingEventType.ModuleLoadSucceeded => "加载成功",
                ModuleLoadingEventType.ModuleLoadFailed => "加载失败",
                ModuleLoadingEventType.ModuleUnloadStarted => "开始卸载",
                ModuleLoadingEventType.ModuleUnloadSucceeded => "卸载成功",
                ModuleLoadingEventType.ModuleUnloadFailed => "卸载失败",
                _ => "未知事件"
            };
        }
    }
    
    /// <summary>
    /// 模块加载事件处理器接口
    /// </summary>
    public interface IModuleLoadingEventHandler
    {
        /// <summary>
        /// 处理模块加载事件
        /// </summary>
        /// <param name="eventArgs">事件参数</param>
        /// <returns>处理任务</returns>
        Task HandleEventAsync(ModuleLoadingEventArgs eventArgs);
    }
}
