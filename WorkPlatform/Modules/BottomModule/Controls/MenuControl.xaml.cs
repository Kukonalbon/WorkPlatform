using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClientManager;

namespace Modules.BottomModule
{
    /// <summary>
    /// MenuControl.xaml 的交互逻辑
    /// </summary>
    // [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    public partial class MenuControl : UserControl
    {

        Popup controlPopup;

        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(double), typeof(MenuControl), new PropertyMetadata(0d));

        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(MenuControl), new PropertyMetadata(8d));

        public MenuControl()
        {
            this.InitializeComponent();

            this.Loaded += MenuControl_Loaded;
            this.Unloaded += MenuControl_Unloaded;
        }

        void MenuControl_Loaded(object sender, RoutedEventArgs e)
        {
            controlPopup = ((System.Windows.FrameworkTemplate)menuitem.Template).FindName("PART_Popup", menuitem) as Popup;

            if (controlPopup != null)
            {
                this.controlPopup.IsOpen = false;
                this.controlPopup.AllowsTransparency = true;
                this.controlPopup.PopupAnimation = PopupAnimation.Fade;
                this.controlPopup.StaysOpen = false;
                this.controlPopup.Opened += controlPopup_Opened;
            }
        }

        void MenuControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (controlPopup != null)
            {
                this.controlPopup.Opened -= controlPopup_Opened;
            }
        }

        void controlPopup_Opened(object sender, EventArgs e)
        {
            var p = this.menuitem.TranslatePoint(new Point(0, 0), this);
            this.controlPopup.HorizontalOffset = -p.X + OffsetX;
            this.controlPopup.VerticalOffset = -OffsetY;

            //WorkClient.Instance.Person.Person   //GetUserModules
        }

    }
}