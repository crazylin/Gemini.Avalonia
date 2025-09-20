#!/usr/bin/env python3
# -*- coding: utf-8 -*-

import re
from pathlib import Path

def fix_xmlns_correctly(theme_file_path):
    """正确修复主题文件中的xmlns声明问题"""
    if not theme_file_path.exists():
        print(f"警告: 主题文件不存在: {theme_file_path}")
        return False
    
    content = theme_file_path.read_text(encoding='utf-8')
    
    # 检查是否已经有正确的xmlns声明
    if 'xmlns:dock="clr-namespace:Dock.Avalonia.Controls;assembly=Dock.Avalonia"' in content:
        # 如果已经有了，检查是否在Styles标签上
        if '<Styles xmlns:dock="clr-namespace:Dock.Avalonia.Controls;assembly=Dock.Avalonia">' in content:
            print("  已经正确配置")
            return True
    
    # 先移除所有错误的xmlns声明
    content = re.sub(
        r'(\s*)xmlns:dock="clr-namespace:Dock\.Avalonia\.Controls;assembly=Dock\.Avalonia"\s*',
        '',
        content
    )
    
    # 在Styles标签上添加正确的xmlns声明
    content = re.sub(
        r'(<Styles)(\s*>)',
        r'\1 xmlns:dock="clr-namespace:Dock.Avalonia.Controls;assembly=Dock.Avalonia"\2',
        content
    )
    
    theme_file_path.write_text(content, encoding='utf-8')
    return True

def main():
    """主函数"""
    print("🔧 开始正确修复xmlns声明问题...")
    
    # 修复基础主题
    base_themes = [
        Path(__file__).parent / 'src' / 'AuroraUI' / 'Modules' / 'Theme' / 'Resources' / 'LightTheme.axaml',
        Path(__file__).parent / 'src' / 'AuroraUI' / 'Modules' / 'Theme' / 'Resources' / 'DarkTheme.axaml'
    ]
    
    for theme_file in base_themes:
        if theme_file.exists():
            print(f"正在处理: {theme_file.name}")
            fix_xmlns_correctly(theme_file)
    
    # 扩展主题目录
    base_path = Path(__file__).parent / 'src' / 'AuroraUI' / 'Modules' / 'Theme' / 'Resources' / 'Extended'
    
    if base_path.exists():
        # 查找所有主题目录
        theme_dirs = [d for d in base_path.iterdir() if d.is_dir()]
        
        print(f"📁 找到 {len(theme_dirs)} 个扩展主题目录")
        
        for theme_dir in theme_dirs:
            theme_file = theme_dir / 'Theme.axaml'
            
            if theme_file.exists():
                print(f"正在处理: {theme_dir.name}")
                fix_xmlns_correctly(theme_file)
    
    print("🎉 处理完成！")

if __name__ == "__main__":
    main()
