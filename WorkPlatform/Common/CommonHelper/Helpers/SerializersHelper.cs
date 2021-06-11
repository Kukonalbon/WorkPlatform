﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Jisons
{
    public static class SerializersHelper
    {

        /// <summary>
        /// 将对象流转换成二进制流
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static MemoryStream SerializeStream(this object request)
        {
            var memStream = new MemoryStream();
            new BinaryFormatter().Serialize(memStream, request);
            return memStream;
        }

        public static byte[] SerializeBinary(this object request)
        {
            using (var memStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(memStream, request);
                memStream.Position = 0;
                byte[] datas = new byte[memStream.Length];
                memStream.Read(datas, 0, datas.Count());
                return datas;
            }
        }

        public static T DeSerializeBinary<T>(this byte[] datas) where T : class
        {
            using (var memStream = new MemoryStream(datas))
            {
                memStream.Position = 0;
                T newobj = new BinaryFormatter().Deserialize(memStream) as T;
                return newobj;
            }
        }

        public static T DeSerializeBinaryStruct<T>(this byte[] datas) where T : struct
        {
            using (var memStream = new MemoryStream(datas))
            {
                memStream.Position = 0;
                T newobj = (T)(new BinaryFormatter().Deserialize(memStream));
                return newobj;
            }
        }


        /// <summary>
        /// 将二进制流转换成对象
        /// </summary>
        /// <param name="memStream"></param>
        /// <returns></returns>
        public static object DeSerializeStream(this Stream memStream)
        {
            memStream.Position = 0;
            return new BinaryFormatter().Deserialize(memStream);
        }
    }
}
