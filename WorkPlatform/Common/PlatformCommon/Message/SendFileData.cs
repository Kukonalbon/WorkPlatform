using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlatformCommon.Message
{
    [Serializable]
    public class SendFileData
    {
        public bool IsFloder { get; set; }

        public string Path { get; set; }

        public int MessagePort { get; set; }

        public int ReceivePort { get; set; }

        public SendFileData()
        {
            this.IsFloder = false;
        }
    }
}
