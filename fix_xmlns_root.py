#!/usr/bin/env python3
# -*- coding: utf-8 -*-

import re
from pathlib import Path

def fix_xmlns_in_root(theme_file_path):
    """修复主题文件，将xmlns:dock声明移到根元素"""
    if not theme_file_path.exists():
        print(f"警告: 主题文件不存在: {theme_file_path}")
        return False
    
    content = theme_file_path.read_text(encoding='utf-8')
    
    # 检查是否已经在根元素上有xmlns:dock声明
    if 'xmlns:dock="clr-namespace:Dock.Avalonia.Controls;assembly=Dock.Avalonia"' in content.split('\n')[0:3]:
        print("  已经在根元素上有xmlns:dock声明")
        # 移除Styles上的声明
        content = re.sub(
            r'(<Styles)\s+xmlns:dock="clr-namespace:Dock\.Avalonia\.Controls;assembly=Dock\.Avalonia"(>)',
            r'\1\2',
            content
        )
        theme_file_path.write_text(content, encoding='utf-8')
        return True
    
    # 在根元素上添加xmlns:dock声明
    content = re.sub(
        r'(<ResourceDictionary[^>]*xmlns:x="http://schemas\.microsoft\.com/winfx/2006/xaml")(\s*>)',
        r'\1\n                    xmlns:dock="clr-namespace:Dock.Avalonia.Controls;assembly=Dock.Avalonia"\2',
        content
    )
    
    # 移除Styles上的声明
    content = re.sub(
        r'(<Styles)\s+xmlns:dock="clr-namespace:Dock\.Avalonia\.Controls;assembly=Dock\.Avalonia"(>)',
        r'\1\2',
        content
    )
    
    theme_file_path.write_text(content, encoding='utf-8')
    return True

def main():
    """主函数"""
    print("🔧 开始修复xmlns声明到根元素...")
    
    # 扩展主题目录
    base_path = Path(__file__).parent / 'src' / 'AuroraUI' / 'Modules' / 'Theme' / 'Resources' / 'Extended'
    
    if not base_path.exists():
        print(f"❌ 错误: 扩展主题目录不存在: {base_path}")
        return
    
    # 查找所有主题目录
    theme_dirs = [d for d in base_path.iterdir() if d.is_dir()]
    
    print(f"📁 找到 {len(theme_dirs)} 个扩展主题目录")
    
    fixed_count = 0
    for theme_dir in theme_dirs:
        theme_file = theme_dir / 'Theme.axaml'
        
        if not theme_file.exists():
            print(f"  警告: 主题文件不存在: {theme_file}")
            continue
        
        print(f"正在处理: {theme_dir.name}")
        if fix_xmlns_in_root(theme_file):
            fixed_count += 1
            print(f"  ✅ 已修复")
        else:
            print(f"  ❌ 修复失败")
    
    print(f"\n🎉 处理完成！")
    print(f"   总主题数: {len(theme_dirs)}")
    print(f"   已修复: {fixed_count}")

if __name__ == "__main__":
    main()
