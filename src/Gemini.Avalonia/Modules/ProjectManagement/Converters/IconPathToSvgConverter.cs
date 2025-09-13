using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Svg.Skia;

namespace Gemini.Avalonia.Modules.ProjectManagement.Converters
{
    /// <summary>
    /// 图标路径到SVG图像转换器
    /// </summary>
    public class IconPathToSvgConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string iconPath && !string.IsNullOrEmpty(iconPath))
            {
                try
                {
                    var svgImage = new SvgImage();
                    svgImage.Source = SvgSource.Load(iconPath, null);
                    return svgImage;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}