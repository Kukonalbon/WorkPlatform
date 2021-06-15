using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace PlatformCommon.Plugin
{
    public interface IPluginObject
    {

        string PluginName { get; }

        ImageSource PluginIcon { get; }

        FrameworkElement Plugin { get; }

        PluginType Type { get; }

        event EventHandler Opening;

        event EventHandler Closing;

        event EventHandler Hiding;

        bool IsShow { get; set; }

        /// <summary> 声明是否为工具，不是是工具的就不显示在底部状态栏 </summary>
        bool IsTool { get; }
    }

    public enum PluginType
    {
        Window,
        Plugin
    }
}
