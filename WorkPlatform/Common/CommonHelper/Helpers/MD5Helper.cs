using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Jisons
{
    public static class MD5Helper
    {
        public static string GetFastMD5(string fileName)
        {
            string hashStr = string.Empty;
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                hashStr = GetFastMD5(fs, fileName);
                fs.Close();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return hashStr;
        }

        /// <summary>
        /// 因为对于大文件来说，全文MD5效率过低，因此采取取前5k位+最后5k位进行判定文件相同，通过此项验证相同文件
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string GetFastMD5(Stream stream, string name = "")
        {
            var md5 = new MD5CryptoServiceProvider();

            byte[] datas = new byte[10000];
            if (stream.Length > 10000)
            {
                stream.Position = 0;
                stream.Read(datas, 0, 5000);

                stream.Position = stream.Length - 5000;
                stream.Read(datas, 5000, 5000);

                return md5.ComputeHash(datas).ConvertToHexString() + stream.Length;
            }
            else
            {
                return md5.ComputeHash(stream).ConvertToHexString();
            }
        }


        public static string GetMD5(this string fileName)
        {
            string str = string.Empty;
            try
            {
                using (var file = new FileStream(fileName, FileMode.Open))
                {
                    str = GetMD5(file);
                }
            }
            catch { }
            return str;
        }

        public static string GetMD5(this Stream stream)
        {
            StringBuilder sb = new StringBuilder();
            byte[] retVal = (new System.Security.Cryptography.MD5CryptoServiceProvider()).ComputeHash(stream);
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

    }
}
