using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6PanaAgentAddressesOption : BaseIpAddressListOption
    {
        /**
	 * Instantiates a new dhcp pana agent addresses option.
	 */
        public DhcpV6PanaAgentAddressesOption(): this(null)
        {
        }

        /**
         * Instantiates a new dhcp pana agent addresses option.
         * 
         * @param panaAgentAddressesOption the pana agent addresses option
         */
        public DhcpV6PanaAgentAddressesOption(v6PanaAgentAddressesOption panaAgentAddressesOption):base(panaAgentAddressesOption)
        {
            SetCode(DhcpConstants.V6OPTION_PANA_AGENT_ADDRESSES);
        }
    }
}