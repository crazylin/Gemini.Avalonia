using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Avalonia.Input;
using AuroraUI.Framework.Commands;
using AuroraUI.Demo.ViewModels;
using AuroraUI.Demo.Views;
using AuroraUI.Demo.Controls;
using AuroraUI.Framework;
using AuroraUI.Framework.Services;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using Avalonia.LogicalTree;

namespace AuroraUI.Demo.Commands
{
    #region 文本编辑命令定义

    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class UndoCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Edit.Undo";
        
        public override string Name => CommandName;
        public override string Text => LocalizationService?.GetString("Command.Edit.Undo");
        public override string ToolTip => LocalizationService?.GetString("Command.Edit.Undo.ToolTip");
        public override Uri IconSource => new Uri("avares://AuroraUI.Demo/Assets/Icons/undo.svg");
    }
    
    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class RedoCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Edit.Redo";
        
        public override string Name => CommandName;
        public override string Text => LocalizationService?.GetString("Command.Edit.Redo");
        public override string ToolTip => LocalizationService?.GetString("Command.Edit.Redo.ToolTip");
        public override Uri IconSource => new Uri("avares://AuroraUI.Demo/Assets/Icons/redo.svg");
    }

    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class CutCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Edit.Cut";
        
        public override string Name => CommandName;
        public override string Text => LocalizationService?.GetString("Command.Edit.Cut");
        public override string ToolTip => LocalizationService?.GetString("Command.Edit.Cut.ToolTip");
        public override Uri IconSource => new Uri("avares://AuroraUI.Demo/Assets/Icons/cut.svg");
    }

    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class CopyCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Edit.Copy";
        
        public override string Name => CommandName;
        public override string Text => LocalizationService?.GetString("Command.Edit.Copy");
        public override string ToolTip => LocalizationService?.GetString("Command.Edit.Copy.ToolTip");
        public override Uri IconSource => new Uri("avares://AuroraUI.Demo/Assets/Icons/copy.svg");
    }

    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class PasteCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Edit.Paste";
        
        public override string Name => CommandName;
        public override string Text => LocalizationService?.GetString("Command.Edit.Paste");
        public override string ToolTip => LocalizationService?.GetString("Command.Edit.Paste.ToolTip");
        public override Uri IconSource => new Uri("avares://AuroraUI.Demo/Assets/Icons/paste.svg");
    }

    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class SelectAllCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Edit.SelectAll";
        
        public override string Name => CommandName;
        public override string Text => LocalizationService?.GetString("Command.Edit.SelectAll");
        public override string ToolTip => LocalizationService?.GetString("Command.Edit.SelectAll.ToolTip");
        public override Uri IconSource => new Uri("avares://AuroraUI.Demo/Assets/Icons/select-all.svg");
    }

    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class FindCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Edit.Find";
        
        public override string Name => CommandName;
        public override string Text => LocalizationService?.GetString("Command.Edit.Find");
        public override string ToolTip => LocalizationService?.GetString("Command.Edit.Find.ToolTip");
        public override Uri IconSource => new Uri("avares://AuroraUI.Demo/Assets/Icons/search.svg");
    }

    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class ReplaceCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Edit.Replace";
        
        public override string Name => CommandName;
        public override string Text => LocalizationService?.GetString("Command.Edit.Replace");
        public override string ToolTip => LocalizationService?.GetString("Command.Edit.Replace.ToolTip");
        public override Uri IconSource => new Uri("avares://AuroraUI.Demo/Assets/Icons/replace.svg");
    }

    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class GoToLineCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Edit.GoToLine";
        
        public override string Name => CommandName;
        public override string Text => LocalizationService?.GetString("Command.Edit.GoToLine");
        public override string ToolTip => LocalizationService?.GetString("Command.Edit.GoToLine.ToolTip");
        public override Uri IconSource => new Uri("avares://AuroraUI.Demo/Assets/Icons/go-to-line.svg");
    }

    // 文件操作命令
    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class SaveDocumentCommandDefinition : CommandDefinition
    {
        public const string CommandName = "File.Save";
        
        public override string Name => CommandName;
        public override string Text => LocalizationService?.GetString("Command.File.Save");
        public override string ToolTip => LocalizationService?.GetString("Command.File.Save.ToolTip");
        public override Uri IconSource => new Uri("avares://AuroraUI.Demo/Assets/Icons/save.svg");
    }

    [Export(typeof(CommandDefinitionBase))]
    [CommandDefinition]
    public class SaveAsDocumentCommandDefinition : CommandDefinition
    {
        public const string CommandName = "File.SaveAs";
        
        public override string Name => CommandName;
        public override string Text => LocalizationService?.GetString("Command.File.SaveAs");
        public override string ToolTip => LocalizationService?.GetString("Command.File.SaveAs.ToolTip");
        public override Uri IconSource => new Uri("avares://AuroraUI.Demo/Assets/Icons/save-as.svg");
    }

    #endregion

    #region 文本编辑命令处理器

    [Export(typeof(ICommandHandler))]
    public class UndoCommandHandler : CommandHandlerBase<UndoCommandDefinition>
    {
        public override Task Run(Command command)
        {
            var textEditor = TextEditorHelper.GetActiveTextEditor();
            if (textEditor != null)
            {
                textEditor.Undo();
            }
            return Task.CompletedTask;
        }

        public override void Update(Command command)
        {
            var textEditor = TextEditorHelper.GetActiveTextEditor();
            command.Enabled = textEditor?.CanUndo == true;
        }
    }

    [Export(typeof(ICommandHandler))]
    public class RedoCommandHandler : CommandHandlerBase<RedoCommandDefinition>
    {
        public override Task Run(Command command)
        {
            var textEditor = TextEditorHelper.GetActiveTextEditor();
            if (textEditor != null)
            {
                textEditor.Redo();
            }
            return Task.CompletedTask;
        }

        public override void Update(Command command)
        {
            var textEditor = TextEditorHelper.GetActiveTextEditor();
            command.Enabled = textEditor?.CanRedo == true;
        }
    }

    [Export(typeof(ICommandHandler))]
    public class CutCommandHandler : CommandHandlerBase<CutCommandDefinition>
    {
        public override async Task Run(Command command)
        {
            var textEditor = TextEditorHelper.GetActiveTextEditor();
            if (textEditor != null)
            {
                await textEditor.Cut();
            }
        }

        public override void Update(Command command)
        {
            var textEditor = TextEditorHelper.GetActiveTextEditor();
            command.Enabled = textEditor?.HasSelection == true;
        }
    }

    [Export(typeof(ICommandHandler))]
    public class CopyCommandHandler : CommandHandlerBase<CopyCommandDefinition>
    {
        public override async Task Run(Command command)
        {
            var textEditor = TextEditorHelper.GetActiveTextEditor();
            if (textEditor != null)
            {
                await textEditor.Copy();
            }
        }

        public override void Update(Command command)
        {
            var textEditor = TextEditorHelper.GetActiveTextEditor();
            command.Enabled = textEditor?.HasSelection == true;
        }
    }

    [Export(typeof(ICommandHandler))]
    public class PasteCommandHandler : CommandHandlerBase<PasteCommandDefinition>
    {
        public override async Task Run(Command command)
        {
            var textEditor = TextEditorHelper.GetActiveTextEditor();
            if (textEditor != null)
            {
                await textEditor.Paste();
            }
        }

        public override void Update(Command command)
        {
            var textEditor = TextEditorHelper.GetActiveTextEditor();
            command.Enabled = textEditor?.CanPaste == true;
        }
    }

    [Export(typeof(ICommandHandler))]
    public class SelectAllCommandHandler : CommandHandlerBase<SelectAllCommandDefinition>
    {
        public override Task Run(Command command)
        {
            var textEditor = TextEditorHelper.GetActiveTextEditor();
            if (textEditor != null)
            {
                textEditor.SelectAll();
            }
            return Task.CompletedTask;
        }

        public override void Update(Command command)
        {
            var textEditor = TextEditorHelper.GetActiveTextEditor();
            command.Enabled = textEditor != null && !string.IsNullOrEmpty(textEditor.Text);
        }
    }

    [Export(typeof(ICommandHandler))]
    public class FindCommandHandler : CommandHandlerBase<FindCommandDefinition>
    {
        public override Task Run(Command command)
        {
            var textEditor = TextEditorHelper.GetActiveTextEditor();
            if (textEditor != null)
            {
                textEditor.ShowFindDialog();
            }
            return Task.CompletedTask;
        }

        public override void Update(Command command)
        {
            var textEditor = TextEditorHelper.GetActiveTextEditor();
            command.Enabled = textEditor != null;
        }
    }

    [Export(typeof(ICommandHandler))]
    public class ReplaceCommandHandler : CommandHandlerBase<ReplaceCommandDefinition>
    {
        public override Task Run(Command command)
        {
            var textEditor = TextEditorHelper.GetActiveTextEditor();
            if (textEditor != null)
            {
                textEditor.ShowReplaceDialog();
            }
            return Task.CompletedTask;
        }

        public override void Update(Command command)
        {
            var textEditor = TextEditorHelper.GetActiveTextEditor();
            command.Enabled = textEditor != null;
        }
    }

    [Export(typeof(ICommandHandler))]
    public class GoToLineCommandHandler : CommandHandlerBase<GoToLineCommandDefinition>
    {
        public override Task Run(Command command)
        {
            var textEditor = TextEditorHelper.GetActiveTextEditor();
            if (textEditor != null)
            {
                textEditor.ShowGoToLineDialog();
            }
            return Task.CompletedTask;
        }

        public override void Update(Command command)
        {
            var textEditor = TextEditorHelper.GetActiveTextEditor();
            command.Enabled = textEditor != null;
        }
    }

    // 文件操作命令处理器
    [Export(typeof(ICommandHandler))]
    public class SaveDocumentCommandHandler : CommandHandlerBase<SaveDocumentCommandDefinition>
    {
        public override async Task Run(Command command)
        {
            var documentViewModel = DocumentHelper.GetActiveDocument();
            if (documentViewModel?.SaveCommand.CanExecute(null) == true)
            {
                await Task.Run(() => documentViewModel.SaveCommand.Execute(null));
            }
        }

        public override void Update(Command command)
        {
            var documentViewModel = DocumentHelper.GetActiveDocument();
            command.Enabled = documentViewModel?.IsDirty == true;
        }
    }

    [Export(typeof(ICommandHandler))]
    public class SaveAsDocumentCommandHandler : CommandHandlerBase<SaveAsDocumentCommandDefinition>
    {
        public override async Task Run(Command command)
        {
            var documentViewModel = DocumentHelper.GetActiveDocument();
            if (documentViewModel?.SaveAsCommand.CanExecute(null) == true)
            {
                await Task.Run(() => documentViewModel.SaveAsCommand.Execute(null));
            }
        }

        public override void Update(Command command)
        {
            var documentViewModel = DocumentHelper.GetActiveDocument();
            command.Enabled = documentViewModel != null;
        }
    }

    #endregion

    #region 辅助方法

    public static class TextEditorHelper
    {
        public static ITextEditor? GetActiveTextEditor()
        {
            try
            {
                // 获取当前活动窗口中的文本编辑器
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    var mainWindow = desktop.MainWindow;
                    if (mainWindow?.DataContext is IShell shell)
                    {
                        var activeDocument = shell.ActiveItem;
                        if (activeDocument is SampleDocumentViewModel documentViewModel)
                        {
                            // 通过视图查找文本编辑器控件
                            return FindTextEditorInDocument(documentViewModel, mainWindow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"获取活动文本编辑器失败: {ex.Message}");
            }
            
            return null;
        }

        private static ITextEditor? FindTextEditorInDocument(SampleDocumentViewModel documentViewModel, Control parentControl)
        {
            try
            {
                // 查找与文档关联的SampleDocumentView
                var documentView = FindControlRecursive<SampleDocumentView>(parentControl, 
                    view => view.DataContext == documentViewModel);
                
                if (documentView != null)
                {
                    // 获取视图中的EnhancedTextEditor
                    return documentView.EditorControl;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"查找文本编辑器控件失败: {ex.Message}");
            }
            
            return null;
        }
        
        private static T? FindControlRecursive<T>(Control parent, Func<T, bool>? predicate = null) 
            where T : Control
        {
            if (parent is T target && (predicate == null || predicate(target)))
            {
                return target;
            }

            foreach (var child in parent.GetLogicalChildren())
            {
                if (child is Control control)
                {
                    var result = FindControlRecursive<T>(control, predicate);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }
    }

    /// <summary>
    /// 文本编辑器接口
    /// </summary>
    public interface ITextEditor
    {
        string? Text { get; set; }
        bool HasSelection { get; }
        bool CanUndo { get; }
        bool CanRedo { get; }
        bool CanPaste { get; }
        
        void Undo();
        void Redo();
        Task Cut();
        Task Copy();
        Task Paste();
        void SelectAll();
        void ShowFindDialog();
        void ShowReplaceDialog();
        void ShowGoToLineDialog();
    }

    #endregion

    #region 辅助方法类

    /// <summary>
    /// 文档帮助类
    /// </summary>
    public static class DocumentHelper
    {
        public static SampleDocumentViewModel? GetActiveDocument()
        {
            try
            {
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    var mainWindow = desktop.MainWindow;
                    if (mainWindow?.DataContext is IShell shell)
                    {
                        return shell.ActiveItem as SampleDocumentViewModel;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"获取活动文档失败: {ex.Message}");
            }
            
            return null;
        }
    }

    #endregion
}

// 扩展CommandHandlerBase以包含通用的文本编辑器获取方法
namespace AuroraUI.Demo.Commands
{
    public abstract class TextEditCommandHandlerBase<T> : CommandHandlerBase<T>
        where T : CommandDefinition
    {
        protected ITextEditor? GetActiveTextEditor()
        {
            return TextEditorHelper.GetActiveTextEditor();
        }
    }
}
