using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace Jisons
{
    public static class NetHelper
    {
        /// <summary>
        /// 获取网卡信息
        /// </summary>
        /// <returns></returns>
        public static List<NetInfoStruct> GetNetInfo()
        {
            List<NetInfoStruct> netList = new List<NetInfoStruct>();
            ManagementObjectSearcher query = new
      ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");
            ManagementObjectCollection queryCollection = query.Get();
            foreach (ManagementObject mo in queryCollection)
            {
                NetInfoStruct net = new NetInfoStruct();
                net.IP = mo["IPAddress"] as string[];
                net.Subnet = mo["IPSubnet"] as string[];
                net.Gateway = mo["DefaultIPGateway"] as string[];
                net.DNS = mo["DNSServerSearchOrder"] as string[];
                net.Description = mo["Description"] as string;
                net.Mac = mo["MACAddress"] as string;
                net.Name = GetName(mo["Description"] as string);
                netList.Add(net);
            }
            return netList;
        }

        /// <summary>
        /// 获取网卡名称
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        private static string GetName(string description)
        {
            string name = string.Empty;
            if (description.Contains("Virtual"))
                name = "虚拟连接";
            else
            {
                if (description.Contains("Wireless"))
                    name += "无线连接";
                else
                    name += "本地连接";
            }
            return name;
        }

    }


    public struct NetInfoStruct
    {
        public string[] IP;
        public string[] Subnet;
        public string[] Gateway;
        public string[] DNS;
        public string Mac;
        public string Name;
        public string Description;
        public NetInfoStruct
            (string[] ip, string[] subnet, string[] getway, string[] dns, string mac, string name, string description)
        {
            this.IP = ip;
            this.Subnet = subnet;
            this.Gateway = getway;
            this.DNS = dns;
            this.Mac = mac;
            this.Name = name;
            this.Description = description;
        }
    }
}
