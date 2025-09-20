using System;

namespace AuroraUI.Modules.Theme.Models
{
    /// <summary>
    /// 主题信息
    /// </summary>
    public class ThemeInfo
    {
        /// <summary>
        /// 主题类型
        /// </summary>
        public ThemeType Type { get; set; }
        
        /// <summary>
        /// 主题名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 主题描述
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// 主题分类
        /// </summary>
        public ThemeCategory Category { get; set; }
        
        /// <summary>
        /// 主题图标
        /// </summary>
        public string Icon { get; set; }
        
        /// <summary>
        /// 主题预览色
        /// </summary>
        public string PreviewColor { get; set; }
        
        /// <summary>
        /// 是否为深色主题
        /// </summary>
        public bool IsDark { get; set; }
        
        /// <summary>
        /// 主题资源文件路径
        /// </summary>
        public string ResourcePath { get; set; }

        public ThemeInfo()
        {
            Name = string.Empty;
            Description = string.Empty;
            Icon = string.Empty;
            PreviewColor = string.Empty;
            ResourcePath = string.Empty;
        }
    }
    
    /// <summary>
    /// 主题分类
    /// </summary>
    public enum ThemeCategory
    {
        /// <summary>
        /// 系统主题
        /// </summary>
        System,
        
        /// <summary>
        /// 基础主题
        /// </summary>
        Basic,
        
        /// <summary>
        /// 专业主题
        /// </summary>
        Professional,
        
        /// <summary>
        /// 特色主题
        /// </summary>
        Special,
        
        /// <summary>
        /// 高级主题
        /// </summary>
        Premium,
        
        /// <summary>
        /// 无障碍主题
        /// </summary>
        Accessibility
    }
}
