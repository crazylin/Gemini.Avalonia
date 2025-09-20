#!/usr/bin/env python3
# -*- coding: utf-8 -*-

import re
from pathlib import Path

def remove_document_theme_includes(theme_file_path):
    """从扩展主题文件中移除DocumentTheme.axaml引用"""
    if not theme_file_path.exists():
        print(f"警告: 主题文件不存在: {theme_file_path}")
        return False
    
    content = theme_file_path.read_text(encoding='utf-8')
    
    # 查找并移除整个MergedDictionaries部分
    pattern = r'\s*<!-- 合并文档主题样式 -->\s*<ResourceDictionary\.MergedDictionaries>\s*<StyleInclude Source="avares://AuroraUI/Modules/Theme/Resources/DocumentTheme\.axaml"/>\s*</ResourceDictionary\.MergedDictionaries>\s*'
    
    if re.search(pattern, content):
        updated_content = re.sub(pattern, '\n', content)
        theme_file_path.write_text(updated_content, encoding='utf-8')
        return True
    
    return False

def main():
    """主函数"""
    print("🔧 开始移除DocumentTheme.axaml引用...")
    
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
        if remove_document_theme_includes(theme_file):
            fixed_count += 1
            print(f"  ✅ 已移除引用")
        else:
            print(f"  ℹ️ 无需处理")
    
    print(f"\n🎉 处理完成！")
    print(f"   总主题数: {len(theme_dirs)}")
    print(f"   已处理: {fixed_count}")
    print(f"   无需处理: {len(theme_dirs) - fixed_count}")

if __name__ == "__main__":
    main()
