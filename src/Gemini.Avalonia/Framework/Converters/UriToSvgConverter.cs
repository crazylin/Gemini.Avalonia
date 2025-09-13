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
            if (value is Uri uri)
            {
                try
                {
                    var svgImage = new SvgImage();
                    svgImage.Source = SvgSource.Load(uri.ToString(), null);
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