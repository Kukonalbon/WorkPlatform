using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ClientManager;
using WorkCommon.Message;
using WorkCommon.Plugin;
using Project.Entities;
using WorkCommon.Manager;
using WorkCommon.Events;

namespace Modules.ChatModule
{
    /// <summary>
    /// MainChat.xaml 的交互逻辑
    /// </summary>
    public partial class MainChat : UserControl, IPluginObject
    {

        #region IPluginObject 成员

        public string PluginName
        {
            get
            {
                return "通讯模块";
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
                if (pluginobject == null)
                {
                    pluginobject = new MainChat();
                }
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

        public MainChat()
        {

            InitializeComponent();

            this.Loaded += MainControl_Loaded;

            WorkClient.Instance.OnMessageReceive += client_OnMessageReceive;
        }

        void client_OnMessageReceive(object sender, MessageDataArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
              {
                  switch (e.Data.Type)
                  {
                      case MessageType.GetAllUsers:
                          {
                              var persons = e.Data.Value as List<PersonData>;
                              if (persons != null)
                              {
                                  InitViewPerson(persons);
                              }
                              break;
                          }

                      case MessageType.Login:
                          {
                              WorkClient.Instance.GetAllUsers();
                              break;
                          }

                      case MessageType.Logout:
                          {
                              WorkClient.Instance.GetAllUsers();
                              break;
                          }

                      case MessageType.Message:
                          {
                              var chatcontrol = ShowClient(e.Data.SentUser);
                              if (chatcontrol != null)
                              {
                                  chatcontrol.AddMessage(e.Data);
                              }

                              break;
                          }
                  }
              }));
        }

        void MainControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitData();
        }

        public void InitData()
        {
            WorkClient.Instance.GetAllUsers();
        }

        int logincount = 0;
        public void InitViewPerson(List<PersonData> persons)
        {

            logincount = 0;
            List<UserGrop> gropList = new List<UserGrop>();
            foreach (var person in persons)
            {
                if (person.ClientInfo.IsLogin == true)
                {
                    logincount++;
                }

                var gropname = person.Person.CompName;
                if (string.IsNullOrWhiteSpace(gropname))
                {
                    gropname = "【未分组】";
                }
                var grop = gropList.FirstOrDefault(i => i.Name.Equals(gropname));
                if (grop == null)
                {
                    grop = new UserGrop() { Name = gropname };
                    gropList.Add(grop);
                }

                if (person.KeyId.Equals(WorkClient.Instance.Person.KeyId))
                {
                    this.usernam.Content = person.ClientInfo.Name;
                }
                else
                {
                    grop.Users.Add(person.ClientInfo);
                }
            }

            this.usercount.Content = logincount;
            try
            {
                this.groplistbox.ItemsSource = gropList;
            }
            catch
            {

            }
        }

        private void addnewchat_Click(object sender, MouseButtonEventArgs e)
        {
            var control = sender as FrameworkElement;
            var resive = control.DataContext as ClientInfo;
            ShowClient(resive);
        }

        List<ChatControl> chatControlList = new List<ChatControl>();
        public ChatControl ShowClient(ClientInfo client)
        {
            ChatControl chatcontrol = null;
            if (client != null)
            {
                chatcontrol = chatControlList.FirstOrDefault(i => i.Person.KeyId == client.KeyId);
                if (chatcontrol == null)
                {
                    chatcontrol = new ChatControl(client);
                    chatControlList.Add(chatcontrol);
                    GlobalEvent.Instance.EventAggregator.GetEvent<PluginsEvent>().Publish(new PluginsEventArgs() { Action = PluginAction.Add, PluginObject = chatcontrol });
                }
                GlobalEvent.Instance.EventAggregator.GetEvent<PluginsEvent>().Publish(new PluginsEventArgs() { Action = PluginAction.Show, PluginObject = chatcontrol });
            }
            return chatcontrol;
        }

    }
}