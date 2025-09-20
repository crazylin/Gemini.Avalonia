# SCSA 主题预览和转换工具

## 🎨 功能概述

这个工具包含两个主要部分：

1. **主题预览器** (`index.html`) - 在网页中预览不同主题效果
2. **主题转换工具** (`theme-converter.html`) - 将网页主题转换为 Avalonia XAML

## 🚀 快速开始

### 1. 启动主题预览器

在浏览器中打开 `index.html`：

```bash
# 方法 1: 直接在浏览器中打开文件
open index.html

# 方法 2: 使用简单的 HTTP 服务器
python -m http.server 8000
# 然后访问 http://localhost:8000
```

### 2. 使用主题转换工具

在浏览器中打开 `theme-converter.html`：

```bash
open theme-converter.html
```

## 🎯 使用流程

### 步骤 1: 设计和预览主题

1. 打开 `index.html`
2. 点击右上角的主题按钮切换不同主题
3. 观察界面效果，包括：
   - 侧边栏和导航
   - 按钮和表单元素
   - 卡片和数据表格
   - 颜色搭配和对比度
4. 使用快捷键快速切换：
   - `Ctrl/Cmd + 1`: 现代蓝色
   - `Ctrl/Cmd + 2`: 专业深色
   - `Ctrl/Cmd + 3`: 自然绿色
   - `Ctrl/Cmd + 4`: 夕阳橙色
   - `Ctrl/Cmd + 5`: 极简灰色
   - `Ctrl/Cmd + 6`: 高对比度

### 步骤 2: 转换为 Avalonia 主题

1. 打开 `theme-converter.html`
2. 选择要转换的主题
3. 点击"转换主题"按钮
4. 在右侧查看生成的 XAML 代码
5. 复制需要的代码到对应文件

### 步骤 3: 应用到 SCSA 项目

1. 将生成的颜色定义复制到：
   ```
   Themes/Light/Colors.axaml
   Themes/Dark/Colors.axaml
   ```

2. 将控件样式复制到：
   ```
   Themes/Common/Controls.axaml
   ```

3. 更新主题文件：
   ```
   Themes/Light/Theme.axaml
   Themes/Dark/Theme.axaml
   ```

## 🎨 可用主题

### ✨ 基础主题

#### 1. 现代蓝色 (Modern Blue)
- **特点**: 专业、清新的蓝色主题
- **适用**: 长时间工作，专业应用
- **主色调**: #4f7cff

#### 2. 专业深色 (Dark Professional)
- **特点**: 经典深色主题，减少眼部疲劳
- **适用**: 夜间工作，暗光环境
- **主色调**: #64ffda

#### 3. 自然绿色 (Nature Green)
- **特点**: 清新自然的绿色主题
- **适用**: 营造宁静专注的工作环境
- **主色调**: #27ae60

### 🏢 专业主题

#### 4. 深海蓝色 (Ocean Blue)
- **特点**: 深邃海洋主题，沉稳专业
- **适用**: 长期专注工作，企业级应用
- **主色调**: #006ba6

#### 5. 企业金色 (Corporate Gold)
- **特点**: 高端企业风格，金色点缀
- **适用**: 体现专业与品质的商务场景
- **主色调**: #b8860b

#### 6. 医疗洁净 (Medical Clean)
- **特点**: 医疗级洁净主题，简洁可靠
- **适用**: 精密仪器操作界面，严谨工作环境
- **主色调**: #0277bd

### 🎨 特色主题

#### 7. 赛博朋克 (Cyberpunk Neon)
- **特点**: 未来科技风格，霓虹色彩
- **适用**: 创新型工作环境，科技感应用
- **主色调**: #00ffff

#### 8. 深林主题 (Forest Dark)
- **特点**: 深色森林主题，自然沉静
- **适用**: 需要专注的深度工作，自然爱好者
- **主色调**: #4caf50

#### 9. 复古终端 (Retro Terminal)
- **特点**: 经典终端风格，绿色磷光屏效果
- **适用**: 怀旧极客风格，程序员专用
- **主色调**: #00ff41

### 💎 高级主题

#### 10. 皇家紫色 (Royal Purple)
- **特点**: 高贵典雅的紫色主题
- **适用**: 彰显品味与格调的高端应用
- **主色调**: #673ab7

#### 11. 夕阳橙色 (Sunset Orange)
- **特点**: 温暖活力的橙色主题
- **适用**: 激发创造力和热情
- **主色调**: #ff6b35

#### 12. 极地白色 (Arctic White)
- **特点**: 极简纯净的白色主题
- **适用**: 最大化内容可读性，简洁至上
- **主色调**: #2196f3

### ♿ 无障碍主题

#### 13. 高对比度 (High Contrast)
- **特点**: 高对比度主题，提高可读性
- **适用**: 视力敏感用户，无障碍设计
- **主色调**: #0066ff

#### 14. 极简灰色 (Minimal Grey)
- **特点**: 简约优雅的灰色主题
- **适用**: 专注内容，减少干扰
- **主色调**: #5a67d8

## 🔧 自定义主题

### 修改现有主题

1. 编辑 `themes.js` 文件
2. 修改对应主题的 `variables` 对象
3. 调整颜色值和样式
4. 在预览器中查看效果

### 创建新主题

1. 在 `themes.js` 中添加新主题对象
2. 定义颜色变量和样式
3. 在 HTML 中添加对应的按钮
4. 测试和优化

### 主题结构

```javascript
'theme-name': {
    name: '主题名称',
    description: '主题描述',
    variables: {
        '--primary-color': '#颜色值',
        '--background-color': '#颜色值',
        // ... 更多颜色定义
    },
    styles: `
        /* 自定义CSS样式 */
        .selector {
            property: value;
        }
    `
}
```

## 🛠️ 高级功能

### 导出主题配置

在浏览器控制台中运行：

```javascript
// 导出特定主题的 Avalonia XAML
exportThemeForAvalonia('modern-blue');

// 查看所有主题
console.log(themes);
```

### 动态主题切换

```javascript
// 程序化切换主题
applyTheme('dark-professional');
```

## 📝 注意事项

1. **浏览器兼容性**: 建议使用现代浏览器（Chrome, Firefox, Safari, Edge）
2. **颜色格式**: 支持 HEX 和 RGBA 格式，会自动转换为 Avalonia 格式
3. **样式优先级**: 后定义的样式会覆盖先定义的样式
4. **响应式设计**: 主题在不同屏幕尺寸下都有良好的表现

## 🔗 相关文件

- `index.html` - 主题预览器
- `theme-converter.html` - 主题转换工具
- `themes.js` - 主题定义文件
- `README.md` - 使用说明（本文件）

## 🎯 下一步

1. 选择或创建适合的主题
2. 转换为 Avalonia XAML 代码
3. 集成到 SCSA 项目中
4. 测试主题切换功能
5. 根据需要进行微调
