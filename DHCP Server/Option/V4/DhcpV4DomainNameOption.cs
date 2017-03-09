using System;
using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Xml;
using PIXIS.DHCP.Utility;

namespace PIXIS.DHCP.Option.V4
{
    public class DhcpV4DomainNameOption : BaseStringOption
    {
        public DhcpV4DomainNameOption() : this(null)
        { }
        public DhcpV4DomainNameOption(v4DomainNameOption v4DomainNameOption)
            : base(v4DomainNameOption)
        {
            SetCode(DhcpConstants.V4OPTION_DOMAIN_NAME);
            SetV4(true);
        }
    }
}