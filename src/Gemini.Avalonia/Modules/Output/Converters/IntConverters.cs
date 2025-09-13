using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Gemini.Avalonia.Modules.Output.Converters
{
    /// <summary>
    /// 整数转换器静态类
    /// </summary>
    public static class IntConverters
    {
        public static readonly FuncValueConverter<int, bool> Equal = 
            new FuncValueConverter<int, bool>(value => value == 0);
    }
}