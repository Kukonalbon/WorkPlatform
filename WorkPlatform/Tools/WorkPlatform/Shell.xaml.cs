using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PlatformCommon.Manager;
using PlatformCommon.Manager.LayoutMgr;
using PlatformCommon.Updata;
using Jisons;
using System.Windows.Markup;
using System.IO;
using System.Xml;
using Project.BusinessFacade;
using ClientManager;

namespace WorkPlatform
{
    /// <summary>
    /// Shell.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IShell))]
    public partial class Shell : Window, IShell
    {
        public Shell()
        {
            InitializeComponent();


            //ManagerUpdata.Instance.WriteUpdataData();

            this.Loaded += Shell_Loaded;

            this.SizeChanged += Shell_SizeChanged;

            //string ip = "10.237.3.255";
            //var b = WorkClient.Instance.RegisterServerNode(ip, 12000);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            GlobalEvent.Instance.RaiseProjectChange(new PlatformCommon.Events.ProjectEventArgs() { Action = PlatformCommon.Events.ProjectAction.Close });
        }

        void Shell_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.maintool.Height = this.gridMenu.ActualHeight - this.bottomtoolbar.ActualHeight;
            this.maintool.Width = this.gridMenu.ActualWidth;
        }

        void Shell_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
