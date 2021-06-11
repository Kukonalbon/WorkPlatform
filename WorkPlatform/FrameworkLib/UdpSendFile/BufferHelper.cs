using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace UdpSendFiles
{

    public class BufferHelper
    {
        public static byte[] Serialize(object obj)
        {
            var stream = new MemoryStream();
            new BinaryFormatter().Serialize(stream, obj);
            byte[] datas = stream.ToArray();
            stream.Dispose();
            return datas;
        }

        public static object Deserialize(byte[] datas, int index)
        {
            var stream = new MemoryStream(datas, index, datas.Length - index);
            object obj = new BinaryFormatter().Deserialize(stream);
            stream.Dispose();
            return obj;
        }
    }
}
