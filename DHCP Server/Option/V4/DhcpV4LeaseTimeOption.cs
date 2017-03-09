using System;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4LeaseTimeOption: BaseUnsignedIntOption
    {
        public DhcpV4LeaseTimeOption(): this(null)
        {
        }
        /**
	 * Instantiates a new dhcpv4 lease time option.
	 * 
	 * @param v4LeaseTimeOption the v4 lease time option
	 */
        public DhcpV4LeaseTimeOption(v4LeaseTimeOption v4LeaseTimeOption) :  base(v4LeaseTimeOption)
        {
            SetCode(DhcpConstants.V4OPTION_LEASE_TIME);
            SetV4(true);
        }
    }
}