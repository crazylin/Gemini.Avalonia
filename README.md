# Gemini Avalonia Framework

一个基于Avalonia UI的模块化应用程序框架，灵感来源于原始的Gemini WPF框架。

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
│   │   │   └── ToolBars/          # 工具栏模块
│   │   └── Views/                 # 核心视图
│   └── Gemini.Avalonia.Demo/      # 演示应用程序
│       ├── Modules/               # 演示模块
│       ├── ViewModels/            # 视图模型
│       ├── Views/                 # 视图
│       └── Converters/            # 转换器
└── README.md
```

## 核心特性

### 🏗️ 模块化架构
- 基于依赖注入的模块系统
- 可插拔的模块设计
- 松耦合的组件架构

### 📄 文档系统
- 支持多文档界面(MDI)
- 文档生命周期管理
- 脏状态跟踪

### 🔧 工具系统
- 可停靠的工具窗口
- 灵活的布局管理
- 工具状态持久化

### 🎨 用户界面
- 基于Avalonia UI的现代界面
- 响应式设计
- 主题支持

## 快速开始

### 前置要求
- .NET 8.0 或更高版本
- Visual Studio 2022 或 JetBrains Rider

### 构建项目

```bash
# 克隆项目
git clone <repository-url>
cd 框架22

# 还原依赖
dotnet restore

# 构建项目
dotnet build

# 运行演示应用
dotnet run --project src/Gemini.Avalonia.Demo
```

## 核心概念

### 文档(Documents)
文档是应用程序中的主要内容区域，继承自`Document`基类：

```csharp
public class MyDocument : Document
{
    public override string DisplayName => "我的文档";
    // 实现文档逻辑
}
```

### 工具(Tools)
工具是可停靠的面板，继承自`Tool`基类：

```csharp
public class MyTool : Tool
{
    public override string DisplayName => "我的工具";
    public override PaneLocation PreferredLocation => PaneLocation.Right;
    // 实现工具逻辑
}
```

### 模块(Modules)
模块用于组织和初始化应用程序功能：

```csharp
[Export(typeof(IModule))]
public class MyModule : ModuleBase
{
    public override void Initialize()
    {
        // 初始化模块
    }
}
```

## 演示应用程序

演示应用程序包含以下功能：

- **示例文档**: 展示文档系统的使用
- **输出工具**: 显示应用程序输出信息
- **属性工具**: 显示选中对象的属性
- **主菜单**: 应用程序主菜单
- **状态栏**: 显示应用程序状态

## 技术栈

- **UI框架**: Avalonia UI 11.x
- **依赖注入**: Microsoft.Extensions.DependencyInjection
- **MVVM**: ReactiveUI
- **组合**: System.ComponentModel.Composition (MEF)

## 贡献

欢迎提交Issue和Pull Request来改进这个框架！

## 许可证

本项目采用MIT许可证。详见LICENSE文件。