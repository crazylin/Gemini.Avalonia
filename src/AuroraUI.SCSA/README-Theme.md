# SCSA 主题系统使用指南

## 概述

SCSA 应用的主题系统基于 AuroraUI 框架的主题模块构建，提供了 Light 和 Dark 两种主题模式，以及 SCSA 特定的颜色和样式扩展。

## 主题结构

```
Themes/
├── Light/
│   ├── Colors.axaml          # 浅色主题颜色定义
│   └── Theme.axaml           # 浅色主题主文件
├── Dark/
│   ├── Colors.axaml          # 深色主题颜色定义
│   └── Theme.axaml           # 深色主题主文件
└── Common/
    └── Controls.axaml        # 通用控件样式
```

## 主题特性

### 1. 继承框架主题
- 基于 AuroraUI 框架的主题系统
- 自动集成框架的浅色和深色主题
- 使用框架的 `IThemeService` 进行主题管理

### 2. SCSA 特定颜色
- **设备状态颜色**: 连接、断开、连接中
- **参数类型颜色**: 只读、读写、已修改
- **数据图表颜色**: 主色、辅色、强调色

### 3. 统一控件样式
- 所有常用控件的样式定义
- 一致的视觉风格和交互体验
- 响应式设计和动画效果

## 使用方法

### 1. 在 XAML 中使用样式

```xml
<!-- 使用 SCSA 主要按钮样式 -->
<Button Classes="SCSAPrimaryButton" Content="主要操作"/>

<!-- 使用 SCSA 次要按钮样式 -->
<Button Classes="SCSASecondaryButton" Content="次要操作"/>

<!-- 使用设备状态文本样式 -->
<TextBlock Classes="DeviceConnectedText" Text="设备已连接"/>
<TextBlock Classes="DeviceDisconnectedText" Text="设备已断开"/>

<!-- 使用参数类型文本样式 -->
<TextBlock Classes="ParameterReadOnlyText" Text="只读参数"/>
<TextBlock Classes="ParameterReadWriteText" Text="读写参数"/>
<TextBlock Classes="ParameterModifiedText" Text="已修改参数"/>
```

### 2. 在代码中切换主题

```csharp
// 获取主题服务
var themeService = IoC.Get<IThemeService>();

// 切换到深色主题
themeService.ChangeTheme(ThemeType.Dark);

// 切换到浅色主题
themeService.ChangeTheme(ThemeType.Light);

// 跟随系统主题
themeService.ChangeTheme(ThemeType.System);
```

### 3. 监听主题变更

```csharp
public class MyViewModel
{
    [Import]
    private IThemeService _themeService;
    
    public void Initialize()
    {
        _themeService.ThemeChanged += OnThemeChanged;
    }
    
    private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
    {
        // 主题变更处理逻辑
        Console.WriteLine($"主题从 {e.OldTheme} 切换到 {e.NewTheme}");
    }
}
```

## 颜色资源

### SCSA 专用颜色
- `SCSAPrimaryColor` / `SCSAPrimaryBrush` - 主色调
- `SCSASecondaryColor` / `SCSASecondaryBrush` - 辅助色
- `SCSASuccessColor` / `SCSASuccessBrush` - 成功色
- `SCSAWarningColor` / `SCSAWarningBrush` - 警告色
- `SCSAErrorColor` / `SCSAErrorBrush` - 错误色
- `SCSAInfoColor` / `SCSAInfoBrush` - 信息色

### 设备状态颜色
- `DeviceConnectedColor` / `DeviceConnectedBrush` - 设备已连接
- `DeviceDisconnectedColor` / `DeviceDisconnectedBrush` - 设备已断开
- `DeviceConnectingColor` / `DeviceConnectingBrush` - 设备连接中

### 参数类型颜色
- `ParameterReadOnlyColor` / `ParameterReadOnlyBrush` - 只读参数
- `ParameterReadWriteColor` / `ParameterReadWriteBrush` - 读写参数
- `ParameterModifiedColor` / `ParameterModifiedBrush` - 已修改参数

### 数据图表颜色
- `ChartPrimaryColor` / `ChartPrimaryBrush` - 图表主色
- `ChartSecondaryColor` / `ChartSecondaryBrush` - 图表辅色
- `ChartAccentColor` / `ChartAccentBrush` - 图表强调色

## 控件样式

### 按钮样式
- `SCSAPrimaryButton` - 主要按钮
- `SCSASecondaryButton` - 次要按钮

### 文本样式
- `SCSAHeadingText` - 标题文本
- `SCSASubheadingText` - 副标题文本
- `SCSABodyText` - 正文文本
- `SCSACaptionText` - 说明文本

### 状态样式
- `DeviceConnectedText` - 设备连接状态
- `DeviceDisconnectedText` - 设备断开状态
- `DeviceConnectingText` - 设备连接中状态
- `ParameterReadOnlyText` - 只读参数
- `ParameterReadWriteText` - 读写参数
- `ParameterModifiedText` - 已修改参数

### 容器样式
- `SCSACard` - 卡片容器
- `SCSADataGrid` - 数据表格
- `SCSAListBox` - 列表框

## 最佳实践

1. **一致性**: 在整个应用中使用统一的样式类
2. **语义化**: 根据功能和语义选择合适的样式
3. **响应式**: 样式会自动响应主题变更
4. **扩展性**: 可以基于现有样式创建新的样式变体

## 自定义扩展

如果需要添加新的颜色或样式，建议：

1. 在相应的 `Colors.axaml` 文件中添加颜色定义
2. 在 `Controls.axaml` 文件中添加样式定义
3. 保持 Light 和 Dark 主题的一致性
4. 遵循现有的命名约定

## 注意事项

- 主题系统依赖 AuroraUI 框架的主题模块
- 确保所有 XAML 资源文件都正确包含在项目中
- 主题切换会自动应用到所有使用动态资源的控件
- 建议在应用启动时初始化主题服务
