using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using WorkCommon.Message;

namespace Modules.ChatModule
{

    public class UserData
    {
        public ObservableCollection<UserGrop> Grops { get; set; }
        public UserData()
        {
            this.Grops = new ObservableCollection<UserGrop>();
        }
    }

    public class UserGrop
    {
        public string Icon { get; set; }
        public string Name { get; set; }

        public ObservableCollection<ClientInfo> Users { get; set; }
        public UserGrop()
        {
            this.Users = new ObservableCollection<ClientInfo>();
        }
    }

    //public class ClientInfo
    //{

    //    public string Name { get; set; }
    //    public string IP { get; set; }
    //    public int Port { get; set; }

    //}


}
