using System;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4NetbiosNameServersOption : BaseIpAddressListOption
    {
        public DhcpV4NetbiosNameServersOption() : this(null)
        { }

        public DhcpV4NetbiosNameServersOption(v4NetbiosNameServersOption v4NetbiosNameServersOption)
            : base(v4NetbiosNameServersOption)
        {
            SetCode(DhcpConstants.V4OPTION_NETBIOS_NAME_SERVERS);
            SetV4(true);
        }
    }
}