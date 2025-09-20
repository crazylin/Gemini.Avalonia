#!/usr/bin/env python3
# -*- coding: utf-8 -*-

import re
from pathlib import Path

def clean_xmlns_declarations(theme_file_path):
    """清理主题文件中的重复xmlns:dock声明"""
    if not theme_file_path.exists():
        return False
    
    content = theme_file_path.read_text(encoding='utf-8')
    
    # 移除所有非根元素的xmlns:dock声明
    content = re.sub(
        r'\s+xmlns:dock="clr-namespace:Dock\.Avalonia\.Controls;assembly=Dock\.Avalonia"',
        '',
        content
    )
    
    # 确保根元素有xmlns:dock声明
    if 'xmlns:dock=' not in content:
        content = re.sub(
            r'(<ResourceDictionary[^>]*xmlns:x="http://schemas\.microsoft\.com/winfx/2006/xaml")(\s*>)',
            r'\1\n                    xmlns:dock="clr-namespace:Dock.Avalonia.Controls;assembly=Dock.Avalonia"\2',
            content
        )
    
    theme_file_path.write_text(content, encoding='utf-8')
    return True

def main():
    """主函数"""
    print("🔧 开始清理xmlns声明...")
    
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
                clean_xmlns_declarations(theme_file)
                print(f"  ✅ 已清理")
    
    print("🎉 处理完成！")

if __name__ == "__main__":
    main()
