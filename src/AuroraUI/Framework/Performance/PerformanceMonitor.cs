using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using AuroraUI.Framework.Logging;

namespace AuroraUI.Framework.Performance
{
    /// <summary>
    /// 性能监控器，用于测量应用程序各部分的性能
    /// </summary>
    public static class PerformanceMonitor
    {
        private static ILogger? _logger;
        private static readonly ConcurrentDictionary<string, Stopwatch> _timers = new();
        private static readonly ConcurrentDictionary<string, TimeSpan> _results = new();
        
        /// <summary>
        /// 获取Logger实例，支持延迟初始化
        /// </summary>
        private static ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    try
                    {
                        _logger = LogManager.GetLogger();
                    }
                    catch
                    {
                        // LogManager未初始化时返回null，方法中会检查
                        return null!;
                    }
                }
                return _logger;
            }
        }
        
        /// <summary>
        /// 开始计时
        /// </summary>
        /// <param name="name">计时器名称</param>
        public static void StartTimer(string name)
        {
            var stopwatch = Stopwatch.StartNew();
            _timers.AddOrUpdate(name, stopwatch, (key, oldValue) =>
            {
                oldValue.Stop();
                return stopwatch;
            });
            
            Logger?.Debug($"性能监控：开始计时 - {name}");
        }
        
        /// <summary>
        /// 停止计时并记录结果
        /// </summary>
        /// <param name="name">计时器名称</param>
        /// <returns>耗时</returns>
        public static TimeSpan StopTimer(string name)
        {
            if (_timers.TryRemove(name, out var stopwatch))
            {
                stopwatch.Stop();
                var elapsed = stopwatch.Elapsed;
                _results.AddOrUpdate(name, elapsed, (key, oldValue) => elapsed);
                
                Logger?.Info($"性能监控：{name} 耗时 {elapsed.TotalMilliseconds:F2} ms");
                return elapsed;
            }
            
            Logger?.Warning($"性能监控：未找到计时器 - {name}");
            return TimeSpan.Zero;
        }
        
        /// <summary>
        /// 获取计时结果
        /// </summary>
        /// <param name="name">计时器名称</param>
        /// <returns>耗时，如果不存在返回零</returns>
        public static TimeSpan GetResult(string name)
        {
            return _results.TryGetValue(name, out var result) ? result : TimeSpan.Zero;
        }
        
        /// <summary>
        /// 测量操作耗时
        /// </summary>
        /// <param name="name">操作名称</param>
        /// <param name="action">要测量的操作</param>
        /// <returns>耗时</returns>
        public static TimeSpan Measure(string name, Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                action();
            }
            finally
            {
                stopwatch.Stop();
                var elapsed = stopwatch.Elapsed;
                _results.AddOrUpdate(name, elapsed, (key, oldValue) => elapsed);
                Logger?.Info($"性能监控：{name} 耗时 {elapsed.TotalMilliseconds:F2} ms");
            }
            
            return stopwatch.Elapsed;
        }
        
        /// <summary>
        /// 异步测量操作耗时
        /// </summary>
        /// <param name="name">操作名称</param>
        /// <param name="func">要测量的异步操作</param>
        /// <returns>耗时</returns>
        public static async System.Threading.Tasks.Task<TimeSpan> MeasureAsync(string name, Func<System.Threading.Tasks.Task> func)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                await func();
            }
            finally
            {
                stopwatch.Stop();
                var elapsed = stopwatch.Elapsed;
                _results.AddOrUpdate(name, elapsed, (key, oldValue) => elapsed);
                Logger?.Info($"性能监控：{name} 耗时 {elapsed.TotalMilliseconds:F2} ms");
            }
            
            return stopwatch.Elapsed;
        }
        
        /// <summary>
        /// 清除所有计时结果
        /// </summary>
        public static void Clear()
        {
            _timers.Clear();
            _results.Clear();
            Logger?.Debug("性能监控：已清除所有计时结果");
        }
        
        /// <summary>
        /// 输出性能摘要报告
        /// </summary>
        public static void LogSummary()
        {
            if (Logger != null)
            {
                Logger.Info("=== 性能监控摘要报告 ===");
                
                foreach (var result in _results)
                {
                    Logger.Info($"  {result.Key}: {result.Value.TotalMilliseconds:F2} ms");
                }
                
                Logger.Info("========================");
            }
            else
            {
                // 如果Logger不可用，使用Console输出
                Console.WriteLine("=== 性能监控摘要报告 ===");
                
                foreach (var result in _results)
                {
                    Console.WriteLine($"  {result.Key}: {result.Value.TotalMilliseconds:F2} ms");
                }
                
                Console.WriteLine("========================");
            }
        }
    }
}
