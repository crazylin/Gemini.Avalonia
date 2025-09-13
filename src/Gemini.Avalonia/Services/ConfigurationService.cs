using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Gemini.Avalonia.Framework.Logging;

namespace Gemini.Avalonia.Services
{
    [Export(typeof(IConfigurationService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ConfigurationService : IConfigurationService
    {
        private readonly Dictionary<string, object> _settings = new();
        private readonly string _configFilePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public ConfigurationService()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "Gemini.Avalonia");
            Directory.CreateDirectory(appFolder);
            _configFilePath = Path.Combine(appFolder, "settings.json");

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public T GetValue<T>(string key, T defaultValue = default)
        {
            if (_settings.TryGetValue(key, out var value))
            {
                try
                {
                    if (value is JsonElement jsonElement)
                    {
                        return JsonSerializer.Deserialize<T>(jsonElement.GetRawText(), _jsonOptions);
                    }
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        public void SetValue<T>(string key, T value)
        {
            _settings[key] = value;
        }

        public async Task SaveAsync()
        {
            try
            {
                LogManager.Debug("开始保存配置到: {0}", _configFilePath);
                LogManager.Debug("当前设置数量: {0}", _settings.Count);
                
                foreach (var kvp in _settings)
                {
                    LogManager.Debug("设置项: {0} = {1}", kvp.Key, kvp.Value);
                }
                
                var json = JsonSerializer.Serialize(_settings, _jsonOptions);
                LogManager.Debug("序列化后的JSON: {0}", json);
                
                await File.WriteAllTextAsync(_configFilePath, json);
                LogManager.Info("配置文件保存成功");
            }
            catch (Exception ex)
            {
                // 记录错误，但不抛出异常
                LogManager.Error(ex, "保存配置失败");
            }
        }

        public async Task LoadAsync()
        {
            try
            {
                LogManager.Debug("开始加载配置从: {0}", _configFilePath);
                
                if (File.Exists(_configFilePath))
                {
                    LogManager.Debug("配置文件存在，开始读取");
                    
                    var json = await File.ReadAllTextAsync(_configFilePath);
                    LogManager.Debug("读取到的JSON内容: {0}", json);
                    
                    var settings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, _jsonOptions);
                    if (settings != null)
                    {
                        LogManager.Info("反序列化成功，设置项数量: {0}", settings.Count);
                        
                        _settings.Clear();
                        foreach (var kvp in settings)
                        {
                            _settings[kvp.Key] = kvp.Value;
                            LogManager.Debug("加载设置项: {0} = {1}", kvp.Key, kvp.Value);
                        }
                        
                        LogManager.Info("配置加载完成");
                    }
                    else
                    {
                        LogManager.Warning("反序列化结果为null");
                    }
                }
                else
                {
                    LogManager.Info("配置文件不存在: {0}", _configFilePath);
                }
            }
            catch (Exception ex)
            {
                // 记录错误，但不抛出异常
                LogManager.Error(ex, "加载配置失败");
            }
        }
    }
}