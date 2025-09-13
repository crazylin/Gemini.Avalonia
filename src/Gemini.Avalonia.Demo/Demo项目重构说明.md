# Gemini.Avalonia.Demo 项目重构说明

## 概述

Demo项目已经成功重构，完全集成了核心框架的新按需加载架构。本重构消除了冗余代码，优化了性能，并展示了如何正确使用新的模块管理系统。

## 新架构特性

### 1. DemoBootstrapper 引导器
- **位置**: `Framework/DemoBootstrapper.cs`
- **功能**: 继承自核心 `AppBootstrapper`，专门为Demo应用程序设计
- **特点**:
  - 自动注册Demo特有的模块配置
  - 集成性能监控系统
  - 支持按需加载Demo功能模块
  - 提供详细的日志记录

### 2. 模块配置系统
- **位置**: `Framework/DemoModuleConfiguration.cs`
- **功能**: 集中管理Demo应用程序的所有模块配置
- **包含模块**:
  - 所有核心框架模块（MainMenu、StatusBar、Theme等）
  - `SampleDocumentModule` - Demo特有的文档模块

### 3. 懒加载文档模块
- **位置**: `Modules/SampleDocumentModule.cs`
- **改进**:
  - 继承自 `LazyModuleBase` 而非 `ModuleBase`
  - 实现 `CreateMetadata()` 定义模块元数据
  - 实现 `ShouldLoad()` 控制加载条件
  - 支持按需加载和卸载

### 4. Demo专用命令系统
- **位置**: `Commands/DemoCommands.cs`
- **包含命令**:
  - `LoadSampleDocumentModuleCommand` - 按需加载示例文档模块
  - `LoadAllDemoFeaturesCommand` - 加载所有Demo功能模块
  - `ShowModuleStatusCommand` - 显示模块状态信息

### 5. Demo菜单系统
- **位置**: `Menus/DemoMenuDefinitions.cs`
- **功能**:
  - 在主菜单栏中添加"Demo"菜单
  - 提供模块管理和状态查看功能
  - 展示按需加载能力

## 性能优化

### 启动性能
- **分阶段加载**: 核心模块 → UI模块 → 功能模块
- **性能监控**: 实时监控各阶段加载时间
- **懒加载**: Demo功能模块仅在需要时加载

### 内存优化
- **按需实例化**: 模块实例仅在加载时创建
- **依赖管理**: 智能解析和管理模块依赖关系
- **资源清理**: 支持模块卸载和资源释放

## 使用方法

### 启动应用程序
```csharp
// App.axaml.cs 中的新启动方式
var bootstrapper = new DemoBootstrapper();
bootstrapper.Initialize();
var mainWindow = await bootstrapper.StartAsync();
```

### 按需加载模块
通过Demo菜单或编程方式：
```csharp
// 加载特定模块
await bootstrapper.LoadDemoFeatureAsync("SampleDocumentModule");

// 加载所有功能模块
await bootstrapper.LoadAllDemoFeaturesAsync();
```

### 查看模块状态
使用 "Demo → 显示模块状态" 菜单项查看：
- 已注册模块数量
- 已加载模块列表
- 模块类别和优先级信息

## 代码结构

```
src/Gemini.Avalonia.Demo/
├── Framework/
│   ├── DemoBootstrapper.cs          # Demo应用引导器
│   └── DemoModuleConfiguration.cs   # 模块配置管理
├── Commands/
│   └── DemoCommands.cs              # Demo专用命令
├── Menus/
│   └── DemoMenuDefinitions.cs       # Demo菜单定义
├── Modules/
│   └── SampleDocumentModule.cs      # 懒加载文档模块
└── App.axaml.cs                     # 应用程序入口（已优化）
```

## 主要改进点

### 1. 消除冗余代码
- 移除了老式的 `.AddModule<T>()` 调用链
- 统一使用模块配置系统管理所有模块
- 简化了应用程序启动逻辑

### 2. 按需加载实现
- `SampleDocumentModule` 现在支持懒加载
- 提供运行时模块加载控制
- 实现了真正的按需加载机制

### 3. 性能监控集成
- 完整的启动时间监控
- 模块加载性能追踪
- 详细的性能报告输出

### 4. 更好的架构设计
- 清晰的职责分离
- 可扩展的模块系统
- 标准化的配置管理

## 运行和测试

### 编译项目
```bash
dotnet build src/Gemini.Avalonia.Demo/Gemini.Avalonia.Demo.csproj
```

### 运行Demo
```bash
dotnet run --project src/Gemini.Avalonia.Demo/Gemini.Avalonia.Demo.csproj
```

### 性能测试
启动应用程序后，检查控制台输出的性能监控报告，查看各个阶段的加载时间。

## 兼容性

- ✅ 完全兼容新的核心框架架构
- ✅ 保持所有原有功能正常工作
- ✅ 支持现有的MEF依赖注入系统
- ✅ 向后兼容传统模块（如果需要）

## 扩展指南

### 添加新的Demo模块
1. 创建继承自 `LazyModuleBase` 的模块类
2. 在 `DemoModuleConfiguration.cs` 中注册模块配置
3. 根据需要添加相应的命令和菜单项

### 自定义加载策略
通过重写 `ShouldLoad()` 方法实现自定义的模块加载条件，例如：
- 基于用户配置
- 基于许可证状态
- 基于运行环境

## 总结

Demo项目的重构成功展示了新架构的优势：
- **更快的启动速度**: 通过按需加载减少初始化时间
- **更好的用户体验**: 渐进式功能加载
- **更清晰的代码结构**: 标准化的模块管理
- **更好的可维护性**: 集中化的配置管理

这个重构为整个Gemini.Avalonia框架的使用提供了最佳实践参考。
