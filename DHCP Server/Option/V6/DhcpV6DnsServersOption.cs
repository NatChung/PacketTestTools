using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6DnsServersOption : BaseIpAddressListOption
    {
        /**
	     * Instantiates a new dhcp dns servers option.
	     */
        public DhcpV6DnsServersOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp dns servers option.
         * 
         * @param dnsServersOption the dns servers option
         */
        public DhcpV6DnsServersOption(v6DnsServersOption dnsServersOption):base(dnsServersOption)
        {
            SetCode(DhcpConstants.V6OPTION_DNS_SERVERS);
        }
    }
}
