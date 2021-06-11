using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Sodao.FastSocket.Server;
using Sodao.FastSocket.Server.Command;
using Sodao.FastSocket.SocketBase;
using Jisons;
using WorkCommon.Message;

namespace ServerManage
{
    public class WorkService : CommandSocketService<AsyncBinaryCommandInfo>
    {

        private static WorkService instance;
        public static WorkService Instance
        {
            get
            {
                return instance;
            }
        }

        private List<PersonData> connections = new List<PersonData>();
        public List<PersonData> Connections
        {
            get { return connections; }
        }

        public WorkService()
        {
            instance = this;

            InitPersons();
        }

        public void InitPersons()
        {
            Connections.Clear();
            foreach (var person in PersonManager.Instance.Persons)
            {
                var persondata = new PersonData();
                persondata.KeyId = person.KeyId;
                persondata.Person = person;

                persondata.ClientInfo.Name = person.PersonName;
                persondata.ClientInfo.KeyId = person.KeyId;

                Connections.Add(persondata);
            }
        }

        public static void Start()
        {
            SocketServerManager.Init();
            SocketServerManager.Start();
        }

        protected override void AddCommand(ICommand<AsyncBinaryCommandInfo> cmd)
        {
            base.AddCommand(cmd);
        }

        public override void OnReceived(IConnection connection, AsyncBinaryCommandInfo cmdInfo)
        {
            base.OnReceived(connection, cmdInfo);
        }

        /// <summary>
        /// 当连接时会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        public override void OnConnected(IConnection connection)
        {
            base.OnConnected(connection);
        }

        void connection_SendCallback(IConnection connection, SendCallbackEventArgs e)
        {

        }

        /// <summary>
        /// 当连接断开时会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        public override void OnDisconnected(IConnection connection, Exception ex)
        {
            //下线处理
            var conet = WorkService.Instance.Connections.FirstOrDefault(i => i.IConnection != null && i.IConnection.Equals(connection));
            if (conet != null)
            {
                conet.IConnection = null;
                conet.ClientInfo.IsLogin = false;
                //通知下线
                SentAllMessage(new MessageData() { Type = MessageType.Logout });
            }

            base.OnDisconnected(connection, ex);
        }

        /// <summary>
        /// 当发生错误时会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        public override void OnException(IConnection connection, Exception ex)
        {
            base.OnException(connection, ex);
        }

        /// <summary>
        /// 当服务端发送Packet完毕会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="e"></param>
        public override void OnSendCallback(IConnection connection, SendCallbackEventArgs e)
        {
            base.OnSendCallback(connection, e);
        }

        /// <summary>
        /// 处理未知的命令
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandInfo"></param>
        protected override void HandleUnKnowCommand(IConnection connection, AsyncBinaryCommandInfo commandInfo)
        {

        }

        public void SentAllMessage(MessageData msg)
        {
            foreach (var item in WorkService.Instance.Connections)
            {
                if (item.IConnection != null)
                {
                    item.IConnection.BeginSend(Packet.Creater(DateTime.Now.Millisecond, msg.SerializeBinary()));
                }
            }
        }
    }
}
