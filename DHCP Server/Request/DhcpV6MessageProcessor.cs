using PIXIS.DHCP.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Request
{
    public interface DhcpV6MessageProcessor
    {
        DhcpV6Message ProcessMessage();
    }
}
