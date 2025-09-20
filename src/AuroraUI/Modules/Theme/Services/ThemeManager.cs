using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using AuroraUI.Modules.Theme.Models;

namespace AuroraUI.Modules.Theme.Services
{
    /// <summary>
    /// 主题管理器
    /// </summary>
    [Export(typeof(IThemeManager))]
    public class ThemeManager : IThemeManager
    {
        private readonly Dictionary<ThemeType, ThemeInfo> _themes;

        public ThemeManager()
        {
            _themes = new Dictionary<ThemeType, ThemeInfo>();
            InitializeThemes();
        }

        /// <summary>
        /// 获取所有主题
        /// </summary>
        public IEnumerable<ThemeInfo> GetAllThemes()
        {
            return _themes.Values;
        }

        /// <summary>
        /// 根据分类获取主题
        /// </summary>
        public IEnumerable<ThemeInfo> GetThemesByCategory(ThemeCategory category)
        {
            return _themes.Values.Where(t => t.Category == category);
        }

        /// <summary>
        /// 获取主题信息
        /// </summary>
        public ThemeInfo? GetThemeInfo(ThemeType themeType)
        {
            return _themes.TryGetValue(themeType, out var theme) ? theme : null;
        }

        /// <summary>
        /// 检查主题是否可用
        /// </summary>
        public bool IsThemeAvailable(ThemeType themeType)
        {
            return _themes.ContainsKey(themeType);
        }

        /// <summary>
        /// 获取主题分类信息
        /// </summary>
        public IEnumerable<ThemeCategoryInfo> GetCategories()
        {
            return new[]
            {
                new ThemeCategoryInfo
                {
                    Category = ThemeCategory.System,
                    Name = "系统主题",
                    Icon = "🔧",
                    Description = "跟随系统设置"
                },
                new ThemeCategoryInfo
                {
                    Category = ThemeCategory.Basic,
                    Name = "基础主题",
                    Icon = "✨",
                    Description = "经典实用的主题"
                },
                new ThemeCategoryInfo
                {
                    Category = ThemeCategory.Professional,
                    Name = "专业主题",
                    Icon = "🏢",
                    Description = "商务专业风格"
                },
                new ThemeCategoryInfo
                {
                    Category = ThemeCategory.Special,
                    Name = "特色主题",
                    Icon = "🎨",
                    Description = "独特创意风格"
                },
                new ThemeCategoryInfo
                {
                    Category = ThemeCategory.Premium,
                    Name = "高级主题",
                    Icon = "💎",
                    Description = "精致高端体验"
                },
                new ThemeCategoryInfo
                {
                    Category = ThemeCategory.Accessibility,
                    Name = "无障碍主题",
                    Icon = "♿",
                    Description = "提高可访问性"
                }
            };
        }

        private void InitializeThemes()
        {
            // 系统主题
            _themes[ThemeType.Light] = new ThemeInfo
            {
                Type = ThemeType.Light,
                Name = "浅色主题",
                Description = "标准浅色主题",
                Category = ThemeCategory.System,
                Icon = "☀️",
                PreviewColor = "#ffffff",
                IsDark = false,
                ResourcePath = "avares://AuroraUI/Modules/Theme/Resources/LightTheme.axaml"
            };

            _themes[ThemeType.Dark] = new ThemeInfo
            {
                Type = ThemeType.Dark,
                Name = "深色主题",
                Description = "标准深色主题",
                Category = ThemeCategory.System,
                Icon = "🌙",
                PreviewColor = "#2d2d2d",
                IsDark = true,
                ResourcePath = "avares://AuroraUI/Modules/Theme/Resources/DarkTheme.axaml"
            };

            _themes[ThemeType.System] = new ThemeInfo
            {
                Type = ThemeType.System,
                Name = "跟随系统",
                Description = "跟随系统设置自动切换",
                Category = ThemeCategory.System,
                Icon = "🔄",
                PreviewColor = "#5a67d8",
                IsDark = false,
                ResourcePath = ""
            };

            // 基础主题
            _themes[ThemeType.ModernBlue] = new ThemeInfo
            {
                Type = ThemeType.ModernBlue,
                Name = "现代蓝色",
                Description = "专业、清新的蓝色主题，适合长时间工作",
                Category = ThemeCategory.Basic,
                Icon = "💙",
                PreviewColor = "#4f7cff",
                IsDark = false,
                ResourcePath = "avares://AuroraUI/Modules/Theme/Resources/Extended/ModernBlue/Theme.axaml"
            };

            _themes[ThemeType.DarkProfessional] = new ThemeInfo
            {
                Type = ThemeType.DarkProfessional,
                Name = "专业深色",
                Description = "经典深色主题，减少眼部疲劳，适合夜间工作",
                Category = ThemeCategory.Basic,
                Icon = "🌃",
                PreviewColor = "#64ffda",
                IsDark = true,
                ResourcePath = "avares://AuroraUI/Modules/Theme/Resources/Extended/DarkProfessional/Theme.axaml"
            };

            _themes[ThemeType.NatureGreen] = new ThemeInfo
            {
                Type = ThemeType.NatureGreen,
                Name = "自然绿色",
                Description = "清新自然的绿色主题，营造宁静专注的工作环境",
                Category = ThemeCategory.Basic,
                Icon = "🌿",
                PreviewColor = "#27ae60",
                IsDark = false,
                ResourcePath = "avares://AuroraUI/Modules/Theme/Resources/Extended/NatureGreen/Theme.axaml"
            };

            // 专业主题
            _themes[ThemeType.OceanBlue] = new ThemeInfo
            {
                Type = ThemeType.OceanBlue,
                Name = "深海蓝色",
                Description = "深邃海洋主题，沉稳专业，适合长期专注工作",
                Category = ThemeCategory.Professional,
                Icon = "🌊",
                PreviewColor = "#006ba6",
                IsDark = false,
                ResourcePath = "avares://AuroraUI/Modules/Theme/Resources/Extended/OceanBlue/Theme.axaml"
            };

            _themes[ThemeType.CorporateGold] = new ThemeInfo
            {
                Type = ThemeType.CorporateGold,
                Name = "企业金色",
                Description = "高端企业风格，金色点缀，体现专业与品质",
                Category = ThemeCategory.Professional,
                Icon = "🏆",
                PreviewColor = "#b8860b",
                IsDark = false,
                ResourcePath = "avares://AuroraUI/Modules/Theme/Resources/Extended/CorporateGold/Theme.axaml"
            };

            _themes[ThemeType.MedicalClean] = new ThemeInfo
            {
                Type = ThemeType.MedicalClean,
                Name = "医疗洁净",
                Description = "医疗级洁净主题，简洁可靠，适合精密仪器操作界面",
                Category = ThemeCategory.Professional,
                Icon = "🏥",
                PreviewColor = "#0277bd",
                IsDark = false,
                ResourcePath = "avares://AuroraUI/Modules/Theme/Resources/Extended/MedicalClean/Theme.axaml"
            };

            // 特色主题
            _themes[ThemeType.CyberpunkNeon] = new ThemeInfo
            {
                Type = ThemeType.CyberpunkNeon,
                Name = "赛博朋克",
                Description = "未来科技风格，霓虹色彩，适合创新型工作环境",
                Category = ThemeCategory.Special,
                Icon = "🤖",
                PreviewColor = "#00ffff",
                IsDark = true,
                ResourcePath = "avares://AuroraUI/Modules/Theme/Resources/Extended/CyberpunkNeon/Theme.axaml"
            };

            _themes[ThemeType.ForestDark] = new ThemeInfo
            {
                Type = ThemeType.ForestDark,
                Name = "深林主题",
                Description = "深色森林主题，自然沉静，适合需要专注的深度工作",
                Category = ThemeCategory.Special,
                Icon = "🌲",
                PreviewColor = "#4caf50",
                IsDark = true,
                ResourcePath = "avares://AuroraUI/Modules/Theme/Resources/Extended/ForestDark/Theme.axaml"
            };

            _themes[ThemeType.RetroTerminal] = new ThemeInfo
            {
                Type = ThemeType.RetroTerminal,
                Name = "复古终端",
                Description = "经典终端风格，绿色磷光屏效果，怀旧极客风格",
                Category = ThemeCategory.Special,
                Icon = "💻",
                PreviewColor = "#00ff41",
                IsDark = true,
                ResourcePath = "avares://AuroraUI/Modules/Theme/Resources/Extended/RetroTerminal/Theme.axaml"
            };

            // 高级主题
            _themes[ThemeType.RoyalPurple] = new ThemeInfo
            {
                Type = ThemeType.RoyalPurple,
                Name = "皇家紫色",
                Description = "高贵典雅的紫色主题，彰显品味与格调",
                Category = ThemeCategory.Premium,
                Icon = "👑",
                PreviewColor = "#673ab7",
                IsDark = false,
                ResourcePath = "avares://AuroraUI/Modules/Theme/Resources/Extended/RoyalPurple/Theme.axaml"
            };

            _themes[ThemeType.SunsetOrange] = new ThemeInfo
            {
                Type = ThemeType.SunsetOrange,
                Name = "夕阳橙色",
                Description = "温暖活力的橙色主题，激发创造力和热情",
                Category = ThemeCategory.Premium,
                Icon = "🌅",
                PreviewColor = "#ff6b35",
                IsDark = false,
                ResourcePath = "avares://AuroraUI/Modules/Theme/Resources/Extended/SunsetOrange/Theme.axaml"
            };

            _themes[ThemeType.ArcticWhite] = new ThemeInfo
            {
                Type = ThemeType.ArcticWhite,
                Name = "极地白色",
                Description = "极简纯净的白色主题，最大化内容可读性",
                Category = ThemeCategory.Premium,
                Icon = "❄️",
                PreviewColor = "#2196f3",
                IsDark = false,
                ResourcePath = "avares://AuroraUI/Modules/Theme/Resources/Extended/ArcticWhite/Theme.axaml"
            };

            // 无障碍主题
            _themes[ThemeType.HighContrast] = new ThemeInfo
            {
                Type = ThemeType.HighContrast,
                Name = "高对比度",
                Description = "高对比度主题，提高可读性，适合视力敏感用户",
                Category = ThemeCategory.Accessibility,
                Icon = "🔍",
                PreviewColor = "#0066ff",
                IsDark = false,
                ResourcePath = "avares://AuroraUI/Modules/Theme/Resources/Extended/HighContrast/Theme.axaml"
            };

            _themes[ThemeType.MinimalGrey] = new ThemeInfo
            {
                Type = ThemeType.MinimalGrey,
                Name = "极简灰色",
                Description = "简约优雅的灰色主题，专注内容，减少干扰",
                Category = ThemeCategory.Accessibility,
                Icon = "⚪",
                PreviewColor = "#5a67d8",
                IsDark = false,
                ResourcePath = "avares://AuroraUI/Modules/Theme/Resources/Extended/MinimalGrey/Theme.axaml"
            };
        }
    }

    /// <summary>
    /// 主题分类信息
    /// </summary>
    public class ThemeCategoryInfo
    {
        public ThemeCategory Category { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
