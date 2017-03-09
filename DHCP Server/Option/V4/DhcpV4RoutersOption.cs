using System;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4RoutersOption : BaseIpAddressListOption
    {
        public DhcpV4RoutersOption() : this(null) { }

        public DhcpV4RoutersOption(v4RoutersOption v4RoutersOption)
            : base(v4RoutersOption)
        {
            SetCode(DhcpConstants.V4OPTION_ROUTERS);
            SetV4(true);
        }
    }
}