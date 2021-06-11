using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkCommon.Message
{
    public enum MessageType
    {
        Login,

        Message,

        Command,

        Logout,

        StartSendFile,

        SendingFile,

        GetAllUsers,
    }
}
