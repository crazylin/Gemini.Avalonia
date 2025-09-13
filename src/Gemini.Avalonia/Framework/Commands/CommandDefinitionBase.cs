
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using Gemini.Avalonia.Framework.Services;
using ReactiveUI;
using Gemini.Avalonia.Framework;

namespace Gemini.Avalonia.Framework.Commands
{
    public abstract class CommandDefinitionBase
    {
        protected ILocalizationService? _localizationService;

        [Import(AllowDefault = true)]
        public ILocalizationService? LocalizationService
        {
            get 
            {
                // 如果MEF注入的服务为空，尝试从IoC容器获取
                if (_localizationService == null)
                {
                    try
                    {
                        _localizationService = IoC.Get<ILocalizationService>();
                    }
                    catch
                    {
                        // 忽略获取失败的情况
                    }
                }
                return _localizationService;
            }
            set
            {
                _localizationService = value;
            }
        }



        public abstract string Name { get; }
        public abstract string Text { get; }
        public abstract string ToolTip { get; }
        public abstract Uri IconSource { get; }
        public abstract string IconName { get; }
        public abstract bool IsList { get; }
    }
}