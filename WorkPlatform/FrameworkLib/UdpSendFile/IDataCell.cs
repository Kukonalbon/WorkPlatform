using System;
using System.Collections.Generic;
using System.Text;

namespace UdpSendFiles
{

    public interface IDataCell
    {
        byte[] ToBuffer();

        void FromBuffer(byte[] buffer);
    }
}
