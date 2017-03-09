using PIXIS.DHCP.Config;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Request.Bind
{
    public interface BindingPool : DhcpConfigObject
    {
        IPAddress GetStartAddress();
        IPAddress GetEndAddress();
        IPAddress GetNextAvailableAddress();
        IPAddress GetNextAvailableAddress(string str1, string str2);
        void SetUsed(IPAddress addr);
        void SetFree(IPAddress addr);
        bool Contains(IPAddress addr);
        linkFilter GetLinkFilter();
        BigInteger GetSize();
        //AssignDhcpV6Rule GetV6AssignRule();
        bool IsFree(BigInteger free);
    }
}
