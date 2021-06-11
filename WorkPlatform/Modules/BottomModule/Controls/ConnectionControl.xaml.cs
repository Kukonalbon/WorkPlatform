using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Modules.ChatModule;
using WorkCommon.Events;
using WorkCommon.Manager;

namespace Modules.BottomModule
{
    /// <summary>
    /// ConnectionControl.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectionControl : UserControl
    {
        MainChat mainChat = new MainChat();

        public ConnectionControl()
        {
            InitializeComponent();
        }

        private void action_Click(object sender, RoutedEventArgs e)
        {
            var args = new PluginsEventArgs() { Action = PluginAction.Add, PluginObject = mainChat };
            GlobalEvent.Instance.EventAggregator.GetEvent<PluginsEvent>().Publish(args);
        }
    }
}
