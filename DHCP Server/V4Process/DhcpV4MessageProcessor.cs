using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIXIS.DHCP.Message;
using System.Net;

namespace PIXIS.DHCP.V4Process
{
    public interface DhcpV4MessageProcessor
    {
        DhcpV4Message ProcessMessage(IPAddress localAddress);
    }
}