# Gemini.Avalonia.SimpleDemo

这是一个精简版的 Gemini.Avalonia 框架演示项目，展示了框架的核心功能和模块化架构。

## 🎯 项目特点

### 核心功能演示
- **多文档系统**: 支持多个文档同时打开，每个文档都有独立的标签页
- **文档生命周期管理**: 自动跟踪文档修改状态，支持保存操作
- **模块化架构**: 使用 MEF 依赖注入框架实现组件的自动发现和注册
- **现代化界面**: 基于 Avalonia 的现代桌面应用程序界面

### 技术架构
- **框架**: Avalonia 11.3.2
- **依赖注入**: MEF (Managed Extensibility Framework)
- **模式**: MVVM (Model-View-ViewModel)
- **目标框架**: .NET 9.0

## 📁 项目结构

```
src/Gemini.Avalonia.SimpleDemo/
├── Documents/                  # 文档相关
│   └── SimpleDocument.cs      # 简单文档实现
├── Framework/                  # 框架核心
│   ├── SimpleDemoBootstrapper.cs  # 应用启动器
│   └── SimpleDemoModule.cs    # Demo模块
├── Views/                      # 视图文件
│   ├── SimpleDocumentView.axaml    # 文档视图
│   └── SimpleDocumentView.axaml.cs # 视图代码
├── App.axaml                   # 应用程序主题和样式
├── App.axaml.cs               # 应用程序启动逻辑
└── Program.cs                 # 程序入口点
```

## 🚀 运行项目

### 前提条件
- .NET 9.0 SDK 或更高版本
- 支持 Avalonia 的操作系统 (Windows, macOS, Linux)

### 编译和运行
```bash
# 编译项目
dotnet build src/Gemini.Avalonia.SimpleDemo/Gemini.Avalonia.SimpleDemo.csproj

# 运行项目
dotnet run --project src/Gemini.Avalonia.SimpleDemo/Gemini.Avalonia.SimpleDemo.csproj
```

## 📋 功能演示

### 文档系统
- **新建文档**: 启动时自动创建欢迎文档和示例文档
- **编辑文档**: 支持多行文本编辑，实时显示字符统计
- **修改状态**: 文档修改后标题显示 `*` 标记
- **标签页导航**: 可以在多个文档之间快速切换

### 用户界面
- **现代化设计**: 使用 Fluent 主题
- **响应式布局**: 界面元素自适应窗口大小
- **状态栏信息**: 显示文档统计信息（字符数、修改状态、文件路径）

## 🏗️ 架构特点

### 模块化设计
```csharp
[Export(typeof(IModule))]
public class SimpleDemoModule : ModuleBase
{
    // 模块自动被框架发现和加载
}
```

### 文档系统
```csharp
public class SimpleDocument : Document, INotifyPropertyChanged
{
    // 实现了文档的基本操作和属性变更通知
}
```

### 依赖注入
- 使用 MEF 实现组件的自动发现
- 支持松耦合的模块间通信
- 便于扩展和维护

## 🔧 扩展能力

这个简单demo只是开始，Gemini.Avalonia 框架支持扩展以下功能：

### 文档类型
- 代码编辑器（语法高亮）
- 富文本编辑器
- 图片查看器
- 数据表格编辑器
- 图表和可视化组件

### 界面组件
- 自定义菜单和工具栏
- 停靠窗口系统
- 状态栏扩展
- 主题和样式定制

### 功能模块
- 项目管理系统
- 插件系统
- 设置和配置管理
- 多语言支持

## 📖 代码示例

### 创建自定义文档类型
```csharp
public class MyCustomDocument : Document
{
    public string CustomProperty { get; set; }
    
    // 实现自定义的文档逻辑
}
```

### 添加新模块
```csharp
[Export(typeof(IModule))]
public class MyCustomModule : ModuleBase
{
    public override void Initialize()
    {
        // 模块初始化逻辑
    }
}
```

## 💡 学习价值

这个精简版demo是学习 Gemini.Avalonia 框架的最佳起点：

1. **架构理解**: 了解模块化桌面应用的基本架构
2. **技术实践**: 学习 MVVM、依赖注入、组件化开发
3. **界面设计**: 掌握现代桌面应用的界面设计原则
4. **扩展基础**: 为构建更复杂的应用奠定基础

## 🎈 开始探索

启动应用后，你会看到：
- 欢迎页面，介绍框架的主要特性
- 示例文档，可以直接编辑体验
- 简洁直观的用户界面

开始尝试编辑文档内容，观察标题栏的变化，体验这个现代化的桌面应用框架吧！

## 🔧 技术说明

### 日志系统
- 日志初始化已集成在基类 `AppBootstrapper` 中
- 支持控制台输出和不同日志级别
- Debug模式显示详细日志，Release模式仅显示重要信息

### 线程安全
- 所有UI操作都在UI线程上执行
- 异步启动确保界面响应性
- 完善的错误处理和异常显示

---

*这个demo展示了 Gemini.Avalonia 框架的核心能力，为构建更复杂的桌面应用程序提供了良好的起点。*
