using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sodao.FastSocket.Server.Command;
using Sodao.FastSocket.SocketBase;
using Jisons;
using PlatformCommon.Message;
using ServerManage;
using Project.Entities;

namespace ServerManage
{
    public class ServicesCommands
    {

        /// <summary>
        /// 登录命令
        /// </summary>
        public sealed class LoginCommand : ICommand<AsyncBinaryCommandInfo>
        {
            /// <summary>
            /// 返回命令名称
            /// </summary>
            public string Name
            {
                get { return CommandType.Login; }
            }
            /// <summary>
            /// 执行命令
            /// </summary>
            /// <param name="connection"></param>
            /// <param name="commandInfo"></param>
            public void ExecuteCommand(IConnection connection, AsyncBinaryCommandInfo commandInfo)
            {
                PersonData persondata = new PersonData();
                var userinfo = commandInfo.Buffer.DeSerializeBinary<ClientInfo>();
                if (userinfo != null)
                {
                    var persond = WorkService.Instance.Connections.FirstOrDefault(i => i.Person.DomainAcc == userinfo.Name);
                    if (persond != null)
                    {
                        persondata = persond;
                        //persondata.ClientInfo = userinfo;
                        persondata.ClientInfo.IsLogin = true;
                        persondata.ClientInfo.IP = connection.RemoteEndPoint.Address.ToString();
                        persondata.ClientInfo.Port = connection.RemoteEndPoint.Port;

                        if (persondata.IConnection == null)
                        {
                            persondata.IConnection = connection;
                        }
                        else
                        {
                            if (!persondata.IConnection.Equals(connection))
                            {
                                MessageData md = new MessageData();
                                md.Type = MessageType.Logout;
                                persondata.IConnection.BeginSend(Packet.Creater(commandInfo.SeqID, md.SerializeBinary()));
                                persondata.IConnection = connection;
                            }
                        }
                    }
                }

                commandInfo.Reply(connection, persondata.SerializeBinary());

                var sendlogin = new MessageData();
                sendlogin.Type = MessageType.Login;
                WorkService.Instance.SentAllMessage(sendlogin);
            }
        }

        public sealed class GetUsersCommand : ICommand<AsyncBinaryCommandInfo>
        {
            /// <summary>
            /// 返回命令名称
            /// </summary>
            public string Name
            {
                get { return CommandType.GetUsers; }
            }

            /// <summary>
            /// 执行命令
            /// </summary>
            /// <param name="connection"></param>
            /// <param name="commandInfo"></param>
            public void ExecuteCommand(IConnection connection, AsyncBinaryCommandInfo commandInfo)
            {
                commandInfo.Reply(connection, WorkService.Instance.Connections.SerializeBinary());
            }
        }

        public sealed class MessageCommand : ICommand<AsyncBinaryCommandInfo>
        {
            /// <summary>
            /// 返回命令名称
            /// </summary>
            public string Name
            {
                get { return CommandType.Message; }
            }

            /// <summary>
            /// 执行命令
            /// </summary>
            /// <param name="connection"></param>
            /// <param name="commandInfo"></param>
            public void ExecuteCommand(IConnection connection, AsyncBinaryCommandInfo commandInfo)
            {
                var packet = commandInfo.Buffer.DeSerializeBinary<Packet>();
                if (packet != null)
                {
                    var msg = packet.Body.DeSerializeBinary<MessageData>();
                    if (msg != null)
                    {
                        var persondata = WorkService.Instance.Connections.FirstOrDefault(i => i.KeyId == msg.ResiveUser.KeyId);
                        if (persondata != null)
                        {
                            persondata.IConnection.BeginSend(packet);
                        }
                        else
                        {
                            //未登录
                        }
                    }
                }
            }
        }

    }

}
