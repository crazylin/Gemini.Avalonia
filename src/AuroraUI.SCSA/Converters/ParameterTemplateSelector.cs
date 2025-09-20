using Avalonia.Controls;
using Avalonia.Controls.Templates;
using SCSA.Models;

namespace SCSA.Converters;

/// <summary>
/// 参数模板选择器 - 选择合适的DataTemplate
/// </summary>
public class ParameterTemplateSelector : IDataTemplate
{
    public IDataTemplate? EnumParameterTemplate { get; set; }
    public IDataTemplate? BoolParameterTemplate { get; set; }
    public IDataTemplate? IntegerParameterTemplate { get; set; }
    public IDataTemplate? FloatParameterTemplate { get; set; }
    public IDataTemplate? NumberParameterTemplate { get; set; }

    public Control? Build(object? param)
    {
        return param switch
        {
            EnumParameter => EnumParameterTemplate?.Build(param),
            BoolParameter => BoolParameterTemplate?.Build(param),
            IntegerParameter => IntegerParameterTemplate?.Build(param),
            FloatParameter => FloatParameterTemplate?.Build(param),
            NumberParameter => NumberParameterTemplate?.Build(param),
            _ => NumberParameterTemplate?.Build(param) // 默认使用数值模板
        };
    }

    public bool Match(object? data)
    {
        return data is DeviceParameter;
    }
}
