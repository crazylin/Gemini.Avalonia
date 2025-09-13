namespace Gemini.Avalonia.Modules.Settings
{
    public interface ISettingsEditor
    {
        string SettingsPageName { get; }
        string SettingsPagePath { get; }
        
        /// <summary>
        /// 排序顺序，数值越小越靠前
        /// </summary>
        int SortOrder { get; }

        void ApplyChanges();
    }
}