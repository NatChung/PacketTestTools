using System;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4TimeServersOption : BaseIpAddressListOption
    {
        public DhcpV4TimeServersOption() : this(null) { }
        public DhcpV4TimeServersOption(v4TimeServersOption v4TimeServersOption)
            :base(v4TimeServersOption)
        {
            SetCode(DhcpConstants.V4OPTION_TIME_SERVERS);
            SetV4(true);
        }
    }
}