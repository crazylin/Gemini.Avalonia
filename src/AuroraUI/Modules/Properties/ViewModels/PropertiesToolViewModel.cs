using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using AuroraUI.Framework;
using AuroraUI.Framework.Services;
using AuroraUI.Services;
using ReactiveUI;

namespace AuroraUI.Modules.Properties.ViewModels
{
    /// <summary>
    /// 属性工具视图模型
    /// </summary>
    [Export(typeof(ITool))]
    [Export(typeof(PropertiesToolViewModel))]
    public class PropertiesToolViewModel : Tool
    {
        private object? _selectedObject;
        
        /// <summary>
        /// 本地化服务
        /// </summary>
        [Import]
        public ILocalizationService LocalizationService { get; set; } = null!;
        
        /// <summary>
        /// 首选位置
        /// </summary>
        public override PaneLocation PreferredLocation => PaneLocation.Right;
        
        /// <summary>
        /// 首选宽度
        /// </summary>
        public override double PreferredWidth => 300;
        
        /// <summary>
        /// 首选高度
        /// </summary>
        public override double PreferredHeight => 400;
        
        /// <summary>
        /// 选中的对象
        /// </summary>
        public object? SelectedObject
        {
            get => _selectedObject;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedObject, value);
                UpdateProperties();
            }
        }
        
        /// <summary>
        /// 属性集合
        /// </summary>
        public ObservableCollection<PropertyItem> Properties { get; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public PropertiesToolViewModel()
        {
            Properties = new ObservableCollection<PropertyItem>();
            
            // 监听活动文档变化
            // 注意：这里需要在实际应用中通过Shell服务获取活动文档
        }
        
        /// <summary>
        /// 显示名称
        /// </summary>
        // 注意：实际绑定请使用基类 Tool.DisplayName，这里提供一个便于调试/复用的本地化只读属性，避免与基类同名属性冲突
        public string LocalizedDisplayName => LocalizationService?.GetString("Properties.Title");
        
        public string NoObjectSelectedText => LocalizationService?.GetString("Properties.NoObjectSelected") ;
        
        public string SelectObjectHintText => LocalizationService?.GetString("Properties.SelectObjectHint");
        
        /// <summary>
        /// 初始化显示名称（在MEF注入完成后调用）
        /// </summary>
        public override void Initialize()
        {
            // 设置标题与工具提示（使用基类的可通知属性）
            DisplayName = LocalizationService?.GetString("Properties.Title");
            ToolTip = LocalizationService?.GetString("Properties.ToolTip");
        }
        
        /// <summary>
        /// 更新属性列表
        /// </summary>
        private void UpdateProperties()
        {
            Properties.Clear();
            
            if (SelectedObject == null)
                return;
                
            var type = SelectedObject.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .OrderBy(p => p.Name);
                
            foreach (var property in properties)
            {
                try
                {
                    var value = property.GetValue(SelectedObject);
                    var propertyItem = new PropertyItem
                    {
                        Name = property.Name,
                        Value = value?.ToString() ?? "<null>",
                        Type = property.PropertyType.Name,
                        IsReadOnly = !property.CanWrite,
                        Property = property,
                        Target = SelectedObject
                    };
                    
                    Properties.Add(propertyItem);
                }
                catch (Exception ex)
                {
                    var errorItem = new PropertyItem
                    {
                        Name = property.Name,
                        Value = $"<错误: {ex.Message}>",
                        Type = property.PropertyType.Name,
                        IsReadOnly = true
                    };
                    
                    Properties.Add(errorItem);
                }
            }
        }
        
        /// <summary>
        /// 设置选中对象（从Shell服务）
        /// </summary>
        /// <param name="shell">Shell服务</param>
        public void SetSelectedObjectFromShell(IShell shell)
        {
            // 获取当前活动的文档或工具
            var activeDocument = shell.ActiveLayoutItem as IDocument;
            if (activeDocument != null)
            {
                SelectedObject = activeDocument;
                return;
            }
            
            var activeTool = shell.ActiveLayoutItem as ITool;
            if (activeTool != null)
            {
                SelectedObject = activeTool;
                return;
            }
            
            SelectedObject = null;
        }
        
        /// <summary>
        /// 加载状态
        /// </summary>
        public void LoadState(BinaryReader reader)
        {
            // 属性工具通常不需要保存状态，因为它显示的是当前选中对象的属性
        }
        
        /// <summary>
        /// 保存状态
        /// </summary>
        public void SaveState(BinaryWriter writer)
        {
            // 属性工具通常不需要保存状态
        }
    }
    
    /// <summary>
    /// 属性项
    /// </summary>
    public class PropertyItem : ReactiveObject
    {
        private string _name = string.Empty;
        private string _value = string.Empty;
        private string _type = string.Empty;
        private bool _isReadOnly;
        
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        
        /// <summary>
        /// 属性值
        /// </summary>
        public string Value
        {
            get => _value;
            set
            {
                this.RaiseAndSetIfChanged(ref _value, value);
                if (!IsReadOnly)
                {
                    UpdateTargetProperty();
                }
            }
        }
        
        /// <summary>
        /// 属性类型
        /// </summary>
        public string Type
        {
            get => _type;
            set => this.RaiseAndSetIfChanged(ref _type, value);
        }
        
        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => this.RaiseAndSetIfChanged(ref _isReadOnly, value);
        }
        
        /// <summary>
        /// 反射属性信息
        /// </summary>
        public PropertyInfo? Property { get; set; }
        
        /// <summary>
        /// 目标对象
        /// </summary>
        public object? Target { get; set; }
        
        /// <summary>
        /// 更新目标属性值
        /// </summary>
        private void UpdateTargetProperty()
        {
            if (Property == null || Target == null || !Property.CanWrite)
                return;
                
            try
            {
                // 尝试转换值类型
                var convertedValue = ConvertValue(Value, Property.PropertyType);
                Property.SetValue(Target, convertedValue);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"设置属性值失败: {ex.Message}");
                // 恢复原值
                try
                {
                    var originalValue = Property.GetValue(Target);
                    _value = originalValue?.ToString() ?? "<null>";
                    this.RaisePropertyChanged(nameof(Value));
                }
                catch
                {
                    // 忽略恢复失败
                }
            }
        }
        
        /// <summary>
        /// 转换值类型
        /// </summary>
        private object? ConvertValue(string value, Type targetType)
        {
            if (string.IsNullOrEmpty(value) && !targetType.IsValueType)
                return null;
                
            if (targetType == typeof(string))
                return value;
                
            // 处理可空类型
            var underlyingType = Nullable.GetUnderlyingType(targetType);
            if (underlyingType != null)
            {
                if (string.IsNullOrEmpty(value))
                    return null;
                targetType = underlyingType;
            }
            
            // 使用TypeConverter进行转换
            var converter = TypeDescriptor.GetConverter(targetType);
            if (converter.CanConvertFrom(typeof(string)))
            {
                return converter.ConvertFromString(value);
            }
            
            // 回退到Convert.ChangeType
            return Convert.ChangeType(value, targetType);
        }
    }
}