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
using CommonHelper;
using Jisons;
using UdpSendFiles;
using PlatformCommon.Message;

namespace ClientManager.Controls
{
    /// <summary>
    /// ReceiveFileControl.xaml 的交互逻辑
    /// </summary>
    public partial class ReceiveFileControl : UserControl
    {

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(ReceiveFileControl), new PropertyMetadata(""));

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(ReceiveFileControl), new PropertyMetadata(null));

        public double Schedule
        {
            get { return (double)GetValue(ScheduleProperty); }
            set { SetValue(ScheduleProperty, value); }
        }
        public static readonly DependencyProperty ScheduleProperty =
            DependencyProperty.Register("Schedule", typeof(double), typeof(ReceiveFileControl), new PropertyMetadata(0d));

        public string Sendlength
        {
            get { return (string)GetValue(SendlengthProperty); }
            set { SetValue(SendlengthProperty, value); }
        }
        public static readonly DependencyProperty SendlengthProperty =
            DependencyProperty.Register("Sendlength", typeof(string), typeof(ReceiveFileControl), new PropertyMetadata(""));

        public string Rate
        {
            get { return (string)GetValue(RateProperty); }
            set { SetValue(RateProperty, value); }
        }
        public static readonly DependencyProperty RateProperty =
            DependencyProperty.Register("Rate", typeof(string), typeof(ReceiveFileControl), new PropertyMetadata(""));

        public UdpReceiveFile UDPReceive { get; private set; }

        public RequestSendFileEventArgs SendFileEventArgs { get; protected set; }

        protected int TotalTransfersSize = 0;

        public WorkClient WorkClient { get; protected set; }

        protected int StartTime = 0;

        protected string LockObject = "lock";

        public virtual int MessagePort
        {
            get
            {
                return UDPReceive != null ? UDPReceive.Port : -1; ;
            }
        }

        public virtual int ReceivePort
        {
            get
            {
                return MessagePort;
            }
        }

        public ReceiveFileControl(WorkClient workclient)
        {
            InitializeComponent();

            StartReceiveFile();

            this.DataContext = this;

            this.WorkClient = workclient;
        }

        protected virtual void StartReceiveFile()
        {
            int port = SocketHelper.FindNoUsedPort();
            UDPReceive = new UdpReceiveFile(port);
            UDPReceive.Start();

            UDPReceive.RequestSendFile += UDPReceive_RequestSendFile;
            UDPReceive.FileReceiveBuffer += UDPReceive_FileReceiveBuffer;
            UDPReceive.FileReceiveComplete += UDPReceive_FileReceiveComplete;
            UDPReceive.FileReceiveCancel += UDPReceive_FileReceiveCancel;
        }

        protected virtual void UDPReceive_FileReceiveCancel(object sender, FileReceiveEventArgs e)
        {
            SendType(ControlType.ReceiveCancel);
        }

        protected virtual void UDPReceive_FileReceiveBuffer(object sender, FileReceiveBufferEventArgs e)
        {
            TotalTransfersSize += e.Size;
            ShowSpeed(TotalTransfersSize, (double)SendFileEventArgs.TraFransfersFileStart.Length);
        }

        protected virtual void UDPReceive_RequestSendFile(object sender, RequestSendFileEventArgs e)
        {
            SendFileEventArgs = new RequestSendFileEventArgs(e.TraFransfersFileStart, e.RemoteIP);
            SendFileEventArgs.Path = "c:\\";

            var bmp = e.TraFransfersFileStart.Image as Bitmap;

            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                if (bmp != null)
                {
                    Icon = bmp.ConvertImageSource();
                }

                FileName = e.TraFransfersFileStart.FileName;
            }));
        }

        protected virtual void UDPReceive_FileReceiveComplete(object sender, FileReceiveEventArgs e)
        {
            SendType(ControlType.ReceiveComplete);
        }

        protected virtual void Save()
        {
            TotalTransfersSize = 0;
            UDPReceive.AcceptReceive(SendFileEventArgs);
            StartTime = HighPrecisionTimerHelper.PrecisionTimerOfMillisecond;
        }

        protected virtual void SaveAs()
        {
            var fileinfo = new FileInfo(SendFileEventArgs.TraFransfersFileStart.FileName);
            TotalTransfersSize = 0;
            var savefile = DialogHelper.SaveFile(fileinfo.Extension);
            if (!string.IsNullOrWhiteSpace(savefile))
            {
                fileinfo = new FileInfo(savefile);
                SendFileEventArgs.Path = fileinfo.DirectoryName;
                SendFileEventArgs.TraFransfersFileStart.FileName = fileinfo.Name;
                UDPReceive.AcceptReceive(SendFileEventArgs);
                StartTime = HighPrecisionTimerHelper.PrecisionTimerOfMillisecond;
            }
        }

        protected virtual void Refuse()
        {
            SendFileEventArgs.Cancel = true;
            UDPReceive.AcceptReceive(SendFileEventArgs);
            SendType(ControlType.ReceiveRefuse);
        }

        protected virtual void Cancel()
        {
            UDPReceive.CancelReceive(SendFileEventArgs.TraFransfersFileStart.MD5, SendFileEventArgs.RemoteIP);
            SendType(ControlType.ReceiveRefuse);
        }

        protected virtual void SendType(ControlType type)
        {
            if (type != ControlType.ReceiveStart || type != ControlType.SendStart)
            {
                this.Close();
            }

            this.Dispatcher.BeginInvoke((Action)(() => WorkClient.FileActionHandle(new FileActionArgs()
            {
                Control = this,
                Type = type
            })));
        }

        public virtual void Close()
        {
            if (this.UDPReceive != null)
            {
                this.UDPReceive.RequestSendFile -= UDPReceive_RequestSendFile;
                this.UDPReceive.FileReceiveBuffer -= UDPReceive_FileReceiveBuffer;
                this.UDPReceive.FileReceiveComplete -= UDPReceive_FileReceiveComplete;
                this.UDPReceive.FileReceiveCancel -= UDPReceive_FileReceiveCancel;
                this.UDPReceive.Dispose();
                this.UDPReceive = null;
            }
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

        #region 界面按钮

        private void save_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void saveas_Click(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Refuse();
        }

        #endregion
    }
}
