using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Jisons;
using UdpSendFiles;
using PlatformCommon.Message;

namespace ClientManager.Controls
{
    public class SendFloderControl : SendFileControl
    {

        public DirectoryInfo DirectoryInfo { get; private set; }

        protected int RecievePort { get; set; }

        public UdpSendFile UdpSendFiles { get; private set; }

        public List<FileData> FileDataInfoList = new List<FileData>();

        public FloderData FloderData { get; private set; }

        public SendFloderControl(WorkClient workclient, string remoteIP, int remotePort, int receivePort)
            : base(workclient, remoteIP, remotePort)
        {
            this.RecievePort = receivePort;
        }

        public override void SentFile(string filename)
        {
            DirectoryInfo = new System.IO.DirectoryInfo(filename);

            FileDataInfoList = DirectoryHelper.GetAllFiles(DirectoryInfo.FullName);

            if (FileDataInfoList.Count > 300)
            {
                //超出数量
                return;
            }

            FileName = DirectoryInfo.Name;

            var icon = IconHelper.GetDirectoryIcon(filename).ToBitmap();

            FloderData = new FloderData();
            FloderData.FullPath = DirectoryInfo.FullName;
            FloderData.FileDataList = FileDataInfoList;
            FloderData.FileDataList.ForEach(i => FloderData.Length += i.Length);

            Stream fs = FloderData.SerializeStream();
            this.TraFransfersFileStart = UdpSend.SendFile(fs, icon, MessageSign.StartSign);

            Icon = icon.ConvertImageSource();
        }

        protected override void UdpSendFile_FileSendComplete(object sender, UdpSendFiles.FileSendEventArgs e)
        {
            if (e.SendFileManager.FileName.Contains(MessageSign.StartSign))
            {
                byte[] datas = new byte[4] { 1, 3, 1, 4 };
                Stream ds = new MemoryStream(datas);
                this.TraFransfersFileStart = UdpSend.SendFile(ds, this.TraFransfersFileStart.Image, DirectoryInfo.Name);
            }
        }

        protected override void UdpSend_FileSendCancel(object sender, UdpSendFiles.FileSendEventArgs e)
        {
            SendType(ControlType.SendCancel);
        }

        protected override void UdpSendFile_FileSendAccept(object sender, UdpSendFiles.FileSendEventArgs e)
        {
            if (!e.SendFileManager.FileName.Contains(MessageSign.StartSign))
            {
                Stream fs = FloderData.SerializeStream();
                this.TraFransfersFileStart = UdpSend.SendFile(fs, null, MessageSign.SendingSign);

                int port = SocketHelper.FindNoUsedPort();
                this.UdpSendFiles = new UdpSendFile(base.UdpSend.RemoteIP, this.RecievePort, port);
                this.UdpSendFiles.FileSendComplete += udpsend_FileSendComplete;
                this.UdpSendFiles.FileSendBuffer += udpsend_FileSendBuffer;
                this.UdpSendFiles.FileSendRefuse += UdpSendFiles_FileSendRefuse;
                this.UdpSendFiles.FileSendCancel += UdpSendFiles_FileSendCancel;
                this.UdpSendFiles.Start();

                SendNextFile();
                StartTime = HighPrecisionTimerHelper.PrecisionTimerOfMillisecond;
            }
        }

        void UdpSendFiles_FileSendRefuse(object sender, FileSendEventArgs e)
        {

        }

        void UdpSendFiles_FileSendCancel(object sender, FileSendEventArgs e)
        {


        }

        void udpsend_FileSendBuffer(object sender, FileSendBufferEventArgs e)
        {
            TotalTransfersSize += e.Size;
            base.ShowSpeed(TotalTransfersSize, FloderData.Length);
        }

        int receiveCount = 0;
        void udpsend_FileSendComplete(object sender, FileSendEventArgs e)
        {
            var sendfile = this.FloderData.FileDataList.FirstOrDefault(i => i.FullName.Equals(e.SendFileManager.FileName) && i.MD5.Equals(e.SendFileManager.MD5));
            if (sendfile != null)
            {
                sendfile.IsReceive = true;
            }

            SendNextFile();

            receiveCount++;

            if (this.FloderData.FileDataList.All(i => i.IsReceive) || receiveCount == this.FloderData.FileDataList.Count)
            {
                Close();
                SendType(ControlType.SendComplete);
            }
        }

        protected override void UdpSend_FileSendRefuse(object sender, UdpSendFiles.FileSendEventArgs e)
        {
            SendType(ControlType.SendRefuse);
        }

        protected override void UdpSend_FileSendBuffer(object sender, UdpSendFiles.FileSendBufferEventArgs e)
        {

        }

        public override void Close()
        {
            if (this.UdpSendFiles != null)
            {
                this.UdpSendFiles.FileSendRefuse -= UdpSendFiles_FileSendRefuse;
                this.UdpSendFiles.FileSendCancel -= UdpSendFiles_FileSendCancel;
                this.UdpSendFiles.FileSendComplete -= udpsend_FileSendComplete;
                this.UdpSendFiles.FileSendBuffer -= udpsend_FileSendBuffer;
                this.UdpSendFiles.Dispose();
                this.UdpSendFiles = null;
            }

            base.Close();
        }

        protected void SendNextFile()
        {
            var file = this.FloderData.FileDataList.FirstOrDefault(i => !i.IsSend);
            if (file != null)
            {
                file.IsSend = true;
                this.UdpSendFiles.SendFile(file.FullName);

            }
        }

    }
}

