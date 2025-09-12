using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Gemini.Avalonia.Demo.Converters
{
    /// <summary>
    /// 布尔值到字符串转换器
    /// </summary>
    public class BoolToStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string paramString)
            {
                var parts = paramString.Split('|');
                if (parts.Length == 2)
                {
                    return boolValue ? parts[0] : parts[1];
                }
            }
            
            return value?.ToString() ?? string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    /// <summary>
    /// 布尔转换器静态类
    /// </summary>
    public static class BoolConverters
    {
        public static new readonly BoolToStringConverter ToString = new();
    }
}