using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PlatformCommon.Manager;
using Jisons;

namespace PlatformCommon.ViewMode
{
    [Serializable]
    public class ServerData
    {
        private static ServerData instance;
        public static ServerData Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary> 用户配置文件名 </summary>
        [field: NonSerialized]
        public const string ServerDataFileName = "ServerData.xml";

        public string IP { get; set; }

        public int Port { get; set; }

        static ServerData()
        {
            LoadData();
        }

        private ServerData()
        {

        }

        public static string GetServerDataFileFullPath()
        {
            return Path.Combine(Option.UserCustomerConfigFloder, ServerDataFileName);
        }

        public static ServerData LoadData()
        {

            var fileinfo = new FileInfo(GetServerDataFileFullPath());
            if (fileinfo.Exists)
            {
                instance = fileinfo.ReadDataFromXml<ServerData>();
            }

            if (instance == null)
            {
                instance = new ServerData();
            }

            return instance;
        }

        public void SaveData()
        {
            this.WriteDataToXml(GetServerDataFileFullPath());
        }

        public void SaveData(string ip, string port)
        {
            try
            {
                this.IP = ip;
                this.Port = int.Parse(port);
                SaveData();
            }
            catch { }
        }

    }
}
