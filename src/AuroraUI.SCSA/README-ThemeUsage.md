# SCSA 主题系统使用指南

## 🎨 **主题系统架构**

SCSA 的主题系统采用"监听+扩展"的架构，与框架主题服务协同工作：

```
框架主题服务 (ThemeService)
    ↓ 主题切换事件
SCSA 主题管理器 (SCSAThemeResourceManager)
    ↓ 同步加载
SCSA 特定主题资源 (Themes/Light|Dark/Theme.axaml)
```

## 🔧 **工作原理**

### 1. **自动初始化**
- SCSA 模块在后初始化阶段自动创建并初始化主题管理器
- 主题管理器自动订阅框架主题服务的变化事件

### 2. **主题同步**
- 当用户在设置中切换主题时
- 框架主题服务首先切换基础主题
- SCSA 主题管理器监听到变化，同步加载对应的 SCSA 主题

### 3. **资源覆盖**
- SCSA 主题资源会覆盖框架的默认样式
- 通过 `Application.Current.Resources.MergedDictionaries` 进行资源合并
- 后加载的资源具有更高的优先级

## 📂 **主题文件结构**

```
AuroraUI.SCSA/
├── Themes/
│   ├── Light/
│   │   ├── Colors.axaml      # 浅色主题颜色定义
│   │   └── Theme.axaml       # 浅色主题完整资源
│   ├── Dark/
│   │   ├── Colors.axaml      # 深色主题颜色定义
│   │   └── Theme.axaml       # 深色主题完整资源
│   └── Common/
│       └── Controls.axaml    # 通用控件样式
```

## 🎯 **如何测试主题切换**

### 1. **启动应用程序**
```bash
cd /Users/crazylin/code/AuroraUI/src/AuroraUI.SCSA
dotnet run
```

### 2. **打开设置对话框**
- 菜单 → 工具 → 选项
- 或使用快捷键

### 3. **切换主题**
- 在"应用程序设置"页面中
- 选择不同的主题：Light / Dark / Auto
- 观察应用程序界面的变化

### 4. **验证 SCSA 主题加载**
查看控制台日志，应该看到类似：
```
info: SCSAThemeResourceManager[0]
      框架主题已切换: Light -> Dark，同步加载 SCSA 主题
info: SCSAThemeResourceManager[0]
      加载 SCSA 主题资源: Dark
info: SCSAThemeResourceManager[0]
      SCSA 主题资源加载成功: Dark
```

## 🎨 **自定义主题样式**

### 1. **修改颜色**
编辑 `Themes/Light/Colors.axaml` 或 `Themes/Dark/Colors.axaml`：
```xml
<Color x:Key="SCSAPrimaryColor">#FF007ACC</Color>
<SolidColorBrush x:Key="SCSAPrimaryBrush" Color="{StaticResource SCSAPrimaryColor}"/>
```

### 2. **修改控件样式**
编辑 `Themes/Common/Controls.axaml`：
```xml
<Style Selector="Button.scsa-primary">
    <Setter Property="Background" Value="{StaticResource SCSAPrimaryBrush}"/>
    <Setter Property="Foreground" Value="White"/>
</Style>
```

### 3. **在界面中使用**
在 AXAML 文件中：
```xml
<Button Classes="scsa-primary" Content="SCSA 按钮"/>
```

## 🔍 **调试和故障排除**

### 1. **检查主题管理器初始化**
日志中应该看到：
```
info: AuroraUI.SCSA.Module[0]
      SCSA 主题资源管理器已初始化
```

### 2. **检查主题切换响应**
切换主题时应该看到：
```
info: SCSAThemeResourceManager[0]
      框架主题已切换: X -> Y，同步加载 SCSA 主题
```

### 3. **检查资源文件**
确保主题文件存在且格式正确：
- `avares://AuroraUI.SCSA/Themes/Light/Theme.axaml`
- `avares://AuroraUI.SCSA/Themes/Dark/Theme.axaml`

## 💡 **最佳实践**

1. **保持资源分离**：将颜色定义和控件样式分开
2. **使用语义化命名**：如 `SCSAPrimaryColor`、`DeviceConnectedBrush`
3. **测试所有主题**：确保 Light 和 Dark 主题都正常工作
4. **避免硬编码颜色**：使用主题资源而不是直接的颜色值

## 🚀 **扩展功能**

### 1. **添加新主题**
- 创建新的主题文件夹（如 `Themes/HighContrast/`）
- 在 `GetSCSAThemeResourceUri` 方法中添加新的映射
- 扩展 `ThemeType` 枚举（如果需要）

### 2. **动态主题切换**
```csharp
var themeService = IoC.Get<IThemeService>();
themeService.ChangeTheme(ThemeType.Dark);
```

### 3. **主题切换回调**
```csharp
var scsaThemeManager = IoC.Get<SCSAThemeResourceManager>();
// 主题管理器会自动响应框架主题服务的变化
```
