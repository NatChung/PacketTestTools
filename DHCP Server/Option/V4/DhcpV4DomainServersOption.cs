using System;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4DomainServersOption : BaseIpAddressListOption
    {
        public DhcpV4DomainServersOption() : base(null)
        { }
        public DhcpV4DomainServersOption(v4DomainServersOption v4DomainServersOption) 
            : base(v4DomainServersOption)
        {
            SetCode(DhcpConstants.V4OPTION_DOMAIN_SERVERS);
            SetV4(true);
        }
       
    }
}