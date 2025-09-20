#!/usr/bin/env python3
"""
批量更新所有主题文件，应用完整的 Avalonia 资源覆盖
"""

import os
import shutil
from pathlib import Path

# 主题映射
THEME_MAPPING = {
    'ModernBlue': 'ModernBlue',
    'DarkProfessional': 'DarkProfessional', 
    'NatureGreen': 'NatureGreen',
    'OceanBlue': 'OceanBlue',
    'CorporateGold': 'CorporateGold',
    'MedicalClean': 'MedicalClean',
    'CyberpunkNeon': 'CyberpunkNeon',
    'ForestDark': 'ForestDark',
    'RetroTerminal': 'RetroTerminal',
    'RoyalPurple': 'RoyalPurple',
    'SunsetOrange': 'SunsetOrange',
    'ArcticWhite': 'ArcticWhite',
    'HighContrast': 'HighContrast',
    'MinimalGrey': 'MinimalGrey'
}

# 完整主题模板
THEME_TEMPLATE = '''<!-- {theme_name} - 完整主题文件 -->
<!-- {theme_description} -->
<ResourceDictionary xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

  <!-- 合并基础主题和颜色 -->
  <ResourceDictionary.MergedDictionaries>
    <ResourceInclude Source="avares://AuroraUI/Modules/Theme/Resources/{base_theme}Theme.axaml"/>
    <ResourceInclude Source="avares://AuroraUI/Modules/Theme/Resources/Extended/{theme_folder}/Colors.axaml"/>
  </ResourceDictionary.MergedDictionaries>

  <!-- =========================== -->
  <!-- 核心系统背景色覆盖 -->
  <!-- =========================== -->
  
  <!-- 应用程序主背景 -->
  <SolidColorBrush x:Key="ApplicationPageBackgroundThemeBrush" Color="{{StaticResource BackgroundColor}}"/>
  <SolidColorBrush x:Key="SystemControlPageBackgroundAltHighBrush" Color="{{StaticResource BackgroundColor}}"/>
  <SolidColorBrush x:Key="SystemControlPageBackgroundChromeLowBrush" Color="{{StaticResource BackgroundColor}}"/>
  
  <!-- 控件表面背景 -->
  <SolidColorBrush x:Key="SystemControlBackgroundChromeMediumBrush" Color="{{StaticResource SurfaceColor}}"/>
  <SolidColorBrush x:Key="SystemControlBackgroundChromeMediumLowBrush" Color="{{StaticResource SurfaceColor}}"/>
  <SolidColorBrush x:Key="SystemControlBackgroundChromeHighBrush" Color="{{StaticResource SurfaceColor}}"/>
  <SolidColorBrush x:Key="SystemControlBackgroundBaseHighBrush" Color="{{StaticResource SurfaceColor}}"/>
  <SolidColorBrush x:Key="SystemControlBackgroundBaseLowBrush" Color="{{StaticResource CardBackground}}"/>
  <SolidColorBrush x:Key="SystemControlBackgroundBaseMediumBrush" Color="{{StaticResource CardBackground}}"/>
  
  <!-- =========================== -->
  <!-- 主题色和强调色覆盖 -->
  <!-- =========================== -->
  
  <!-- 系统强调色 -->
  <SolidColorBrush x:Key="SystemAccentColor" Color="{{StaticResource PrimaryColor}}"/>
  <SolidColorBrush x:Key="SystemAccentColorLight1" Color="{{StaticResource SecondaryColor}}"/>
  <SolidColorBrush x:Key="SystemAccentColorLight2" Color="{{StaticResource AccentColor}}"/>
  <SolidColorBrush x:Key="SystemAccentColorLight3" Color="{{StaticResource AccentColor}}"/>
  <SolidColorBrush x:Key="SystemAccentColorDark1" Color="{{StaticResource PrimaryColor}}"/>
  <SolidColorBrush x:Key="SystemAccentColorDark2" Color="{{StaticResource PrimaryColor}}"/>
  <SolidColorBrush x:Key="SystemAccentColorDark3" Color="{{StaticResource PrimaryColor}}"/>
  
  <!-- 控件强调色 -->
  <SolidColorBrush x:Key="SystemControlHighlightAccentBrush" Color="{{StaticResource PrimaryColor}}"/>
  <SolidColorBrush x:Key="SystemControlHighlightAltAccentBrush" Color="{{StaticResource SecondaryColor}}"/>
  <SolidColorBrush x:Key="SystemControlBackgroundAccentBrush" Color="{{StaticResource PrimaryColor}}"/>
  
  <!-- =========================== -->
  <!-- 文本和前景色覆盖 -->
  <!-- =========================== -->
  
  <!-- 主要文本颜色 -->
  <SolidColorBrush x:Key="SystemControlForegroundBaseHighBrush" Color="{{StaticResource TextPrimary}}"/>
  <SolidColorBrush x:Key="SystemControlForegroundBaseMediumHighBrush" Color="{{StaticResource TextPrimary}}"/>
  <SolidColorBrush x:Key="SystemControlForegroundBaseMediumBrush" Color="{{StaticResource TextSecondary}}"/>
  <SolidColorBrush x:Key="SystemControlForegroundBaseLowBrush" Color="{{StaticResource TextMuted}}"/>
  <SolidColorBrush x:Key="SystemControlForegroundBaseDisabledBrush" Color="{{StaticResource TextMuted}}"/>
  
  <!-- 强调文本颜色 -->
  <SolidColorBrush x:Key="SystemControlForegroundAccentBrush" Color="{{StaticResource PrimaryColor}}"/>
  <SolidColorBrush x:Key="SystemControlHighlightAltBaseHighBrush" Color="{{StaticResource TextPrimary}}"/>
  <SolidColorBrush x:Key="SystemControlHighlightBaseHighBrush" Color="{{StaticResource SurfaceColor}}"/>
  <SolidColorBrush x:Key="SystemControlHighlightBaseMediumBrush" Color="{{StaticResource SurfaceColor}}"/>
  
  <!-- =========================== -->
  <!-- 边框和分隔线覆盖 -->
  <!-- =========================== -->
  
  <!-- 控件边框 -->
  <SolidColorBrush x:Key="SystemControlForegroundBaseMediumLowBrush" Color="{{StaticResource BorderColor}}"/>
  <SolidColorBrush x:Key="SystemControlForegroundChromeDisabledLowBrush" Color="{{StaticResource BorderColor}}"/>
  <SolidColorBrush x:Key="SystemControlForegroundChromeHighBrush" Color="{{StaticResource BorderColor}}"/>
  <SolidColorBrush x:Key="SystemControlForegroundChromeMediumBrush" Color="{{StaticResource BorderColor}}"/>
  
  <!-- =========================== -->
  <!-- 按钮控件样式覆盖 -->
  <!-- =========================== -->
  
  <!-- 按钮背景 -->
  <SolidColorBrush x:Key="ButtonBackground" Color="{{StaticResource SurfaceColor}}"/>
  <SolidColorBrush x:Key="ButtonBackgroundPointerOver" Color="{{StaticResource HoverColor}}"/>
  <SolidColorBrush x:Key="ButtonBackgroundPressed" Color="{{StaticResource PrimaryColorAlpha}}"/>
  <SolidColorBrush x:Key="ButtonBackgroundDisabled" Color="{{StaticResource BorderColor}}"/>
  
  <!-- 按钮前景 -->
  <SolidColorBrush x:Key="ButtonForeground" Color="{{StaticResource TextPrimary}}"/>
  <SolidColorBrush x:Key="ButtonForegroundPointerOver" Color="{{StaticResource TextPrimary}}"/>
  <SolidColorBrush x:Key="ButtonForegroundPressed" Color="{{StaticResource TextPrimary}}"/>
  <SolidColorBrush x:Key="ButtonForegroundDisabled" Color="{{StaticResource TextMuted}}"/>
  
  <!-- 按钮边框 -->
  <SolidColorBrush x:Key="ButtonBorderBrush" Color="{{StaticResource BorderColor}}"/>
  <SolidColorBrush x:Key="ButtonBorderBrushPointerOver" Color="{{StaticResource PrimaryColor}}"/>
  <SolidColorBrush x:Key="ButtonBorderBrushPressed" Color="{{StaticResource PrimaryColor}}"/>
  <SolidColorBrush x:Key="ButtonBorderBrushDisabled" Color="{{StaticResource BorderColor}}"/>
  
  <!-- =========================== -->
  <!-- 文本框控件样式覆盖 -->
  <!-- =========================== -->
  
  <!-- 文本框背景 -->
  <SolidColorBrush x:Key="TextControlBackground" Color="{{StaticResource SurfaceColor}}"/>
  <SolidColorBrush x:Key="TextControlBackgroundPointerOver" Color="{{StaticResource SurfaceColor}}"/>
  <SolidColorBrush x:Key="TextControlBackgroundFocused" Color="{{StaticResource SurfaceColor}}"/>
  <SolidColorBrush x:Key="TextControlBackgroundDisabled" Color="{{StaticResource BorderColor}}"/>
  
  <!-- 文本框前景 -->
  <SolidColorBrush x:Key="TextControlForeground" Color="{{StaticResource TextPrimary}}"/>
  <SolidColorBrush x:Key="TextControlForegroundPointerOver" Color="{{StaticResource TextPrimary}}"/>
  <SolidColorBrush x:Key="TextControlForegroundFocused" Color="{{StaticResource TextPrimary}}"/>
  <SolidColorBrush x:Key="TextControlForegroundDisabled" Color="{{StaticResource TextMuted}}"/>
  
  <!-- 文本框边框 -->
  <SolidColorBrush x:Key="TextControlBorderBrush" Color="{{StaticResource BorderColor}}"/>
  <SolidColorBrush x:Key="TextControlBorderBrushPointerOver" Color="{{StaticResource PrimaryColor}}"/>
  <SolidColorBrush x:Key="TextControlBorderBrushFocused" Color="{{StaticResource PrimaryColor}}"/>
  <SolidColorBrush x:Key="TextControlBorderBrushDisabled" Color="{{StaticResource BorderColor}}"/>
  
  <!-- 占位符文本 -->
  <SolidColorBrush x:Key="TextControlPlaceholderForeground" Color="{{StaticResource TextMuted}}"/>
  <SolidColorBrush x:Key="TextControlPlaceholderForegroundPointerOver" Color="{{StaticResource TextSecondary}}"/>
  <SolidColorBrush x:Key="TextControlPlaceholderForegroundFocused" Color="{{StaticResource TextSecondary}}"/>
  
  <!-- =========================== -->
  <!-- 菜单和上下文菜单覆盖 -->
  <!-- =========================== -->
  
  <!-- 菜单背景 -->
  <SolidColorBrush x:Key="MenuFlyoutPresenterBackground" Color="{{StaticResource SurfaceColor}}"/>
  <SolidColorBrush x:Key="MenuFlyoutItemBackground" Color="Transparent"/>
  <SolidColorBrush x:Key="MenuFlyoutItemBackgroundPointerOver" Color="{{StaticResource HoverColor}}"/>
  <SolidColorBrush x:Key="MenuFlyoutItemBackgroundPressed" Color="{{StaticResource PrimaryColorAlpha}}"/>
  
  <!-- 菜单前景 -->
  <SolidColorBrush x:Key="MenuFlyoutItemForeground" Color="{{StaticResource TextPrimary}}"/>
  <SolidColorBrush x:Key="MenuFlyoutItemForegroundPointerOver" Color="{{StaticResource TextPrimary}}"/>
  <SolidColorBrush x:Key="MenuFlyoutItemForegroundPressed" Color="{{StaticResource TextPrimary}}"/>
  <SolidColorBrush x:Key="MenuFlyoutItemForegroundDisabled" Color="{{StaticResource TextMuted}}"/>
  
  <!-- =========================== -->
  <!-- 列表和网格控件覆盖 -->
  <!-- =========================== -->
  
  <!-- 列表项背景 -->
  <SolidColorBrush x:Key="ListViewItemBackground" Color="Transparent"/>
  <SolidColorBrush x:Key="ListViewItemBackgroundPointerOver" Color="{{StaticResource HoverColor}}"/>
  <SolidColorBrush x:Key="ListViewItemBackgroundPressed" Color="{{StaticResource PrimaryColorAlpha}}"/>
  <SolidColorBrush x:Key="ListViewItemBackgroundSelected" Color="{{StaticResource PrimaryColorAlpha}}"/>
  <SolidColorBrush x:Key="ListViewItemBackgroundSelectedPointerOver" Color="{{StaticResource PrimaryColorAlpha}}"/>
  
  <!-- 列表项前景 -->
  <SolidColorBrush x:Key="ListViewItemForeground" Color="{{StaticResource TextPrimary}}"/>
  <SolidColorBrush x:Key="ListViewItemForegroundPointerOver" Color="{{StaticResource TextPrimary}}"/>
  <SolidColorBrush x:Key="ListViewItemForegroundPressed" Color="{{StaticResource TextPrimary}}"/>
  <SolidColorBrush x:Key="ListViewItemForegroundSelected" Color="{{StaticResource TextPrimary}}"/>

</ResourceDictionary>'''

# 主题信息
THEME_INFO = {
    'ModernBlue': {
        'name': '现代蓝色',
        'description': '专业、清新的蓝色主题，适合长时间工作',
        'is_dark': False
    },
    'DarkProfessional': {
        'name': '专业深色',
        'description': '经典深色主题，减少眼部疲劳，适合夜间工作',
        'is_dark': True
    },
    'NatureGreen': {
        'name': '自然绿色',
        'description': '清新自然的绿色主题，营造宁静专注的工作环境',
        'is_dark': False
    },
    'OceanBlue': {
        'name': '深海蓝色',
        'description': '深邃海洋主题，沉稳专业，适合长期专注工作',
        'is_dark': False
    },
    'CorporateGold': {
        'name': '企业金色',
        'description': '高端企业风格，金色点缀，体现专业与品质',
        'is_dark': False
    },
    'MedicalClean': {
        'name': '医疗洁净',
        'description': '医疗级洁净主题，简洁可靠，适合精密仪器操作界面',
        'is_dark': False
    },
    'CyberpunkNeon': {
        'name': '赛博朋克',
        'description': '未来科技风格，霓虹色彩，适合创新型工作环境',
        'is_dark': True
    },
    'ForestDark': {
        'name': '深林主题',
        'description': '深色森林主题，自然沉静，适合需要专注的深度工作',
        'is_dark': True
    },
    'RetroTerminal': {
        'name': '复古终端',
        'description': '经典终端风格，绿色磷光屏效果，怀旧极客风格',
        'is_dark': True
    },
    'RoyalPurple': {
        'name': '皇家紫色',
        'description': '高贵典雅的紫色主题，彰显品味与格调',
        'is_dark': False
    },
    'SunsetOrange': {
        'name': '夕阳橙色',
        'description': '温暖活力的橙色主题，激发创造力和热情',
        'is_dark': False
    },
    'ArcticWhite': {
        'name': '极地白色',
        'description': '极简纯净的白色主题，最大化内容可读性',
        'is_dark': False
    },
    'HighContrast': {
        'name': '高对比度',
        'description': '高对比度主题，提高可读性，适合视力敏感用户',
        'is_dark': False
    },
    'MinimalGrey': {
        'name': '极简灰色',
        'description': '简约优雅的灰色主题，专注内容，减少干扰',
        'is_dark': False
    }
}

def update_theme_file(theme_folder, theme_info):
    """更新单个主题文件"""
    theme_path = Path(__file__).parent.parent.parent / 'AuroraUI' / 'Modules' / 'Theme' / 'Resources' / 'Extended' / theme_folder
    theme_file = theme_path / 'Theme.axaml'
    
    if not theme_file.exists():
        print(f"警告: 主题文件不存在: {theme_file}")
        return
    
    # 确定基础主题
    base_theme = 'Dark' if theme_info['is_dark'] else 'Light'
    
    # 生成主题内容
    content = THEME_TEMPLATE.format(
        theme_name=theme_info['name'],
        theme_description=theme_info['description'],
        theme_folder=theme_folder,
        base_theme=base_theme
    )
    
    # 写入文件
    with open(theme_file, 'w', encoding='utf-8') as f:
        f.write(content)
    
    print(f"✅ 已更新主题: {theme_info['name']} ({theme_folder})")

def main():
    """主函数"""
    print("🎨 开始批量更新所有主题文件...")
    
    base_path = Path(__file__).parent.parent.parent / 'AuroraUI' / 'Modules' / 'Theme' / 'Resources' / 'Extended'
    
    if not base_path.exists():
        print(f"❌ 错误: 主题目录不存在: {base_path}")
        return
    
    updated_count = 0
    
    for theme_folder, theme_info in THEME_INFO.items():
        if theme_folder in THEME_MAPPING:
            try:
                update_theme_file(theme_folder, theme_info)
                updated_count += 1
            except Exception as e:
                print(f"❌ 更新主题 {theme_folder} 失败: {e}")
    
    print(f"\n🎉 批量更新完成！")
    print(f"📊 总计更新了 {updated_count} 个主题文件")
    print(f"🔧 现在所有主题都包含完整的 Avalonia 资源覆盖")

if __name__ == "__main__":
    main()
