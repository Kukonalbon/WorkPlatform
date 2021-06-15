using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlatformCommon.Plugin;

namespace PlatformCommon.Events
{

    public enum PluginAction
    {
        Add,
        Close,
        Show,
        Hide
    }

    public class PluginsEventArgs
    {
        public PluginAction Action { get; set; }

        public IPluginObject PluginObject { get; set; }

    }

    public class PluginsEvent : Microsoft.Practices.Prism.Events.CompositePresentationEvent<PluginsEventArgs>
    {

    }

}
