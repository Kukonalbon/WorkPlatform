using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jisons
{
    public static class ByteHelper
    {
        public static string ConvertToHexString(this byte[] values)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte value in values)
            {
                sb.AppendFormat("{0:X2}", value);
            }
            return sb.ToString();
        }
    }
}
