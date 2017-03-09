using System;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4SubnetMaskOption : BaseIpAddressOption
    {
        public DhcpV4SubnetMaskOption() : this(null) { }
        public DhcpV4SubnetMaskOption(v4SubnetMaskOption v4SubnetMaskOption)
            : base(v4SubnetMaskOption)
        {
            SetCode(DhcpConstants.V4OPTION_SUBNET_MASK);
            SetV4(true);
        }
    }
}