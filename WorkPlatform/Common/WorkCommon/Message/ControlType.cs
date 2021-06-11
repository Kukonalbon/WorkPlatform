using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkCommon.Message
{
    public enum ControlType
    {
        SendStart,
        SendComplete,
        SendRefuse,
        SendCancel,

        ReceiveStart,
        ReceiveComplete,
        ReceiveRefuse,
        ReceiveCancel,
    }
}
