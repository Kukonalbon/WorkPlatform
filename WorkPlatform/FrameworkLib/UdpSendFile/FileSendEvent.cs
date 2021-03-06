using System;
using System.Collections.Generic;
using System.Text;

namespace UdpSendFiles
{

    public delegate void FileSendEventHandler(object sender, FileSendEventArgs e);

    public class FileSendEventArgs : EventArgs
    {
        private SendFileManager _sendFileManager;

        public FileSendEventArgs(SendFileManager sendFileManager)
            : base()
        {
            _sendFileManager = sendFileManager;
        }

        public SendFileManager SendFileManager
        {
            get { return _sendFileManager; }
        }
    }
}
