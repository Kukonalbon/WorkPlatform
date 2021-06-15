using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlatformCommon.Message
{
    [Serializable]
    public class MessageData
    {
        public MessageType Type { get; set; }

        public object Value { get; set; }

        public ClientInfo SentUser { get; set; }

        public ClientInfo ResiveUser { get; set; }
    }

    public class MessageDataArgs : EventArgs
    {
        public MessageData Data { get; set; }
    }

}
