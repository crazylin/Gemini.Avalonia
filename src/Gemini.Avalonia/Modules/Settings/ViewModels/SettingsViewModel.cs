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
using Gemini.Avalonia.Framework;
using Gemini.Avalonia.Framework.Services;
using Gemini.Avalonia.Services;

namespace Gemini.Avalonia.Modules.Settings.ViewModels
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

        public string DisplayName => LocalizationService?.GetString("Settings.Title");
        
        public string OkButtonText => LocalizationService?.GetString("Common.OK");
        
        public string CancelButtonText => LocalizationService?.GetString("Common.Cancel");

        public SettingsViewModel()
        {
            Pages = new List<SettingsPageViewModel>();
        }

        public async Task Initialize()
        {
            // 语言切换改为重启模式，不再订阅CultureChanged事件

            // 使用IoC容器获取所有设置编辑器
            var syncEditors = IoC.GetAll<ISettingsEditor>();
            var asyncEditors = IoC.GetAll<ISettingsEditorAsync>();
            
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