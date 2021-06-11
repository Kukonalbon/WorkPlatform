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
using Project.BusinessFacade;
using ServerManage;
using WorkCommon.Message;

namespace PrismServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<PersonData> Persons
        {
            get { return (List<PersonData>)GetValue(PersonsProperty); }
            set { SetValue(PersonsProperty, value); }
        }
        public static readonly DependencyProperty PersonsProperty =
            DependencyProperty.Register("Persons", typeof(List<PersonData>), typeof(MainWindow), new PropertyMetadata(null));

        public MainWindow()
        {
            InitializeComponent();

            WorkService.Start();

            InitPersons();
        }

        public void InitPersons()
        {
            Persons = WorkService.Instance.Connections;
            this.dataGrid.ItemsSource = Persons;
        }
    }
}

