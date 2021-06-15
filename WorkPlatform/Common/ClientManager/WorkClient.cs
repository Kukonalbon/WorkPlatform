using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sodao.FastSocket.Client;
using Sodao.FastSocket.SocketBase.Utils;
using Jisons;
using Sodao.FastSocket.SocketBase;
using UdpSendFiles;
using System.Windows;
using System.IO;
using ClientManager.Controls;
using PlatformCommon.Message;

namespace ClientManager
{
    public class WorkClient : AsyncBinarySocketClient
    {

        private static WorkClient instance;
        public static WorkClient Instance
        {
            get
            {
                return instance;
            }
        }

        public static void InitWorkClient()
        {
            instance = new WorkClient();
        }

        public IConnection IConnection { get; private set; }

        public bool IsLogin { get; set; }

        public event EventHandler<MessageDataArgs> OnMessageReceive;

        public event EventHandler<MessageDataArgs> OnSendFile;

        public event EventHandler<FileActionArgs> OnFileAction;

        public string Guid { get; private set; }

        public PersonData Person { get; private set; }

        private ClientInfo clientInfo = null;

        protected WorkClient()
            : base(8192, 8192, 3000, 3000)
        {
            Guid = DateTime.Now.ToString();
        }

        public bool RegisterServerNode(string ip = "127.0.0.1", int port = 12000)
        {
            this.UnRegisterServerNode(this.Guid);
            return this.RegisterServerNode(this.Guid, new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ip), port));
        }

        protected void ReceivedMessageHandle(EventHandler<MessageDataArgs> e, MessageData msd)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        if (e != null)
                        {
                            e(this, new MessageDataArgs() { Data = msd });
                        }
                    }));
        }

        internal void FileActionHandle(FileActionArgs args)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                     {
                         if (this.OnFileAction != null)
                         {
                             this.OnFileAction(this, args);
                         }
                     }));
        }

        #region 重载的方法

        protected override void OnMessageReceived(Sodao.FastSocket.SocketBase.IConnection connection, Sodao.FastSocket.SocketBase.MessageReceivedEventArgs e)
        {
            base.OnMessageReceived(connection, e);
        }

        protected override void OnResponse(IConnection connection, Sodao.FastSocket.Client.Response.AsyncBinaryResponse response)
        {
            base.OnResponse(connection, response);

            var msg = response.Buffer.DeSerializeBinary<MessageData>();
            if (msg != null)
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    JudgeRecived(msg);
                }));
            }
        }

        protected override void OnReceiveTimeout(IConnection connection, Request<Sodao.FastSocket.Client.Response.AsyncBinaryResponse> request)
        {
            base.OnReceiveTimeout(connection, request);
        }

        protected override void OnConnected(IConnection connection)
        {
            base.OnConnected(connection);

            var md = new MessageData();

            IConnection = connection;
            if (IsLogin)
            {
                Login();
                md.Type = MessageType.ReConnetServer;
            }
            else
            {
                md.Type = MessageType.ConnetServer;
            }

            ReceivedMessageHandle(this.OnMessageReceive, md);
        }

        protected override void Send(Request<Sodao.FastSocket.Client.Response.AsyncBinaryResponse> request)
        {
            base.Send(request);
        }

        #endregion

        public void SendMessage(MessageData data)
        {
            var pack = Packet.Creater(this.NextRequestSeqID(), data.SerializeBinary());
            this.Send(CommandType.Message, pack.SerializeBinary(), res => res.Buffer);
        }

        public int GetNoUsedRequestSeqID()
        {
            return base.NextRequestSeqID();
        }

        private void JudgeRecived(MessageData data)
        {
            if (data != null)
            {
                switch (data.Type)
                {
                    case MessageType.Login:
                        {
                            if (data.SentUser != null && data.SentUser.KeyId == this.Person.KeyId)
                            {
                                // Console.WriteLine("self login");
                            }
                            else
                            {
                                ReceivedMessageHandle(this.OnMessageReceive, data);
                            }

                            break;
                        }

                    case MessageType.Logout:
                        {
                            if (data.SentUser != null && data.SentUser.KeyId == this.Person.KeyId)
                            {
                                Logout();
                                data.Value = this.clientInfo;
                                ReceivedMessageHandle(this.OnMessageReceive, data);
                            }
                            else
                            {
                                ReceivedMessageHandle(this.OnMessageReceive, data);
                            }

                            break;
                        }

                    case MessageType.StartSendFile:
                        {
                            var filedata = data.Value as SendFileData;
                            if (filedata != null)
                            {
                                ReceiveFileControl fc = filedata.IsFloder ? new ReceiveFloderControl(this) : new ReceiveFileControl(this);

                                var fileargs = new FileActionArgs() { Type = ControlType.ReceiveStart };
                                fileargs.Control = fc;

                                this.FileActionHandle(fileargs);

                                var msg = new MessageData();
                                msg.ResiveUser = data.SentUser;
                                msg.SentUser = data.ResiveUser;

                                filedata.MessagePort = fc.MessagePort;
                                filedata.ReceivePort = fc.ReceivePort;

                                msg.Value = filedata;

                                msg.Type = MessageType.SendingFile;

                                this.SendMessage(msg);
                            }
                            break;
                        }

                    case MessageType.SendingFile:
                        {
                            var filedata = data.Value as SendFileData;
                            if (filedata != null && !string.IsNullOrWhiteSpace(filedata.Path))
                            {
                                var fileargs = new FileActionArgs() { Type = ControlType.SendStart };

                                SendFileControl sc = filedata.IsFloder ? new SendFloderControl(this, data.SentUser.IP, filedata.MessagePort, filedata.ReceivePort) : new SendFileControl(this, data.SentUser.IP, filedata.MessagePort);

                                fileargs.Control = sc;
                                sc.SentFile(filedata.Path);

                                this.FileActionHandle(fileargs);
                            }
                            break;
                        }

                    case MessageType.Message:
                        {
                            ReceivedMessageHandle(this.OnMessageReceive, data);
                            break;
                        }

                    default: break;
                }
            }
        }


        #region 命令处理

        public void Login(string name, string password)
        {
            clientInfo = new ClientInfo();
            clientInfo.Name = name;
            clientInfo.PassWord = password;

            Login();
        }

        public void Login()
        {
            if (this.IConnection == null || clientInfo == null)
            {
                return;
            }

            IsLogin = true;

            clientInfo.IP = this.IConnection.LocalEndPoint.Address.ToString();
            clientInfo.Port = this.IConnection.LocalEndPoint.Port;
            this.Send(CommandType.Login, clientInfo.SerializeBinary(), res => res.Buffer.DeSerializeBinary<PersonData>()).ContinueWith(c =>
            {
                Person = c.Result;
                if (c.IsFaulted || Person == null)
                {

                }

                ReceivedMessageHandle(this.OnMessageReceive, new MessageData() { Value = Person, Type = MessageType.Login });
            });
        }

        public void Logout()
        {
            IsLogin = false;
        }

        public void GetAllUsers()
        {
            this.Send(CommandType.GetUsers, new byte[1] { 1 }, res => res.Buffer.DeSerializeBinary<List<PersonData>>()).ContinueWith(c =>
            {
                if (!c.IsFaulted && c.Result != null)
                {
                    ReceivedMessageHandle(this.OnMessageReceive, new MessageData() { Value = c.Result, Type = MessageType.GetAllUsers });
                }
            });
        }

        #endregion

    }
}
