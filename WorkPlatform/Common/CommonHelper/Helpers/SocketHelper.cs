using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Jisons
{
    public static class SocketHelper
    {

        public static int FindNoUsedPort(int defaultport = 12001)
        {
            int port = defaultport;
            UdpClient u = null;
            bool isfind = false;
            while (!isfind)
            {
                try
                {
                    u = new UdpClient(port);
                    isfind = true;
                }
                catch
                {
                    port++;
                }
                finally
                {
                    if (u != null)
                    {
                        u.Close();
                    }
                }
            }
            return port;
        }

        public static IPEndPoint CreatIPEndPoint(string ip = "127.0.0.1")
        {
            return new IPEndPoint(System.Net.IPAddress.Parse(ip), Jisons.SocketHelper.FindNoUsedPort());
        }
    }
}
