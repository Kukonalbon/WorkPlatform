using ClientManager;
using Jisons;
using System;
using System.Collections.Generic;
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
using System.Linq;

namespace Modules.LoginModule
{
    /// <summary>
    /// LoginControl.xaml 的交互逻辑
    /// </summary>
    public partial class LoginControl : UserControl
    {
        public LoginControl()
        {
            this.InitializeComponent();

            this.name.PreviewMouseLeftButtonDown += txt1_PreviewMouseLeftButtonDown;

            this.password.PreviewMouseLeftButtonDown += txt3_PreviewMouseLeftButtonDown;

            this.Loaded += LoginControl_Loaded;
        }

        void LoginControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (WorkClient.Instance != null)
            {
                WorkClient.Instance.OnMessageReceive += Instance_OnMessageReceive;
            }


             var ip = NetHelper.GetNetInfo().FirstOrDefault().IP.FirstOrDefault();
             serverip.Text = !string.IsNullOrWhiteSpace(ip) ? ip : "127.0.0.1";
        }

        void Instance_OnMessageReceive(object sender, WorkCommon.Message.MessageDataArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                switch (e.Data.Type)
                {
                    case WorkCommon.Message.MessageType.Login:
                        {
                            if (WorkClient.Instance.Person.KeyId != 0)
                            {
                                this.msgText.Text = "登录成功";
                                this.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            else
                            {
                                this.msgText.Text = "登录失败，请检查用户名密码";
                            }

                            break;
                        }
                }
            }));
        }

        void txt3_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.txt4.Visibility = Visibility.Hidden;
            this.Present1();
        }

        void txt1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.txt2.Visibility = Visibility.Hidden;
            this.Presen2();
        }

        public void Present1()
        {
            if (this.name.Text == "")
            {
                this.txt2.Visibility = Visibility.Visible;
            }
            else
            {
                this.txt2.Visibility = Visibility.Hidden;
            }
        }

        public void Presen2()
        {
            if (this.password.Text == "")
            {
                this.txt4.Visibility = Visibility.Visible;
            }
            else
            {
                this.txt4.Visibility = Visibility.Hidden;
            }
        }

        //this.msgText.Text = "已经登录！登录不能重复";
        //this.msgText.Text = "已经登录！登录不能重复";
        //this.msgText.Text = "密码错误!请重新输入";
        //this.msgText.Text = "用户名不存在！";
        //this.msgText.Text = " 用户名和密码错误！";

        bool islogin = false;

        private void login_Click(object sender, RoutedEventArgs e)
        {
            if (!islogin)
            {
                var ip = serverip.Text;
                var endport = port.Text;
                //var ip = NetHelper.GetNetInfo().FirstOrDefault().IP.FirstOrDefault();
                //ip = !string.IsNullOrWhiteSpace(ip) ? ip : "127.0.0.1";
                var b = WorkClient.Instance.RegisterServerNode(ip, int.Parse(endport));
                if (b)
                {
                    islogin = true;
                }
                else
                {
                    MessageBox.Show("无法连接服务器");
                }
            }

            var name = this.name.Text;
            var pw = this.password.Text;
            WorkClient.Instance.Login(name, pw);
        }
    }
}