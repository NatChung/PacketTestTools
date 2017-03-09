using System;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.V4Process;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4ServerIdOption : BaseIpAddressOption
    {
        public DhcpV4ServerIdOption() : this(null)
        {
        }
        public DhcpV4ServerIdOption(v4ServerIdOption serverIdOption)
            :base(serverIdOption)
        {
            SetCode(DhcpConstants.V4OPTION_SERVERID);
            SetV4(true);
        }
    }
}