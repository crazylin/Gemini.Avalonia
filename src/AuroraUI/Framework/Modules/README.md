# 模块加载系统重构说明

## 概述

本次重构对 Gemini.Avalonia 的模块加载系统进行了全面优化，解决了原有系统中代码重复、职责混乱、加载逻辑复杂等问题。

## 重构目标

1. **简化架构**：将复杂的模块加载逻辑分解为独立的组件
2. **提高可维护性**：清晰的职责分离和统一的接口设计
3. **增强扩展性**：通过策略模式支持不同类型的模块加载
4. **完善监控**：统一的事件系统提供全面的模块加载监控

## 新架构组件

### 1. 核心接口

- **`IModuleLoader`**: 模块加载器核心接口
- **`IModuleLoadingStrategy`**: 模块加载策略接口
- **`IModuleLoadingEventHandler`**: 模块加载事件处理器接口

### 2. 核心服务

- **`ModuleLoadingService`**: 统一的模块加载服务，负责协调所有模块操作
- **`ModuleDependencyResolver`**: 模块依赖解析器，处理复杂的依赖关系
- **`ModuleLoadingStrategyFactory`**: 策略工厂，管理不同的加载策略
- **`ModuleLoadingEventMonitor`**: 事件监控器，提供统一的事件管理

### 3. 加载策略

- **`StandardModuleLoadingStrategy`**: 标准模块加载策略
- **`LazyModuleLoadingStrategy`**: 延迟模块加载策略

### 4. 重构后的组件

- **`ModuleManager`**: 简化为轻量级门面，委托给 `ModuleLoadingService`
- **`AppBootstrapper`**: 简化模块加载逻辑，更好的向后兼容性

## 主要改进

### 1. 职责分离

- **ModuleLoadingService**: 负责模块的注册、加载、卸载
- **ModuleDependencyResolver**: 专门处理依赖关系解析
- **ModuleLoadingStrategyFactory**: 管理加载策略选择
- **ModuleLoadingEventMonitor**: 统一事件监控和日志记录

### 2. 策略模式

通过策略模式支持不同类型的模块加载：
- 标准模块：立即加载和初始化
- 延迟模块：按需加载，支持条件检查

### 3. 事件系统

全新的事件监控系统提供：
- 模块注册事件
- 模块加载开始/成功/失败事件
- 模块卸载开始/成功/失败事件
- 统一的日志记录和错误处理

### 4. 依赖管理

改进的依赖解析机制：
- 自动依赖顺序排序
- 循环依赖检测
- 传递依赖解析
- 依赖满足性检查

## 使用方式

### 基本模块注册和加载

```csharp
// 创建模块加载服务
var moduleLoadingService = new ModuleLoadingService();

// 注册模块
moduleLoadingService.RegisterModule<MyModule>();

// 加载特定模块
await moduleLoadingService.LoadModuleAsync("MyModule");

// 按类别加载模块
await moduleLoadingService.LoadModulesByCategoryAsync(ModuleCategory.Core);
```

### 自定义加载策略

```csharp
// 创建自定义加载策略
public class CustomModuleLoadingStrategy : IModuleLoadingStrategy
{
    public bool CanLoad(ModuleMetadata metadata) => /* 自定义条件 */;
    
    public async Task<IModule?> LoadAsync(ModuleMetadata metadata)
    {
        // 自定义加载逻辑
    }
    
    public async Task UnloadAsync(IModule module)
    {
        // 自定义卸载逻辑
    }
}

// 注册自定义策略
moduleLoadingService.RegisterLoadingStrategy(new CustomModuleLoadingStrategy());
```

### 事件监控

```csharp
// 注册事件处理器
public class MyEventHandler : IModuleLoadingEventHandler
{
    public async Task HandleEventAsync(ModuleLoadingEventArgs eventArgs)
    {
        // 处理模块加载事件
    }
}

moduleLoadingService.EventMonitor.RegisterEventHandler(new MyEventHandler());
```

## 向后兼容性

重构保持了与现有代码的完全兼容：
- `ModuleManager` 接口保持不变
- `AppBootstrapper` 的公开API保持不变
- 现有的模块定义无需修改
- 遗留模块通过专门的兼容性处理继续工作

## 性能优化

1. **异步加载**：所有模块加载操作都是异步的
2. **并发安全**：使用信号量确保模块加载的线程安全
3. **延迟加载**：支持模块的按需加载
4. **性能监控**：集成性能计时器，监控加载耗时

## 调试和诊断

1. **详细日志**：每个步骤都有详细的日志记录
2. **事件追踪**：通过事件系统追踪模块生命周期
3. **错误处理**：统一的异常处理和错误报告
4. **依赖分析**：依赖关系的可视化和验证

## 扩展点

1. **自定义加载策略**：实现 `IModuleLoadingStrategy` 接口
2. **事件处理器**：实现 `IModuleLoadingEventHandler` 接口  
3. **模块元数据**：扩展 `ModuleMetadata` 类
4. **依赖解析**：自定义依赖解析逻辑

这次重构大大提高了系统的可维护性、扩展性和可靠性，为未来的功能扩展打下了坚实的基础。
