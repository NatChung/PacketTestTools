using PIXIS.DHCP.Option.Base;
using PIXIS.DHCP.Utility;
using PIXIS.DHCP.Xml;

namespace PIXIS.DHCP.Option.V6
{
    public class DhcpV6ServerUnicastOption : BaseIpAddressOption
    {
        /**
        * Instantiates a new dhcp server unicast option.
       */
        public DhcpV6ServerUnicastOption() : this(null)
        {
        }

        /**
         * Instantiates a new dhcp server unicast option.
         * 
         * @param serverUnicastOption the server unicast option
         */
        public DhcpV6ServerUnicastOption(v6ServerUnicastOption serverUnicastOption):base(serverUnicastOption)
        {
            SetCode(DhcpConstants.V6OPTION_UNICAST);
        }
    }
}