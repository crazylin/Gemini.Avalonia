#!/usr/bin/env python3
# -*- coding: utf-8 -*-

import re
from pathlib import Path

def clean_theme_styles(theme_file_path):
    """从主题文件中移除Styles部分"""
    if not theme_file_path.exists():
        return False
    
    content = theme_file_path.read_text(encoding='utf-8')
    
    # 移除xmlns:dock声明（如果存在）
    content = re.sub(
        r'(\s+)xmlns:dock="clr-namespace:Dock\.Avalonia\.Controls;assembly=Dock\.Avalonia"',
        '',
        content
    )
    
    # 移除整个Styles部分
    content = re.sub(
        r'\s*<!-- 文档主题样式 -->\s*<Styles>.*?</Styles>\s*',
        '\n\n',
        content,
        flags=re.DOTALL
    )
    
    theme_file_path.write_text(content, encoding='utf-8')
    return True

def main():
    """主函数"""
    print("🧹 清理扩展主题文件中的Styles部分...")
    
    # 扩展主题目录
    extended_themes_dir = Path("src/AuroraUI/Modules/Theme/Resources/Extended")
    
    if not extended_themes_dir.exists():
        print(f"❌ 扩展主题目录不存在: {extended_themes_dir}")
        return
    
    # 处理所有扩展主题
    for theme_dir in extended_themes_dir.iterdir():
        if theme_dir.is_dir():
            theme_file = theme_dir / "Theme.axaml"
            if theme_file.exists():
                if clean_theme_styles(theme_file):
                    print(f"✅ 已清理: {theme_file}")
                else:
                    print(f"❌ 清理失败: {theme_file}")
    
    print("🎉 所有扩展主题文件清理完成!")

if __name__ == "__main__":
    main()
