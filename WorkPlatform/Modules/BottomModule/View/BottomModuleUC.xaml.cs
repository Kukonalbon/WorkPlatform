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
using PlatformCommon.Events;
using PlatformCommon.Manager;
using PlatformCommon.Plugin;

namespace Modules.BottomModule
{
    /// <summary>
    /// BottomToolBarUC.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.BottomToolBar)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class BottomModuleUC : UserControl
    {
        public BottomModuleUC()
        {
            InitializeComponent();

            this.Loaded += BottomToolBarUC_Loaded;
        }

        void BottomToolBarUC_Loaded(object sender, RoutedEventArgs e)
        {
          
        }

        [Import]
        public BottomModuleViewModel ViewModel
        {
            get
            {
                return this.DataContext as BottomModuleViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        private void pluginAction_Click(object sender, RoutedEventArgs e)
        {
            var control = sender as FrameworkElement;
            if (control != null)
            {
                var po = control.DataContext as IPluginObject;
                if (po != null)
                {
                    po.IsShow = !po.IsShow;
                }
            }
        }


    }
}
