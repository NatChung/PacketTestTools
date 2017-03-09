using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP
{
    public enum DHCPMessageType
    {
        DISCOVER,
        OFFER,
        V4REQUEST,
        DECLINE,
        ACK,
        NAK,
        RELEASE,
        INFORM,
        Undefined,
        SOLICIT,
        ADVERTISE,
        REPLY,
        V6REQUEST,
    }
}
