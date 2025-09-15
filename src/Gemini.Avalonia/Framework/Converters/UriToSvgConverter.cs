using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Svg.Skia;

namespace Gemini.Avalonia.Framework.Converters
{
    /// <summary>
    /// Uri到SVG图像转换器
    /// </summary>
    public class UriToSvgConverter : IValueConverter
    {
        public static readonly UriToSvgConverter Instance = new();
        
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string? uriString = null;
            
            // 处理Uri或string类型
            if (value is Uri uri)
            {
                uriString = uri.ToString();
            }
            else if (value is string str && !string.IsNullOrEmpty(str))
            {
                uriString = str;
            }
            
            if (!string.IsNullOrEmpty(uriString))
            {
                try
                {
                    var svgImage = new SvgImage();
                    svgImage.Source = SvgSource.Load(uriString, null);
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