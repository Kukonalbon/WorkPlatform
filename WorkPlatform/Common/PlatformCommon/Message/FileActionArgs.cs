using Jisons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PlatformCommon.Message
{

    public class FileActionArgs : EventArgs
    {
        public ControlType Type { get; set; }
        public FrameworkElement Control { get; set; }
    }
}
