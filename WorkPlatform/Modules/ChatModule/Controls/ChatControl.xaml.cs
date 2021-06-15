using ClientManager;
using ClientManager.Controls;
using CommonHelper;
using Project.Entities;
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
using PlatformCommon.Message;
using PlatformCommon.Plugin;

namespace Modules.ChatModule
{
    /// <summary>
    /// ChatControl.xaml 的交互逻辑
    /// </summary>
    public partial class ChatControl : UserControl, IPluginObject
    {
        #region IPluginObject 成员

        public string PluginName
        {
            get
            {
                return Person != null ? Person.Name : "";
            }
        }

        public ImageSource PluginIcon
        {
            get
            {
                return null;
            }
        }

        private FrameworkElement pluginobject;
        public FrameworkElement Plugin
        {
            get
            {
                return pluginobject;
            }
        }

        public PluginType Type
        {
            get { return PluginType.Window; }
        }

        public event EventHandler Closing;
        protected void OnClosingChanged()
        {
            if (this.Closing != null)
            {
                this.Closing(this, EventArgs.Empty);
            }
        }

        public event EventHandler Opening;
        protected void OpeningChanged()
        {
            if (this.Opening != null)
            {
                this.Opening(this, EventArgs.Empty);
            }
        }

        public event EventHandler Hiding;
        protected void HidingChanged()
        {
            if (this.Hiding != null)
            {
                this.Hiding(this, EventArgs.Empty);
            }
        }
        public bool IsShow
        {
            get
            {
                return this.Visibility == System.Windows.Visibility.Visible;
            }
            set
            {
                if (value)
                {
                    OpeningChanged();
                }
                else
                {
                    HidingChanged();
                }
            }
        }

        public bool IsTool
        {
            get { return true; }
        }

        #endregion

        public ClientInfo Person;

        public ChatControl(ClientInfo person)
        {
            InitializeComponent();

            Person = person;

            this.pluginobject = this;
            this.Loaded += ChatControl_Loaded;
        }

        void ChatControl_Loaded(object sender, RoutedEventArgs e)
        {
            WorkClient.Instance.OnFileAction += client_OnFileAction;
        }

        void client_OnFileAction(object sender, FileActionArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
              {
                  switch (e.Type)
                  {
                      case ControlType.SendStart:
                          {
                              this.filescontrol.Children.Add(e.Control);
                              break;
                          }

                      case ControlType.SendCancel:
                      case ControlType.SendRefuse:
                          {

                              this.filescontrol.Children.Remove(e.Control);
                              break;
                          }
                      case ControlType.SendComplete:
                          {
                              var path = "";
                              var control = e.Control as SendFileControl;

                              if (control != null)
                              {
                                  path = control.FileName + " 发送已经完成"; ;
                              }

                              this.receivemsg.Text += Environment.NewLine;
                              this.receivemsg.Text += path;


                              this.filescontrol.Children.Remove(e.Control);
                              break;
                          }



                      case ControlType.ReceiveStart:
                          {
                              this.filescontrol.Children.Add(e.Control);
                              break;
                          }

                      case ControlType.ReceiveCancel:
                      case ControlType.ReceiveRefuse:
                          {
                              this.filescontrol.Children.Remove(e.Control);
                              break;
                          }
                      case ControlType.ReceiveComplete:
                          {
                              var path = "";
                              var control = e.Control as ReceiveFloderControl;
                              if (control != null)
                              {

                                  path = control.FloderPath + " 接收已经完成"; ;

                              }
                              else
                              {
                                  var filecontrol = e.Control as ReceiveFileControl;
                                  path = filecontrol.FileName + " 接收已经完成"; ;
                              }

                              this.receivemsg.Text += Environment.NewLine;
                              this.receivemsg.Text += path;

                              this.filescontrol.Children.Remove(e.Control);
                              break;
                          }


                      default: break;
                  }
              }));
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            var message = this.msg.Text;
            var data = new MessageData();
            data.Type = MessageType.Message;
            data.Value = message;

            data.SentUser = WorkClient.Instance.Person.ClientInfo;
            data.ResiveUser = Person;

            this.receivemsg.Text += Environment.NewLine;
            this.receivemsg.Text += WorkClient.Instance.Person.ClientInfo.Name + "  " + DateTime.Now.ToString("HH:mm:ss");
            this.receivemsg.Text += Environment.NewLine;
            this.receivemsg.Text += message;
            this.receivemsg.ScrollToEnd();

            WorkClient.Instance.SendMessage(data);

            this.msg.Text = "";
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            OnClosingChanged();
        }

        private void sendfile_Click(object sender, RoutedEventArgs e)
        {
            var files = DialogHelper.SearchFiles(); ;
            if (files != null && files.Count() > 0)
            {
                foreach (var item in files)
                {
                    var data = new MessageData();
                    data.Type = MessageType.StartSendFile;

                    data.Value = new SendFileData() { IsFloder = false, Path = item };
                    data.SentUser = WorkClient.Instance.Person.ClientInfo;
                    data.ResiveUser = Person;
                    WorkClient.Instance.SendMessage(data);
                }
            }
        }

        private void sendfloder_Click(object sender, RoutedEventArgs e)
        {
            var floder = DialogHelper.SearchFolder();
            if (!string.IsNullOrWhiteSpace(floder))
            {
                var data = new MessageData();
                data.Type = MessageType.StartSendFile;
                data.Value = new SendFileData() { IsFloder = true, Path = floder };
                data.SentUser = WorkClient.Instance.Person.ClientInfo; ;
                data.ResiveUser = Person;
                WorkClient.Instance.SendMessage(data);
            }
        }

        public void AddMessage(MessageData message)
        {
            if (message.ResiveUser.KeyId == WorkClient.Instance.Person.ClientInfo.KeyId)
            {
                this.receivemsg.Text += Environment.NewLine;
                this.receivemsg.Text += message.SentUser.Name + "  " + DateTime.Now.ToString("HH:mm:ss");
                this.receivemsg.Text += Environment.NewLine;
                this.receivemsg.Text += message.Value.ToString();
                this.receivemsg.ScrollToEnd();
            }
        }

    }
}
