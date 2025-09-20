using System.Collections.Generic;
using AuroraUI.Modules.Theme.Models;

namespace AuroraUI.Modules.Theme.Services
{
    /// <summary>
    /// 主题管理器接口
    /// </summary>
    public interface IThemeManager
    {
        /// <summary>
        /// 获取所有主题
        /// </summary>
        /// <returns>所有主题信息</returns>
        IEnumerable<ThemeInfo> GetAllThemes();

        /// <summary>
        /// 根据分类获取主题
        /// </summary>
        /// <param name="category">主题分类</param>
        /// <returns>指定分类的主题</returns>
        IEnumerable<ThemeInfo> GetThemesByCategory(ThemeCategory category);

        /// <summary>
        /// 获取主题信息
        /// </summary>
        /// <param name="themeType">主题类型</param>
        /// <returns>主题信息，如果不存在则返回null</returns>
        ThemeInfo? GetThemeInfo(ThemeType themeType);

        /// <summary>
        /// 检查主题是否可用
        /// </summary>
        /// <param name="themeType">主题类型</param>
        /// <returns>是否可用</returns>
        bool IsThemeAvailable(ThemeType themeType);

        /// <summary>
        /// 获取主题分类信息
        /// </summary>
        /// <returns>所有分类信息</returns>
        IEnumerable<ThemeCategoryInfo> GetCategories();
    }
}
