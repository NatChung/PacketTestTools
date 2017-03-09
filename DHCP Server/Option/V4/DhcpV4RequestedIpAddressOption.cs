using System;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4RequestedIpAddressOption : BaseIpAddressOption
    {
        public DhcpV4RequestedIpAddressOption() : this(null)
        { }

        public DhcpV4RequestedIpAddressOption(v4RequestedIpAddressOption v4RequestedIpAddressOption)
            :base(v4RequestedIpAddressOption)
        {
            SetCode(DhcpConstants.V4OPTION_REQUESTED_IP);
            base.SetV4(true);
        }
    }
}