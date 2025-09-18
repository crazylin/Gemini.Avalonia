using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace SCSA.Converters;

/// <summary>
/// 布尔值转换器集合
/// </summary>
public static class BoolConverters
{
    /// <summary>
    /// 布尔值转是否文本转换器
    /// </summary>
    public static readonly IValueConverter ToYesNo = new FuncValueConverter<bool, string>(value => value ? "是" : "否");

    /// <summary>
    /// 布尔值转成功/错误颜色画刷转换器
    /// </summary>
    public static readonly IValueConverter ToSuccessErrorBrush = new FuncValueConverter<bool, IBrush>(value =>
        value ? Brushes.Green : Brushes.Red);
}

/// <summary>
/// 对象转换器集合
/// </summary>
public static class ObjectConverters
{
    /// <summary>
    /// 对象是否不为空转换器
    /// </summary>
    public static readonly IValueConverter IsNotNull = new FuncValueConverter<object?, bool>(value => value != null);

    /// <summary>
    /// 对象是否为空转换器
    /// </summary>
    public static readonly IValueConverter IsNull = new FuncValueConverter<object?, bool>(value => value == null);
}
