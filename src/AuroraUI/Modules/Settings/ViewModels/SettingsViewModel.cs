using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AuroraUI.Framework;
using AuroraUI.Framework.Services;
using AuroraUI.Framework.Logging;
using AuroraUI.Services;

namespace AuroraUI.Modules.Settings.ViewModels
{
    [Export(typeof(SettingsViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class SettingsViewModel : ObservableObject
    {
        private IEnumerable<ISettingsEditorAsync> _settingsEditors;
        
        [ObservableProperty]
        private List<SettingsPageViewModel> _pages;
        
        [ObservableProperty]
        private SettingsPageViewModel _selectedPage;
        
        /// <summary>
        /// 本地化服务
        /// </summary>
        [Import]
        public ILocalizationService? LocalizationService { get; set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        [ImportingConstructor]
        public SettingsViewModel(ILocalizationService localizationService)
        {
            LocalizationService = localizationService;
            Pages = new List<SettingsPageViewModel>();
        }

        public string DisplayName => LocalizationService?.GetString("Settings.Title");
        
        public string OkButtonText => LocalizationService?.GetString("Common.OK");
        
        public string CancelButtonText => LocalizationService?.GetString("Common.Cancel");


        public async Task Initialize()
        {
            // 语言切换改为重启模式，不再订阅CultureChanged事件

            // 使用IoC容器获取所有设置编辑器
            LogManager.Info("SettingsViewModel", "开始获取设置编辑器...");
            
            // 先尝试手动创建 ApplicationSettingsViewModel 来检查依赖项
            try
            {
                var appSettings = IoC.Get<ApplicationSettingsViewModel>();
                LogManager.Info("SettingsViewModel", $"手动创建 ApplicationSettingsViewModel 成功: {appSettings?.GetType().Name}");
            }
            catch (Exception ex)
            {
                LogManager.Error("SettingsViewModel", $"手动创建 ApplicationSettingsViewModel 失败: {ex.Message}");
                LogManager.Error("SettingsViewModel", $"异常详情: {ex}");
            }
            
            var syncEditors = IoC.GetAll<ISettingsEditor>();
            var asyncEditors = IoC.GetAll<ISettingsEditorAsync>();
            
            // 调试信息：输出找到的设置编辑器
            LogManager.Info("SettingsViewModel", $"找到 {syncEditors.Count()} 个同步设置编辑器");
            LogManager.Info("SettingsViewModel", $"找到 {asyncEditors.Count()} 个异步设置编辑器");
            
            foreach (var editor in syncEditors)
            {
                LogManager.Info("SettingsViewModel", $"同步设置编辑器: {editor.GetType().Name} - {editor.SettingsPageName}");
            }
            
            foreach (var editor in asyncEditors)
            {
                LogManager.Info("SettingsViewModel", $"异步设置编辑器: {editor.GetType().Name} - {editor.SettingsPageName}");
            }
            
            // 额外调试：检查MEF容器中的所有导出
            try
            {
                // 使用反射访问私有字段
                var iocType = typeof(IoC);
                var containerField = iocType.GetField("_container", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                var container = containerField?.GetValue(null) as System.ComponentModel.Composition.Hosting.CompositionContainer;
                
                if (container != null)
                {
                    var exports = container.Catalog.Parts;
                    LogManager.Info("SettingsViewModel", $"MEF容器中总共有 {exports.Count()} 个部件");
                    
                    var settingsExports = exports.Where(p => 
                        p.ExportDefinitions.Any(e => e.ContractName.Contains("ISettingsEditor")));
                    LogManager.Info("SettingsViewModel", $"其中 {settingsExports.Count()} 个部件导出 ISettingsEditor");
                    
                    foreach (var part in settingsExports)
                    {
                        LogManager.Info("SettingsViewModel", $"设置编辑器部件: {part.ToString()}");
                        foreach (var export in part.ExportDefinitions)
                        {
                            LogManager.Info("SettingsViewModel", $"  导出: {export.ContractName}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("SettingsViewModel", $"调试MEF容器失败: {ex.Message}");
            }
            
            _settingsEditors = asyncEditors.Concat(syncEditors.Select(e => new SettingsEditorWrapper(e)))
                .OrderBy(e => e.SortOrder)
                .ThenBy(e => e.SettingsPagePath)
                .ThenBy(e => e.SettingsPageName);

            // 构建设置页树并选择第一个叶子页
            var pages = BuildPages();
            Pages = pages;
            SelectedPage = GetFirstLeafPageRecursive(pages);
        }

        private List<SettingsPageViewModel> BuildPages()
        {
            var pages = new List<SettingsPageViewModel>();

            if (_settingsEditors == null)
                return pages;

            foreach (var settingsEditor in _settingsEditors)
            {
                var parentCollection = GetParentCollection(settingsEditor, pages);

                var page = parentCollection.FirstOrDefault(m => m.Name == settingsEditor.SettingsPageName);

                if (page == null)
                {
                    page = new SettingsPageViewModel { Name = settingsEditor.SettingsPageName };
                    parentCollection.Add(page);
                }

                page.Editors.Add(settingsEditor is SettingsEditorWrapper wrapper ? (object)wrapper.ViewModel : (object)settingsEditor);
            }

            return pages;
        }

        private static SettingsPageViewModel GetFirstLeafPageRecursive(List<SettingsPageViewModel> pages)
        {
            if (!pages.Any())
                return null;

            var firstPage = pages.First();
            if (!firstPage.Children.Any())
                return firstPage;

            return GetFirstLeafPageRecursive(firstPage.Children);
        }

        private List<SettingsPageViewModel> GetParentCollection(ISettingsEditorAsync settingsEditor,
            List<SettingsPageViewModel> pages)
        {
            if (string.IsNullOrEmpty(settingsEditor.SettingsPagePath))
            {
                return pages;
            }

            var path = settingsEditor.SettingsPagePath.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var pathElement in path)
            {
                var page = pages.FirstOrDefault(s => s.Name == pathElement);

                if (page == null)
                {
                    page = new SettingsPageViewModel { Name = pathElement };
                    pages.Add(page);
                }

                pages = page.Children;
            }

            return pages;
        }

        [RelayCommand]
        public async Task SaveChanges()
        {
            foreach (var settingsEditor in _settingsEditors)
            {
                await settingsEditor.ApplyChangesAsync();
            }
        }

        [RelayCommand]
        public Task Cancel()
        {
            return Task.CompletedTask;
        }

        public async Task ShowDialog()
        {
            await Initialize();
            
            var settingsWindow = new Views.SettingsWindow
            {
                DataContext = this
            };
            
            // 使用ShowDialog显示模态对话框
            await settingsWindow.ShowDialog(Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null);
        }
    }
}