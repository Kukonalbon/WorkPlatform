using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project.Entities;
using Sodao.FastSocket.SocketBase;
using PlatformCommon.Message;
using Jisons;

namespace PlatformCommon.Message
{
    [Serializable]
    public class PersonData : Jisons.JisonsNotificationObject.JisonsINotifyPropertyChanged
    {
        public int KeyId { get; set; }

        private Sys_Person person = new Sys_Person();
        public Sys_Person Person
        {
            get
            {
                return person;
            }
            set
            {
                person = value;
                this.RaisePropertyChanged(() => this.Person);
            }
        }

        private ClientInfo clientInfo = new ClientInfo();
        public ClientInfo ClientInfo
        {
            get
            {
                return clientInfo;
            }
            set
            {
                clientInfo = value;
                this.RaisePropertyChanged(() => this.ClientInfo);
            }
        }

        public string Message { get; set; }

        [NonSerialized]
        public IConnection IConnection;

        public PersonData()
        {
            ClientInfo = new ClientInfo();
        }
    }
}
