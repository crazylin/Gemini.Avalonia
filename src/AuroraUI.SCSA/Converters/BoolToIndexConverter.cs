using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace SCSA.Converters;

/// <summary>
/// 布尔值到ComboBox索引转换器
/// </summary>
public class BoolToIndexConverter : IValueConverter
{
    public static readonly BoolToIndexConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return boolValue ? 1 : 0; // false->0(Off), true->1(On)
        return 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int index)
            return index == 1; // 0->false(Off), 1->true(On)
        return false;
    }
}
