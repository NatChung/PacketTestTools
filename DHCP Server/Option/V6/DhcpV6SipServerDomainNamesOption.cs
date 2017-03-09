using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6SipServerDomainNamesOption : BaseDomainNameListOption
    {
        /**
	 * Instantiates a new dhcp sip server domain names option.
	 */
        public DhcpV6SipServerDomainNamesOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp sip server domain names option.
         * 
         * @param sipServerDomainNamesOption the sip server domain names option
         */
        public DhcpV6SipServerDomainNamesOption(v6SipServerDomainNamesOption sipServerDomainNamesOption) : base(sipServerDomainNamesOption)
        {
            SetCode(DhcpConstants.V6OPTION_SIP_SERVERS_DOMAIN_LIST);
        }
    }
}