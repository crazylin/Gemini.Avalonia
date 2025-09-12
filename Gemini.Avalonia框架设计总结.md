# Gemini.Avalonia 框架设计总结

## 框架概述

Gemini.Avalonia 是一个基于 Avalonia UI 的**模块化应用程序框架**，灵感来源于 Visual Studio Shell 和原始的 Gemini WPF 框架。它提供了类似 IDE 的应用程序开发基础架构。

## 核心架构设计

### 🏗️ 模块化架构
- **模块接口**：`IModule.cs` 定义模块基本结构
- **模块基类**：`ModuleBase.cs` 提供默认实现
- **生命周期管理**：PreInitialize → Initialize → PostInitializeAsync
- **资源管理**：支持全局资源字典、默认文档和工具注册

### 🔧 依赖注入与服务定位
- **MEF容器**：使用 Microsoft Extensibility Framework 进行组件组合
- **服务提供者**：`MefServiceProvider.cs` 桥接 MEF 和 .NET DI
- **IoC适配器**：`IoC.cs` 提供服务定位功能

### 📋 应用程序引导
- **引导器**：`AppBootstrapper.cs` 负责框架初始化
- **初始化流程**：日志系统 → 语言配置 → MEF容器 → 模块加载 → Shell创建

## 核心组件系统

### 📄 文档系统
- **文档接口**：`IDocument.cs`
- **文档基类**：`Document.cs`
- **功能特性**：撤销重做、保存状态、布局持久化

### 🔧 工具系统
- **工具接口**：`ITool.cs`
- **工具基类**：`Tool.cs`
- **停靠支持**：基于 Dock.Avalonia 的可停靠工具窗口

### ⚡ 命令系统
- **命令服务**：`CommandService.cs`
- **命令定义**：声明式命令定义和处理器
- **键盘快捷键**：支持全局和上下文相关的快捷键

## UI 组织架构

### 🍔 菜单系统
- **菜单定义**：`MenuDefinitions.cs`
- **菜单构建器**：`MenuBuilder.cs`
- **层次结构**：MenuBar → Menu → MenuGroup → MenuItem

### 🔨 工具栏系统
- **工具栏定义**：`ToolBarDefinitions.cs`
- **动态构建**：支持运行时添加和移除工具栏项

### 🏠 Shell 系统
- **Shell接口**：`IShell.cs`
- **Shell实现**：`ShellViewModel.cs`
- **布局管理**：`DockFactory.cs`

## 核心模块

### 📁 项目管理模块
- **模块实现**：`ProjectManagement/Module.cs`
- **项目服务**：`IProjectService.cs`
- **功能特性**：项目创建、打开、保存、文件管理

### 📊 状态栏模块
- **状态管理**：实时显示应用程序状态信息
- **可扩展性**：支持自定义状态栏项

### 📤 输出模块
- **日志输出**：统一的日志和消息输出界面
- **多通道支持**：支持不同类型的输出通道

### ⚙️ 属性模块
- **属性编辑**：动态属性编辑器
- **类型支持**：支持多种数据类型的属性编辑

## 技术特性

### 🎨 现代化UI
- **Avalonia UI**：跨平台UI框架
- **ReactiveUI**：响应式MVVM模式
- **Fluent Design**：现代化的用户界面设计

### 🔄 响应式架构
- **数据绑定**：双向数据绑定支持
- **命令绑定**：声明式命令绑定
- **属性通知**：自动属性变更通知

### 🌐 国际化支持
- **多语言**：支持中文、英文等多种语言
- **资源管理**：`LanguageService.cs`
- **动态切换**：运行时语言切换

### 🔧 可扩展性
- **插件架构**：基于MEF的插件系统
- **热插拔**：支持运行时模块加载
- **API开放**：丰富的扩展点和API

## 设计模式应用

- **MVVM模式**：视图与业务逻辑分离
- **依赖注入**：松耦合的组件设计
- **命令模式**：统一的操作抽象
- **观察者模式**：事件驱动的架构
- **工厂模式**：对象创建的统一管理
- **服务定位器**：服务的统一访问入口

## 项目结构

```
框架22/
├── src/
│   ├── Gemini.Avalonia/           # 核心框架库
│   │   ├── Framework/             # 核心框架组件
│   │   │   ├── IDocument.cs       # 文档接口
│   │   │   ├── ITool.cs           # 工具接口
│   │   │   ├── ILayoutItem.cs     # 布局项接口
│   │   │   ├── IShell.cs          # Shell接口
│   │   │   ├── Document.cs        # 文档基类
│   │   │   ├── Tool.cs            # 工具基类
│   │   │   └── LayoutItemBase.cs  # 布局项基类
│   │   ├── Modules/               # 核心模块
│   │   │   ├── MainMenu/          # 主菜单模块
│   │   │   ├── Shell/             # Shell模块
│   │   │   ├── StatusBar/         # 状态栏模块
│   │   │   ├── ToolBars/          # 工具栏模块
│   │   │   ├── ProjectManagement/ # 项目管理模块
│   │   │   ├── Output/            # 输出模块
│   │   │   └── Properties/        # 属性模块
│   │   ├── Services/              # 核心服务
│   │   ├── Assets/                # 资源文件
│   │   └── Views/                 # 核心视图
│   └── Gemini.Avalonia.Demo/      # 演示应用程序
│       ├── Modules/               # 演示模块
│       ├── ViewModels/            # 视图模型
│       ├── Views/                 # 视图
│       └── Converters/            # 转换器
└── README.md
```

## 应用场景

适用于开发类似 Visual Studio、IntelliJ IDEA 等复杂桌面应用程序：
- **IDE开发工具**
- **数据分析平台**
- **设计工具软件**
- **企业管理系统**
- **科学计算应用**

## 核心优势

1. **模块化设计**：高度解耦的模块化架构，便于扩展和维护
2. **跨平台支持**：基于Avalonia UI，支持Windows、macOS、Linux
3. **现代化UI**：采用最新的UI设计理念和技术栈
4. **丰富的功能**：提供完整的IDE级别功能支持
5. **易于扩展**：基于MEF的插件系统，支持第三方扩展
6. **响应式架构**：基于ReactiveUI的响应式编程模型
7. **国际化支持**：完整的多语言支持
8. **专业级布局**：基于Dock.Avalonia的专业停靠布局系统

这个框架为开发者提供了一个强大、灵活且易于扩展的应用程序开发基础，大大降低了复杂桌面应用的开发难度。