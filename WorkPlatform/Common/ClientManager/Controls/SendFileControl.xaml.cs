using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Jisons;
using UdpSendFiles;
using WorkCommon.Message;

namespace ClientManager.Controls
{
    /// <summary>
    /// SentFileControl.xaml 的交互逻辑
    /// </summary>
    public partial class SendFileControl : UserControl
    {

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(SendFileControl), new PropertyMetadata(""));

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(SendFileControl), new PropertyMetadata(null));

        public double Schedule
        {
            get { return (double)GetValue(ScheduleProperty); }
            set { SetValue(ScheduleProperty, value); }
        }
        public static readonly DependencyProperty ScheduleProperty =
            DependencyProperty.Register("Schedule", typeof(double), typeof(SendFileControl), new PropertyMetadata(0d));

        public string Sendlength
        {
            get { return (string)GetValue(SendlengthProperty); }
            set { SetValue(SendlengthProperty, value); }
        }
        public static readonly DependencyProperty SendlengthProperty =
            DependencyProperty.Register("Sendlength", typeof(string), typeof(SendFileControl), new PropertyMetadata(""));

        public string Rate
        {
            get { return (string)GetValue(RateProperty); }
            set { SetValue(RateProperty, value); }
        }
        public static readonly DependencyProperty RateProperty =
            DependencyProperty.Register("Rate", typeof(string), typeof(SendFileControl), new PropertyMetadata(""));

        public UdpSendFile UdpSend { get; private set; }

        public TraFransfersFileStart TraFransfersFileStart { get; protected set; }

        public WorkClient WorkClient { get; protected set; }

        protected int TotalTransfersSize = 0;

        protected int StartTime = 0;

        protected string LockObject = "lock";

        public SendFileControl(WorkClient workclient, string remoteIP, int remotePort)
        {
            InitializeComponent();

            this.DataContext = this;

            this.WorkClient = workclient;

            int port = SocketHelper.FindNoUsedPort();
            UdpSend = new UdpSendFile(remoteIP, remotePort, port);
            UdpSend.FileSendAccept += UdpSendFile_FileSendAccept;
            UdpSend.FileSendComplete += UdpSendFile_FileSendComplete;
            UdpSend.FileSendBuffer += UdpSend_FileSendBuffer;
            UdpSend.FileSendRefuse += UdpSend_FileSendRefuse;
            UdpSend.FileSendCancel += UdpSend_FileSendCancel;

            UdpSend.Start();
        }

        public virtual void SentFile(string filename)
        {
            TraFransfersFileStart = UdpSend.SendFile(filename);

            var bmp = (TraFransfersFileStart.Image as Bitmap);
            if (bmp != null)
            {
                Icon = bmp.ConvertImageSource();
            }

            FileName = new FileInfo(filename).Name;
        }

        protected virtual void UdpSend_FileSendCancel(object sender, FileSendEventArgs e)
        {
            SendType(ControlType.SendCancel);
        }

        protected virtual void UdpSend_FileSendRefuse(object sender, FileSendEventArgs e)
        {
            SendType(ControlType.SendRefuse);
        }

        protected virtual void UdpSend_FileSendBuffer(object sender, FileSendBufferEventArgs e)
        {
            var newTime = HighPrecisionTimerHelper.PrecisionTimerOfMillisecond;
            TotalTransfersSize += e.Size;

            ShowSpeed((double)TotalTransfersSize, (double)e.SendFileManager.Length);
        }

        protected virtual void UdpSendFile_FileSendComplete(object sender, FileSendEventArgs e)
        {
            SendType(ControlType.SendComplete);
        }

        protected virtual void UdpSendFile_FileSendAccept(object sender, FileSendEventArgs e)
        {
            StartTime = HighPrecisionTimerHelper.PrecisionTimerOfMillisecond;
            // keyi jieshou
        }

        protected virtual void SendType(ControlType type)
        {
            if (type != ControlType.ReceiveStart || type != ControlType.SendStart)
            {
                this.Close();
            }
            this.Dispatcher.BeginInvoke((Action)(() => WorkClient.FileActionHandle(new FileActionArgs() { Control = this, Type = type })));
        }

        protected virtual void Cancel()
        {
            UdpSend.CancelSend(TraFransfersFileStart.MD5);
            SendType(ControlType.SendRefuse);
        }

        public virtual void Close()
        {
            if (this.UdpSend != null)
            {
                this.UdpSend.FileSendAccept -= UdpSendFile_FileSendAccept;
                this.UdpSend.FileSendComplete -= UdpSendFile_FileSendComplete;
                this.UdpSend.FileSendBuffer -= UdpSend_FileSendBuffer;
                this.UdpSend.FileSendRefuse -= UdpSend_FileSendRefuse;
                this.UdpSend.FileSendCancel -= UdpSend_FileSendCancel;
                this.UdpSend.Dispose();
                this.UdpSend = null;
            }
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        protected virtual void ShowSpeed(double currentsize, double totlesize)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                var newTime = HighPrecisionTimerHelper.PrecisionTimerOfMillisecond;
                this.Schedule = (currentsize / totlesize) * 100;
                this.Sendlength = (currentsize).GetRateText() + "/" + (totlesize).GetRateText(); ;
                double speed = (currentsize) * 1000d / (double)(newTime - StartTime);
                this.Rate = string.Format("{0}/s", speed.GetRateText());
            }));
        }

    }
}
