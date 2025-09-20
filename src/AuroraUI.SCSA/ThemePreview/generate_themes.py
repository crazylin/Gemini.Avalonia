#!/usr/bin/env python3
"""
SCSA 主题生成器
将 JavaScript 主题定义转换为 Avalonia XAML 主题文件
"""

import json
import os
import re
from pathlib import Path

# 主题映射：JavaScript 主题名 -> XAML 主题文件夹名
THEME_MAPPING = {
    'modern-blue': 'ModernBlue',
    'dark-professional': 'DarkProfessional', 
    'nature-green': 'NatureGreen',
    'ocean-blue': 'OceanBlue',
    'corporate-gold': 'CorporateGold',
    'medical-clean': 'MedicalClean',
    'cyberpunk-neon': 'CyberpunkNeon',
    'forest-dark': 'ForestDark',
    'retro-terminal': 'RetroTerminal',
    'royal-purple': 'RoyalPurple',
    'sunset-orange': 'SunsetOrange',
    'arctic-white': 'ArcticWhite',
    'high-contrast': 'HighContrast',
    'minimal-grey': 'MinimalGrey'
}

def parse_css_color(color_value):
    """解析 CSS 颜色值为 Avalonia 颜色"""
    if color_value.startswith('#'):
        return color_value.upper()
    elif color_value.startswith('rgba'):
        # 解析 rgba(r, g, b, a) 为 #AARRGGBB
        match = re.match(r'rgba\((\d+),\s*(\d+),\s*(\d+),\s*([\d.]+)\)', color_value)
        if match:
            r, g, b, a = match.groups()
            alpha = format(int(float(a) * 255), '02X')
            red = format(int(r), '02X')
            green = format(int(g), '02X')
            blue = format(int(b), '02X')
            return f"#{alpha}{red}{green}{blue}"
    return "#FF000000"  # 默认黑色

def css_name_to_pascal_case(css_name):
    """将 CSS 变量名转换为 PascalCase"""
    # 移除 -- 前缀，将 - 分隔的单词转换为 PascalCase
    name = css_name.replace('--', '').replace('-', ' ')
    return ''.join(word.capitalize() for word in name.split())

def generate_colors_axaml(theme_name, theme_data, output_dir):
    """生成颜色定义 AXAML 文件"""
    colors_content = f'''<!-- {theme_data['name']} - 颜色定义 -->
<!-- {theme_data['description']} -->
<ResourceDictionary xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

  <!-- 颜色定义 -->
'''
    
    brush_content = '\n  <!-- 画刷定义 -->\n'
    
    # 生成颜色和画刷定义
    for css_var, css_value in theme_data['variables'].items():
        if css_value.startswith('#') or css_value.startswith('rgba'):
            color_name = css_name_to_pascal_case(css_var)
            color_value = parse_css_color(css_value)
            
            colors_content += f'  <Color x:Key="{color_name}">{color_value}</Color>\n'
            brush_content += f'  <SolidColorBrush x:Key="{color_name}Brush" Color="{{StaticResource {color_name}}}"/>\n'
    
    colors_content += brush_content + '\n</ResourceDictionary>'
    
    # 写入文件
    colors_file = output_dir / 'Colors.axaml'
    with open(colors_file, 'w', encoding='utf-8') as f:
        f.write(colors_content)
    
    print(f"生成颜色文件: {colors_file}")

def generate_theme_axaml(theme_name, theme_data, output_dir):
    """生成主题 AXAML 文件"""
    # 判断是否为深色主题
    is_dark_theme = any(keyword in theme_name.lower() for keyword in ['dark', 'professional', 'cyberpunk', 'forest', 'terminal'])
    base_theme = 'DarkTheme' if is_dark_theme else 'LightTheme'
    
    theme_content = f'''<!-- {theme_data['name']} - 完整主题文件 -->
<!-- {theme_data['description']} -->
<ResourceDictionary xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

  <!-- 合并基础主题和颜色 -->
  <ResourceDictionary.MergedDictionaries>
    <ResourceInclude Source="avares://AuroraUI/Modules/Theme/Resources/{base_theme}.axaml"/>
    <ResourceInclude Source="avares://AuroraUI.SCSA/Themes/{THEME_MAPPING[theme_name]}/Colors.axaml"/>
  </ResourceDictionary.MergedDictionaries>

  <!-- {theme_data['name']} 特定样式覆盖 -->
  
  <!-- 主要背景色覆盖 -->
  <SolidColorBrush x:Key="ApplicationPageBackgroundThemeBrush" Color="{{StaticResource BackgroundColor}}"/>
  <SolidColorBrush x:Key="SystemControlBackgroundChromeMediumBrush" Color="{{StaticResource SurfaceColor}}"/>
  
  <!-- 主题色覆盖 -->
  <SolidColorBrush x:Key="SystemAccentColor" Color="{{StaticResource PrimaryColor}}"/>
  <SolidColorBrush x:Key="SystemAccentColorLight1" Color="{{StaticResource SecondaryColor}}"/>
  <SolidColorBrush x:Key="SystemAccentColorLight2" Color="{{StaticResource AccentColor}}"/>
  
  <!-- 文本颜色覆盖 -->
  <SolidColorBrush x:Key="SystemControlForegroundBaseHighBrush" Color="{{StaticResource TextPrimary}}"/>
  <SolidColorBrush x:Key="SystemControlForegroundBaseMediumBrush" Color="{{StaticResource TextSecondary}}"/>
  
  <!-- 边框颜色覆盖 -->
  <SolidColorBrush x:Key="SystemControlForegroundBaseLowBrush" Color="{{StaticResource BorderColor}}"/>

</ResourceDictionary>'''
    
    # 写入文件
    theme_file = output_dir / 'Theme.axaml'
    with open(theme_file, 'w', encoding='utf-8') as f:
        f.write(theme_content)
    
    print(f"生成主题文件: {theme_file}")

def extract_themes_from_js():
    """从 JavaScript 文件中提取主题定义"""
    js_file = Path(__file__).parent / 'themes.js'
    
    if not js_file.exists():
        print(f"错误: 找不到 {js_file}")
        return {}
    
    # 读取 JavaScript 文件
    with open(js_file, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # 简单的正则表达式解析（这里假设 themes 对象的格式相对固定）
    themes = {}
    
    # 查找每个主题定义
    theme_pattern = r"'([^']+)':\s*\{[^}]*name:\s*'([^']+)'[^}]*description:\s*'([^']+)'[^}]*variables:\s*\{([^}]+)\}"
    
    for match in re.finditer(theme_pattern, content, re.DOTALL):
        theme_key = match.group(1)
        if theme_key in THEME_MAPPING:
            name = match.group(2)
            description = match.group(3)
            variables_str = match.group(4)
            
            # 解析变量
            variables = {}
            var_pattern = r"'([^']+)':\s*'([^']+)'"
            for var_match in re.finditer(var_pattern, variables_str):
                var_name = var_match.group(1)
                var_value = var_match.group(2)
                variables[var_name] = var_value
            
            themes[theme_key] = {
                'name': name,
                'description': description,
                'variables': variables
            }
    
    return themes

def main():
    """主函数"""
    print("SCSA 主题生成器启动...")
    
    # 提取主题数据
    themes = extract_themes_from_js()
    
    if not themes:
        print("错误: 无法从 themes.js 中提取主题数据")
        return
    
    print(f"找到 {len(themes)} 个主题")
    
    # 基础输出目录
    base_output_dir = Path(__file__).parent.parent / 'Themes'
    
    # 为每个主题生成文件
    for theme_key, theme_data in themes.items():
        if theme_key in THEME_MAPPING:
            folder_name = THEME_MAPPING[theme_key]
            output_dir = base_output_dir / folder_name
            
            # 创建目录
            output_dir.mkdir(parents=True, exist_ok=True)
            
            print(f"\n生成主题: {theme_data['name']} -> {folder_name}")
            
            # 生成文件
            generate_colors_axaml(theme_key, theme_data, output_dir)
            generate_theme_axaml(theme_key, theme_data, output_dir)
    
    print(f"\n所有主题文件已生成到: {base_output_dir}")
    print("完成!")

if __name__ == "__main__":
    main()
