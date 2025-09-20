# AuroraUI 主题系统

AuroraUI 框架提供了一个强大而灵活的主题系统，支持多种预设主题以及自定义主题开发。

## 🎨 可用主题

### 系统主题
- **浅色主题** (Light) - 标准浅色主题
- **深色主题** (Dark) - 标准深色主题  
- **跟随系统** (System) - 自动跟随操作系统设置

### 基础主题
- **现代蓝色** (ModernBlue) - 专业、清新的蓝色主题，适合长时间工作
- **专业深色** (DarkProfessional) - 经典深色主题，减少眼部疲劳，适合夜间工作
- **自然绿色** (NatureGreen) - 清新自然的绿色主题，营造宁静专注的工作环境

### 专业主题
- **深海蓝色** (OceanBlue) - 深邃海洋主题，沉稳专业，适合长期专注工作
- **企业金色** (CorporateGold) - 高端企业风格，金色点缀，体现专业与品质
- **医疗洁净** (MedicalClean) - 医疗级洁净主题，简洁可靠，适合精密仪器操作界面

### 特色主题
- **赛博朋克** (CyberpunkNeon) - 未来科技风格，霓虹色彩，适合创新型工作环境
- **深林主题** (ForestDark) - 深色森林主题，自然沉静，适合需要专注的深度工作
- **复古终端** (RetroTerminal) - 经典终端风格，绿色磷光屏效果，怀旧极客风格

### 高级主题
- **皇家紫色** (RoyalPurple) - 高贵典雅的紫色主题，彰显品味与格调
- **夕阳橙色** (SunsetOrange) - 温暖活力的橙色主题，激发创造力和热情
- **极地白色** (ArcticWhite) - 极简纯净的白色主题，最大化内容可读性

### 无障碍主题
- **高对比度** (HighContrast) - 高对比度主题，提高可读性，适合视力敏感用户
- **极简灰色** (MinimalGrey) - 简约优雅的灰色主题，专注内容，减少干扰

## 🛠️ 如何使用

### 在设置中切换主题

1. 打开应用程序
2. 进入 **设置** > **应用程序设置**
3. 在 **主题设置** 部分：
   - 选择主题分类
   - 点击喜欢的主题卡片
   - 点击 **应用** 按钮保存更改

### 程序化切换主题

```csharp
// 获取主题服务
var themeService = IoC.Get<IThemeService>();

// 切换到现代蓝色主题
themeService.ChangeTheme(ThemeType.ModernBlue);

// 获取主题管理器
var themeManager = IoC.Get<IThemeManager>();

// 获取主题信息
var themeInfo = themeManager.GetThemeInfo(ThemeType.CyberpunkNeon);
Console.WriteLine($"主题名称: {themeInfo.Name}");
Console.WriteLine($"主题描述: {themeInfo.Description}");
```

## 🔧 自定义主题开发

### 1. 创建主题枚举值

在 `ThemeType.cs` 中添加新的主题类型：

```csharp
/// <summary>
/// 我的自定义主题
/// </summary>
MyCustomTheme
```

### 2. 创建主题资源文件

在 `AuroraUI/Modules/Theme/Resources/Extended/` 目录下创建主题文件夹：

```
MyCustomTheme/
├── Colors.axaml      # 颜色定义
└── Theme.axaml       # 主题文件
```

#### Colors.axaml 示例：

```xml
<ResourceDictionary xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <!-- 颜色定义 -->
  <Color x:Key="PrimaryColor">#FF6B35</Color>
  <Color x:Key="SecondaryColor">#F7931E</Color>
  <Color x:Key="BackgroundColor">#FAFAFA</Color>
  <!-- 更多颜色... -->
  
  <!-- 画刷定义 -->
  <SolidColorBrush x:Key="PrimaryColorBrush" Color="{StaticResource PrimaryColor}"/>
  <!-- 更多画刷... -->
</ResourceDictionary>
```

#### Theme.axaml 示例：

```xml
<ResourceDictionary xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <!-- 合并基础主题和颜色 -->
  <ResourceDictionary.MergedDictionaries>
    <ResourceInclude Source="avares://AuroraUI/Modules/Theme/Resources/LightTheme.axaml"/>
    <ResourceInclude Source="avares://AuroraUI/Modules/Theme/Resources/Extended/MyCustomTheme/Colors.axaml"/>
  </ResourceDictionary.MergedDictionaries>

  <!-- 自定义样式覆盖 -->
  <SolidColorBrush x:Key="SystemAccentColor" Color="{StaticResource PrimaryColor}"/>
  <!-- 更多覆盖样式... -->
</ResourceDictionary>
```

### 3. 在 ThemeManager 中注册主题

在 `ThemeManager.cs` 的 `InitializeThemes()` 方法中添加：

```csharp
_themes[ThemeType.MyCustomTheme] = new ThemeInfo
{
    Type = ThemeType.MyCustomTheme,
    Name = "我的自定义主题",
    Description = "这是一个自定义主题的描述",
    Category = ThemeCategory.Special,
    Icon = "🎨",
    PreviewColor = "#FF6B35",
    IsDark = false,
    ResourcePath = "avares://AuroraUI/Modules/Theme/Resources/Extended/MyCustomTheme/Theme.axaml"
};
```

## 📁 文件结构

```
AuroraUI/
└── Modules/
    └── Theme/
        ├── Models/
        │   ├── ThemeType.cs        # 主题类型枚举
        │   └── ThemeInfo.cs        # 主题信息模型
        ├── Services/
        │   ├── IThemeManager.cs    # 主题管理器接口
        │   ├── ThemeManager.cs     # 主题管理器实现
        │   ├── IThemeService.cs    # 主题服务接口
        │   └── ThemeService.cs     # 主题服务实现
        └── Resources/
            ├── LightTheme.axaml    # 基础浅色主题
            ├── DarkTheme.axaml     # 基础深色主题
            └── Extended/           # 扩展主题目录
                ├── ModernBlue/
                ├── DarkProfessional/
                ├── NatureGreen/
                └── ...
```

## 🎯 最佳实践

### 主题设计原则
1. **一致性** - 保持颜色搭配和视觉风格的一致性
2. **可读性** - 确保文本在所有背景上都清晰可读
3. **可访问性** - 考虑色盲用户和视力障碍用户的需求
4. **品牌性** - 保持与应用程序整体品牌形象的协调

### 颜色搭配建议
- **主色调**：选择一个主要颜色作为品牌色
- **辅助色**：选择1-2个辅助颜色用于强调和区分
- **中性色**：使用灰色系列用于文本和背景
- **功能色**：定义成功、警告、错误、信息等状态色

### 主题测试
1. 在不同光照条件下测试主题
2. 测试所有UI控件的显示效果
3. 验证文本的可读性
4. 检查颜色对比度是否符合无障碍标准

## 🔍 故障排除

### 常见问题

**Q: 主题切换后没有生效？**
A: 检查资源文件路径是否正确，确保 AXAML 文件被正确编译到程序集中。

**Q: 自定义主题颜色显示不正确？**
A: 确保颜色值格式正确（#RRGGBB 或 #AARRGGBB），并且在 Theme.axaml 中正确引用了 Colors.axaml。

**Q: 主题在设置界面中不显示？**
A: 检查是否在 ThemeManager 中正确注册了主题，并且 ThemeInfo 的所有属性都已正确设置。

### 调试技巧
1. 使用 Avalonia DevTools 检查运行时的资源状态
2. 在应用程序输出中查看主题相关的日志信息
3. 验证主题文件的 Build Action 设置为 "AvaloniaResource"

## 📝 更新日志

### v1.0.0
- ✨ 首次发布主题系统
- 🎨 提供 14 种预设主题
- 🛠️ 支持主题分类和管理
- 📋 完整的设置界面
- 🔧 支持自定义主题开发

---

更多信息请参考 [AuroraUI 官方文档](https://github.com/your-org/AuroraUI)。
