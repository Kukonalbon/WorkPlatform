using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Jisons;
using UdpSendFiles;
using WorkCommon.Message;

namespace ClientManager.Controls
{
    public class ReceiveFloderControl : ReceiveFileControl
    {
        public DirectoryInfo DirectoryInfo { get; private set; }

        public UdpReceiveFile UdpReceiveFiles { get; private set; }

        public override int MessagePort
        {
            get
            {
                return base.MessagePort;
            }
        }

        public override int ReceivePort
        {
            get
            {
                return UdpReceiveFiles != null ? UdpReceiveFiles.Port : -1;
            }
        }

        public string FloderPath { get; private set; }

        public string FloderName { get; private set; }

        protected int TotleFileLength { get; private set; }

        public FloderData FloderData { get; private set; }

        public ReceiveFloderControl(WorkClient workclient)
            : base(workclient)
        {
            this.FloderData = new FloderData();
        }

        protected override void StartReceiveFile()
        {
            base.StartReceiveFile();
            UDPReceive.IsFloder = true;

            int port = SocketHelper.FindNoUsedPort();
            this.UdpReceiveFiles = new UdpReceiveFile(port);
            this.UdpReceiveFiles.Start();

            this.UdpReceiveFiles.RequestSendFile += UdpReceiveFiles_RequestSendFile;
            this.UdpReceiveFiles.FileReceiveBuffer += UdpReceiveFiles_FileReceiveBuffer;
            this.UdpReceiveFiles.FileReceiveComplete += UdpReceiveFiles_FileReceiveComplete;
            this.UdpReceiveFiles.FileReceiveCancel += UdpReceiveFiles_FileReceiveCancel;
        }

        void UdpReceiveFiles_FileReceiveCancel(object sender, FileReceiveEventArgs e)
        {

        }
        int receiveCount = 0;
        void UdpReceiveFiles_FileReceiveComplete(object sender, FileReceiveEventArgs e)
        {
            lock (LockObject)
            {
                var file = this.FloderData.FileDataList.FirstOrDefault(i => i.Name.Equals(e.ReceiveFileManager.Name));
                if (file != null)
                {
                    file.IsReceive = true;
                }
                receiveCount++;

                if (this.FloderData.FileDataList.All(i => i.IsReceive) || receiveCount == this.FloderData.FileDataList.Count)
                {
                    Close();
                    SendType(ControlType.ReceiveComplete);
                }
            }
        }

        void UdpReceiveFiles_FileReceiveBuffer(object sender, FileReceiveBufferEventArgs e)
        {
            TotalTransfersSize += e.Size;
            base.ShowSpeed((double)TotalTransfersSize, (double)TotleFileLength);
        }
      
        void UdpReceiveFiles_RequestSendFile(object sender, RequestSendFileEventArgs e)
        {
            var sendFileEventArgs = new RequestSendFileEventArgs(e.TraFransfersFileStart, e.RemoteIP);
            sendFileEventArgs.Path = this.FloderPath;

            var file = this.FloderData.FileDataList.FirstOrDefault(i => i.MD5.Equals(e.TraFransfersFileStart.MD5));
            if (file != null)
            {
                sendFileEventArgs.TraFransfersFileStart.FileName = file.Name;
                UdpReceiveFiles.AcceptReceive(sendFileEventArgs);
            }
        }

        protected override void UDPReceive_RequestSendFile(object sender, UdpSendFiles.RequestSendFileEventArgs e)
        {
            if (e.TraFransfersFileStart.FileName.Contains(MessageSign.StartSign))
            {
                var sendFileEventArgs = new RequestSendFileEventArgs(e.TraFransfersFileStart, e.RemoteIP);
                base.UDPReceive.AcceptReceive(sendFileEventArgs);
            }
            else if (e.TraFransfersFileStart.FileName.Contains(MessageSign.StartSign))
            {
                base.SendFileEventArgs = new RequestSendFileEventArgs(e.TraFransfersFileStart, e.RemoteIP);
            }
            else
            {
                base.UDPReceive_RequestSendFile(sender, e);
                this.Dispatcher.BeginInvoke((Action)(() => this.FloderName = this.FileName));
            }
        }

        protected override void UDPReceive_FileReceiveComplete(object sender, FileReceiveEventArgs e)
        {
            if (e.ReceiveFileManager.FileName.Contains(MessageSign.StartSign))
            {
                this.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        var floderdata = e.ReceiveFileManager.FileStream.DeSerializeStream() as FloderData;
                        if (floderdata != null)
                        {
                            this.FloderData = floderdata;
                            TotleFileLength += (int)this.FloderData.Length;
                        }
                    }));
            }
        }

        protected override void UDPReceive_FileReceiveBuffer(object sender, FileReceiveBufferEventArgs e)
        {
            //base.UDPReceive_FileReceiveBuffer(sender, e);
        }

        protected override void Save()
        {
            base.Save();
            var path = Path.Combine("c:\\", this.FloderName);
            var fileinfo = new FileInfo(path);
            if (fileinfo.Exists)
            {
                path = Path.Combine(path, DateTime.Now.ToString("yyyyMMddHHmmss"));
            }

            var dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                dir.Create();
            }

            this.FloderPath = dir.FullName;
        }

        public override void Close()
        {
            if (this.UdpReceiveFiles != null)
            {
                this.UdpReceiveFiles.RequestSendFile -= UdpReceiveFiles_RequestSendFile;
                this.UdpReceiveFiles.FileReceiveBuffer -= UdpReceiveFiles_FileReceiveBuffer;
                this.UdpReceiveFiles.FileReceiveComplete -= UdpReceiveFiles_FileReceiveComplete;
                this.UdpReceiveFiles.FileReceiveCancel -= UdpReceiveFiles_FileReceiveCancel;

                this.UdpReceiveFiles.Dispose();
                this.UdpReceiveFiles = null;
            }

            base.Close();
        }

        protected override void Cancel()
        {
            base.Cancel();


        }

    }
}
