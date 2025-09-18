using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Svg.Skia;

namespace AuroraUI.Framework.Converters
{
    /// <summary>
    /// Uri到Image控件转换器，专门用于MenuItem.Icon
    /// </summary>
    public class UriToImageConverter : IValueConverter
    {
        public static readonly UriToImageConverter Instance = new();
        
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null; // 没有图标时返回null
            }
            
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
                    var image = new Image
                    {
                        Width = 16,
                        Height = 16,
                        Stretch = Stretch.Uniform
                    };

                    if (uriString.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                    {
                        var svgImage = new SvgImage();
                        svgImage.Source = SvgSource.Load(uriString, null);
                        image.Source = svgImage;
                    }
                    else // 假设是位图，如PNG
                    {
                        var bitmap = new Bitmap(AssetLoader.Open(new Uri(uriString)));
                        image.Source = bitmap;
                    }
                    
                    return image;
                }
                catch (Exception)
                {
                    return null; // 加载失败时返回null
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
