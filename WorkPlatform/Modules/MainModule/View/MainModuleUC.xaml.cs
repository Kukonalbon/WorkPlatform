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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PlatformCommon.Behaviors;
using PlatformCommon.Manager;
using Jisons;
using PlatformCommon.Plugin;
using ControlLib;
using PlatformCommon.Updata;
using PlatformCommon.Events;

namespace Modules.MainModule
{
    /// <summary>
    /// MainToolUC.xaml 的交互逻辑
    /// </summary>
    /// 
    /// <summary>
    /// BottomToolBarUC.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.MainTool)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class MainModuleUC : UserControl
    {
        public List<PluginWindow> PluginWindows = new List<PluginWindow>();

        [Import]
        public MainModuleViewModel ViewModel
        {
            get
            {
                return this.DataContext as MainModuleViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        public MainModuleUC()
        {
            InitializeComponent();

            this.Loaded += MainToolUC_Loaded;
        }

        void MainToolUC_Loaded(object sender, RoutedEventArgs e)
        {
            var itemscontrol = this.FindVisualParent<ItemsControl>();
            itemscontrol.SizeChanged += ItemsControl_SizeChanged;
            actionview.Height = itemscontrol.ActualHeight - 10;
            actionview.Width = itemscontrol.ActualWidth - 10;

            if (ViewModel.PluginObjects != null)
            {
                foreach (var iplugin in this.ViewModel.PluginObjects)
                {
                    actionview.AddDragControl(iplugin);
                }
            }

            this.actionview.AddControlEvent += actionview_AddControlEvent;

            ContextMenu cmenu = new ContextMenu();

            MenuItem menuItem = new MenuItem();
            menuItem.Header = "名称排列";
            menuItem.Click += menuItem_Click;

            MenuItem menuItemAlign = new MenuItem();
            menuItemAlign.Header = "对齐排列";
            menuItemAlign.Click += menuItemAlign_Click;

            MenuItem menuItemUpdata = new MenuItem();
            menuItemUpdata.Header = "检查更新";
            menuItemUpdata.Click += menuItemUpdata_Click;

            //cmenu.Items.Add(menuItemUpdata);
            cmenu.Items.Add(menuItem);
            cmenu.Items.Add(menuItemAlign);
            this.ContextMenu = cmenu;

            //this.ViewModel.PluginObjects.CollectionChanged += PluginObjects_CollectionChanged;

            GlobalEvent.Instance.EventAggregator.GetEvent<PluginsEvent>().Subscribe(PluginsEventChanged);
            //GlobalEvent.Instance.EventAggregator.RaisePluginChange(new PluginsEventArgs() { Action = PluginAction.Add, PluginObject = this.Context });
        }

        #region 右键菜单

        void menuItemUpdata_Click(object sender, RoutedEventArgs e)
        {
            var data = ManagerUpdata.Instance.DownUpdataData();
            if (data != null)
            {
                var dll = data.DllDatas.FirstOrDefault();
                MessageBox.Show("发现插件（自动下载）：" + dll.Name + "  " + dll.Description);
                var ips = ManagerUpdata.Instance.DownPlugins();
                if (ips != null && ips.Count > 0)
                {
                    foreach (var item in ips)
                    {
                        actionview.AddDragControl(item);
                        this.ViewModel.PluginObjects.Add(item);
                    }
                    //     this.actionview.AlignmentAllControls();
                }
            }
        }

        void menuItemAlign_Click(object sender, RoutedEventArgs e)
        {
            this.actionview.AlignmentAllControls();
        }

        void menuItem_Click(object sender, RoutedEventArgs e)
        {
            this.actionview.OrderAllControls();
        }


        #endregion

        void ItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.pluginview.Height = this.actionview.Height = e.NewSize.Height - 10;
            this.pluginview.Width = this.actionview.Width = e.NewSize.Width - 10;
        }

        void actionview_AddControlEvent(object sender, AddControlArgs e)
        {
            AddContentControl(e.PluginObject);
        }

        private void AddContentControl(IPluginObject iplugin)
        {
            if (iplugin != null)
            {
                var p = this.PluginWindows.FirstOrDefault(i => i.Context.Equals(iplugin));
                if (p != null)
                {
                    p.ShowPlugin();
                }
                else
                {
                    PluginWindow pw = new PluginWindow(iplugin);
                    this.PluginWindows.Add(pw);
                    this.pluginview.Children.Add(pw);
                    pw.ShowPlugin();
                }
            }
        }

        #region 主页面内容添加操作

        private void PluginsEventChanged(PluginsEventArgs args)
        {
            switch (args.Action)
            {

                case PlatformCommon.Events.PluginAction.Add:
                    {
                        AddContentControl(args.PluginObject);
                        break;
                    }

                default: break;
            }
        }

        #endregion

    }

}
