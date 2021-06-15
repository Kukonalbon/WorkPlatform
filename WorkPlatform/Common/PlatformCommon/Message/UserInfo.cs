using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jisons;

namespace PlatformCommon.Message
{

    [Serializable]
    public class ClientInfo : Jisons.JisonsNotificationObject.JisonsINotifyPropertyChanged
    {
        public int KeyId { get; set; }

        private bool isLogin = false;
        public bool IsLogin
        {
            get
            {
                return isLogin;
            }
            set
            {
                isLogin = value;
                this.RaisePropertyChanged(() => this.IsLogin);
            }
        }

        public string Name { get; set; }

        public string PassWord { get; set; }

        private string iP = "";
        public string IP
        {
            get
            {
                return iP;
            }
            set
            {
                iP = value;
                this.RaisePropertyChanged(() => this.IP);
            }
        }

        private int port = 0;
        public int Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
                this.RaisePropertyChanged(() => this.Port);
            }
        }

        public ClientInfo()
        {
            KeyId = 13;
        }
    }
}
