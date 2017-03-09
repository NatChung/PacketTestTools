using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6SipServerAddressesOption : BaseIpAddressListOption
    {
        /**
	     * Instantiates a new dhcp sip server addresses option.
	     */
        public DhcpV6SipServerAddressesOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp sip server addresses option.
         * 
         * @param sipServerAddressesOption the sip server addresses option
         */
        public DhcpV6SipServerAddressesOption(v6SipServerAddressesOption sipServerAddressesOption) : base(sipServerAddressesOption)
        {
            SetCode(DhcpConstants.V6OPTION_SIP_SERVERS_ADDRESS_LIST);
        }
    }
}