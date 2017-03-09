using System;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4HostnameOption : BaseStringOption
    {
        public DhcpV4HostnameOption() : this(null)
        {
        }
        public DhcpV4HostnameOption(v4HostnameOption v4HostnameOption)
            : base()
        {
            SetCode(DhcpConstants.V4OPTION_HOSTNAME);
            SetV4(true);
        }
    }
}