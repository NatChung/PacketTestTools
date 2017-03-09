using PIXIS.DHCP.Option.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Message
{
    public interface DhcpMessage
    {
        void SetMessageType(short msgType);
        short GetMessageType();
        DhcpOption GetDhcpOption(int optionCode);
    }
}
